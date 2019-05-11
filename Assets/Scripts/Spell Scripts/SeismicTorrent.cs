﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeismicTorrent : Spell {

	// Use this for initialization
	public override void Start () {
        base.Start();
        spellName = "Seismic Torrent";
        spellCode = "234";
        castTime = 25;
        postCastTime = 15;
        channelTime = 0;
        duration = 60;
        cost = 40;
    }

    public override void CastSpell() {
        base.CastSpell();

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = transform.position;
        currentAttack.transform.rotation = transform.rotation;
        currentAttack.transform.parent = transform;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "seismicTorrent",
            _HitboxAnimation: "defaultCube",
            _xScale: 4f,
            _yScale: 1f,
            _zScale: 1f,
            _attackDelay: 10, //attack begins quickly
            _attackDuration: 60, //60 frames within which the attack is active
            _attackEnd: 0, //when the attack ends it's done.
            _hitsAirborne: false, //hits standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: false, //doesn't make contact.
            _unblockable: 3); //can't be blocked or parried.

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.1f * charStat.STR) + (0.2f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 5f + 0.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Magic;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _causesAirborne: 20,
            _causesFloored: 180,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: 0f);

        //add impact damage
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.2f * charStat.STR) + (0.1f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 10f + 0.6f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Impact;
        currentAttack.onHit.damageInstances.Add(tempDamage);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = currentAttack.onHit; //this move is unblockable 3 so no guard stuff.

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //this move doesn't have any additional effect against a vulnerable target

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move hurts floored and standing targets alike

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //this move doesn't even hit airborne targets 

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations - or don't
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        //animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        StartCoroutine(movement.motor.TimedBurst(0.2f, 400f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward


    }

    // Update is called once per rendered frame
    public override void Update () {
		
	}

    // FixedUpdate is called once per physics frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (ready == 2) {
            transform.Translate(transform.forward * 0.25f, Space.World);
        }
    }
    
}
