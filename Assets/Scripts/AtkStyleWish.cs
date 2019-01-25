using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyleWish : AtkStyle {

    public enum attackStates { idle, fEvade, bEvade, rEvade, lEvade, spellcast, fParry, bParry, bladework1, bladework2, bladework3 };

    public StatusManager status;
    public CharStatSheet stat;

    private Attack currentAttack;
    public List<Attack> instantiatedAttacks;
    public GameObject attack;
    public Animator playerAnimator;
    public RuntimeAnimatorController hitboxAnimatorController;
    public PlayerMovement movement;

    public Attack.damage tempDamage;

    public attackStates state;
    public int wraithMode; //wish style has a powered-up state (which I may or may not have time to implement wahey)
    public float evadespeed; //you may want attack-break evades to be faster or slower than regular ones
    public float evadetime;
    public int returnToIdle; //the number of frames until the player returns to idle after a move
    int idleCounter;

    // Use this for initialization
    void Start () {
        state = attackStates.idle;
        instantiatedAttacks = new List<Attack>();
	}

    public void defaultAttack() {
        currentAttack = Instantiate(attack).GetComponent<Attack>();
    }

    public void evadeForward() {
        movement.evade(evadespeed, 0f, evadetime);
        playerAnimator.Play("forwardRoll");
    }

    public void evadeBack() {
        movement.evade(-evadespeed, 0f, evadetime);
        playerAnimator.Play("forwardRoll");
    }

    public void evadeRight() {
        movement.evade(0f, evadespeed, evadetime);
        playerAnimator.Play("forwardRoll");
    }

    public void evadeLeft() {
        movement.evade(0f, -evadespeed, evadetime);
        playerAnimator.Play("forwardRoll");
    }

    public void standardBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.parent = this.gameObject.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework1",
            _HitboxAnimation: "standardBladework1", //having names for both animations seems fairly self-explanatory
            _attackDelay: 20, //delay of 20 frames before the attack starts
            _attackDuration: 30, //30 frames within which the attack is active
            _attackEnd: 40, //and 40 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true); //and makes contact.
        
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 10f + (0.25f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage }, 
            _causesFlinch: true, 
            _causesStun: 50, 
            _onHitForwardBackward: -300f, 
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost : 10f, 
            _causesGuardStun: 40, 
            _onHitForwardBackward: -250f, 
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 14f + (0.25f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 70,
            _causesFloored: 120,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        tempDamage.damageAmount = 8f + (0.25f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 12f + (0.25f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesFloored: 120,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        playerAnimator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked

        idleCounter = 0; //always remember to reset the idle counter
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void FixedUpdate() {

        //manage attack progression
        for (int i = 0; i < instantiatedAttacks.Count; i++) {
            if (instantiatedAttacks[i].data.attackDelay > 0) {

                instantiatedAttacks[i].data.attackDelay -= 1;
            }

            else if (instantiatedAttacks[i].data.attackDuration > 0) {

                instantiatedAttacks[i].data.attackDuration -= 1;
            }

            else if (instantiatedAttacks[i].data.attackEnd > 0) {

                instantiatedAttacks[i].data.attackEnd -= 1;
            }

        }

        //manage returning to idle
        if (state != attackStates.idle) {
            idleCounter++;
        }

        if (idleCounter >= returnToIdle) {
            idleCounter = 0;
            state = attackStates.idle;
        }
    }
}
