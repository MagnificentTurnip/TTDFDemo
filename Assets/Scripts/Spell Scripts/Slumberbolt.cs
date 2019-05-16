using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slumberbolt : Spell {

    public Hittable hit;
    public GameObject gfx;

    public AudioClip boltClip;

	// Use this for initialization
	public override void Start () {
        base.Start();
        spellName = "Slumberbolt";
        spellCode = "2";
        castTime = 15;
        postCastTime = 10;
        channelTime = 0;
        duration = 60;
        cost = 20;
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
            _atkHitBox: currentAttack.gameObject.AddComponent<SphereCollider>(), //this attack uses a box collider
            _GFXAnimation: "slumberbolt",
            _HitboxAnimation: "defaultSphere",
            _xScale: 1f,
            _yScale: 1f,
            _zScale: 1f,
            _attackDelay: 10, //attack begins quickly
            _attackDuration: 60, //60 frames within which the attack is active
            _attackEnd: 0, //when the attack ends it's done.
            _hitsAirborne: true, //hits standing and floored.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: false, //doesn't make contact.
            _unblockable: 1); //can't be blocked.

        currentAttack.clip = boltClip;

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = sphere; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics

        //set the attack's properties on hit (all unset properties are defaults)
        currentAttack.onHit = new Attack.HitProperties(
            _causesFlinch: true,
            _causesStun: 60,
            _onHitForwardBackward: 0f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _causesFlinch: false,
            _causesGuardStun: 40,
            _onHitForwardBackward: -600f,
            _onHitRightLeft: 0f);

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

    // Update is called once per rendered frame
    public override void Update () {
		
	}

    // FixedUpdate is called once per physics frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (ready == 2) {

            transform.Translate(transform.forward * 0.5f, Space.World);

            if (currentAttack != null) {
                if (currentAttack.thingsHit.Count > 0) {
                    hit = currentAttack.thingsHit[0].GetComponent<Hittable>();
                    if (hit.status.parryFrames <= 0) {
                        if (charStat != null) {
                            if ((hit.stat.HP <= 10 + (0.3 * charStat.WIL)) || (hit.stat.HP <= (hit.stat.MaxHP / 100) * (5 + (0.05f * charStat.WIL)))) {
                                hit.status.unconscious = true;
                            }
                        }
                        else {
                            if ((hit.stat.HP <= 10 + (0.5 * stat.Level)) || (hit.stat.HP <= (hit.stat.MaxHP / 100) * (5 + (0.4f * stat.Level)))) {
                                hit.status.unconscious = true;
                            }
                        }
                    }
                    
                    DestroyAllAttacks();
                    Destroy(gfx);
                }
            }
        }
    }
    
}
