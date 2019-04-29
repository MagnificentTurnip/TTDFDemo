using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneBarrage : Spell {

    public int arcaneBarrageNo;

	// Use this for initialization
	public override void Start () {
        base.Start();
        switch (arcaneBarrageNo) {
            case 1:
                spellCode = "1";
                break;
            case 2:
                spellCode = "11";
                break;
            case 3:
                spellCode = "111";
                break;
            case 4:
                spellCode = "1111";
                break;
            case 5:
                spellCode = "11111";
                break;
        }
        spellName = "Arcane Barrage";
        castTime = 0;
        postCastTime = 15;
        channelTime = 0;
        duration = 76;
        cost = -5 + (15 * arcaneBarrageNo); //COST MUST BE PRE-SET IN THE PREFAB TO ALLOW MISFIRING
    }

    public void Bolt(int boltNumber) {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = transform.position;
        currentAttack.transform.rotation = transform.rotation;
        currentAttack.transform.parent = transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<SphereCollider>(), //this attack uses a box collider
            _GFXAnimation: "arcaneBarrage",
            _HitboxAnimation: "defaultSphere",
            _xScale: 0.2f,
            _yScale: 0.2f,
            _zScale: 0.2f,
            _attackDelay: 40 + boltNumber * 3, //attack begins quickly
            _attackDuration: 5, //5 frames within which the attack is active
            _attackEnd: 0, //when the attack ends it's done.
            _hitsAirborne: true, //hits all.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: false, //doesn't make contact.
            _unblockable: 1); //can't be blocked

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = sphere; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 1f + (0.05f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 1f + 0.25f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Magic;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _onHitForwardBackward: 0f,
            _onHitRightLeft: 0f);
        
        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _causesGuardStun: 5,
            _onHitForwardBackward: -50f);

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
    }

    public override void CastSpell() {
        base.CastSpell();
        if (mouseTarget) {
            if (target != null && target.gameObject.tag.Contains("Enm")) {
                //do spell stuff
                switch (arcaneBarrageNo) {
                    case 1:
                        Bolt(1);
                        Bolt(2);
                        break;
                    case 2:
                        Bolt(1);
                        Bolt(2);
                        Bolt(3);
                        Bolt(4);
                        break;
                    case 3:
                        Bolt(1);
                        Bolt(2);
                        Bolt(3);
                        Bolt(4);
                        Bolt(5);
                        Bolt(6);
                        break;
                    case 4:
                        Bolt(1);
                        Bolt(2);
                        Bolt(3);
                        Bolt(4);
                        Bolt(5);
                        Bolt(6);
                        Bolt(7);
                        Bolt(8);
                        break;
                    case 5:
                        Bolt(1);
                        Bolt(2);
                        Bolt(3);
                        Bolt(4);
                        Bolt(5);
                        Bolt(6);
                        Bolt(7);
                        Bolt(8);
                        Bolt(9);
                        Bolt(10);
                        break;
                }
            } else {
                //refund the spell
                status.channelLock -= castTime + channelTime;
                if (charStat != null) {
                    charStat.MP += cost;
                } else {
                    stat.MP += cost;
                }
            }
        } else {
            switch (arcaneBarrageNo) {
                case 1:
                    Bolt(1);
                    Bolt(2);
                    break;
                case 2:
                    Bolt(1);
                    Bolt(2);
                    Bolt(3);
                    Bolt(4);
                    break;
                case 3:
                    Bolt(1);
                    Bolt(2);
                    Bolt(3);
                    Bolt(4);
                    Bolt(5);
                    Bolt(6);
                    break;
                case 4:
                    Bolt(1);
                    Bolt(2);
                    Bolt(3);
                    Bolt(4);
                    Bolt(5);
                    Bolt(6);
                    Bolt(7);
                    Bolt(8);
                    break;
                case 5:
                    Bolt(1);
                    Bolt(2);
                    Bolt(3);
                    Bolt(4);
                    Bolt(5);
                    Bolt(6);
                    Bolt(7);
                    Bolt(8);
                    Bolt(9);
                    Bolt(10);
                    break;
            }
        }
    }

    // Update is called once per rendered frame
    public override void Update () {
		
	}

    // FixedUpdate is called once per physics frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (ready == 2) {
            for (int i = 0; i < instantiatedAttacks.Count; i++) {
                instantiatedAttacks[i].transform.LookAt(target.transform);
                if (instantiatedAttacks[i].data.attackDelay > 0) {
                    instantiatedAttacks[i].transform.Translate((target.transform.position - instantiatedAttacks[i].transform.position) / instantiatedAttacks[i].data.attackDelay, Space.World); //move toward the target
                    if (i % 2 == 0) { //even arcane bolts veer left
                        instantiatedAttacks[i].transform.Translate(transform.right * (0.01f * (duration - instantiatedAttacks[i].data.attackDelay)), Space.World);
                    }
                    else { //odd arcane bolts veer right
                        instantiatedAttacks[i].transform.Translate(transform.right * (0.01f * -(duration - instantiatedAttacks[i].data.attackDelay)), Space.World);
                    }
                    instantiatedAttacks[i].transform.position = new Vector3(instantiatedAttacks[i].transform.position.x, 1.5f, instantiatedAttacks[i].transform.position.z);
                } else {
                    instantiatedAttacks[i].transform.position = target.transform.position;
                }
                
            }
        }
    }
    
}
