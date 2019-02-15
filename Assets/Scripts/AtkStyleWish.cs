using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyleWish : AtkStyle {

    public enum attackStates { drawing, idle, fEvade, bEvade, rEvade, lEvade, spellcast, fParry, bParry, bladework1, bladework2, bladework3, bladework4 }; //Think I'm going to need a guard state because guarding between attacks is possible and not sure if I want that
    public attackStates state;

    public GameObject rgtHndBone; //just a couple of relevant bones for the sake of making sonme simple animations easier
    public GameObject lftHndBone;

    new public CharStatSheet stat; //this is an attack style for characters, so the statsheet in base AtkStyle needs to be overwritten by a charstatsheet

    public Attack.damage tempDamage;

    public int wraithMode; //wish style has a powered-up state (which I may or may not have time to implement wahey)
    public float evadespeed; //you may want attack-break evades to be faster or slower than regular ones
    public float evadetime;


    // Use this for initialization
    void Start () {
        state = attackStates.idle;
        instantiatedAttacks = new List<Attack>();
	}

    public override void forceIdle() {
        state = attackStates.idle;
    }

    public override void returnToIdle() {
        //manage returning to idle
        if (state != attackStates.idle) {
            if (state != attackStates.drawing) {
                status.toIdleLock = true;
            }
            idleCounter--;
        }

        if (idleCounter <= 0 && state != attackStates.drawing) {
            idleCounter = 0;
            state = attackStates.idle;
            status.toIdleLock = false;
        }
    }

    public override void bParry() {
        animator.Play("bParry");
        if (status.sheathed == false) {
            state = attackStates.bParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        status.parryLock = 60;
        status.parryFrames = 10;
        movement.motor.instantBurst(-100f, 50f);
    }

    public override void fParry() {
        animator.Play("fParry");
        if (status.sheathed == false) {
            state = attackStates.fParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        status.parryLock = 60;
        status.parryFrames = 10;
        movement.motor.instantBurst(400f, -50f);
    }

    public void defaultAttack() {
        currentAttack = Instantiate(attack).GetComponent<Attack>();
    }

    public void evadeForward() {
        movement.evade(evadespeed, 0f, evadetime);
        animator.Play("fRoll");
        state = attackStates.fEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void evadeBack() {
        movement.evade(-evadespeed, 0f, evadetime);
        animator.Play("bRoll", -1, 0.3f);
        state = attackStates.bEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void evadeRight() {
        movement.evade(0f, evadespeed, evadetime);
        animator.Play("rRoll");
        state = attackStates.rEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void evadeLeft() {
        movement.evade(0f, -evadespeed, evadetime);
        animator.Play("lRoll");
        state = attackStates.lEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void standardBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework1",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 35, //delay of 35 frames before the attack starts
            _attackDuration: 20, //20 frames within which the attack is active
            _attackEnd: 5, //and 5 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true, //makes contact.
            _unblockable: 0); //and isn't unblockable

        if (debug == true){
        currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 10f + (0.25f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -300f, 
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost : 10f, 
            _causesGuardStun: 20, 
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
            _causesStun: 50,
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
            _causesStun: 15,
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
            _causesStun: 50,
            _causesFloored: 100,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointToTarget(); //this move takes perfect directional input, meaning you can even direct it outside of the space within which you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.3f, 3000f, 50f, 0f, 0f, 0, 0f)); //with this move, you jump forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void standardBladework2() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 5, //delay of 5 frames before the attack starts
            _attackDuration: 25, //25 frames within which the attack is active
            _attackEnd: 5, //and 5 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true){
        currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework2; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 8f + (0.25f * stat.STR) + (0.1f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 8f,
            _causesGuardStun: 5,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //this move doesn't have any additional effect against a vulnerable target

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't even hit floored targets 

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //this move doesn't even hit airborne targets 

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(20f); //this move doesn't follow the cursor extremely well but it's a wide attack so it doesn't need to that much
        StartCoroutine(movement.motor.timedBurst(0.2f, 400f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 25; //always remember to reset the idle counter
    }

    public void standardBladework3() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position; //this attack uses the right hand bone's default movement
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 10, //delay of 10 frames before the attack starts
            _attackDuration: 18, //18 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne and standing not floored.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true){
        currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework3; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)12+(0.2xSTR)+(0.25xDEX)
        tempDamage.damageAmount = 12f + (0.2f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 25,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 20,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 15f + (0.25f * stat.STR) + (0.3f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _causesAirborne: 40,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        tempDamage.damageAmount = 8f + (0.25f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 8f + (0.15f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 15,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.2f, 400f, -50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 25; //always remember to reset the idle counter
    }

    public void standardBladework4() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position; //this attack uses the right hand bone's default movement
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 5, //delay of 5 frames before the attack starts
            _attackDuration: 15, //15 frames within which the attack is active
            _attackEnd: 20, //and 15 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true); //and makes contact.

        if (debug == true){
        currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework4; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 8f + (0.1f * stat.STR) + (0.35f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 16,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 12f + (0.1f * stat.STR) + (0.35f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesFloored: 120,
            _onHitForwardBackward: -20f,
            _onHitRightLeft: -30f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        tempDamage.damageAmount = 8f + (0.1f * stat.STR) + (0.30f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 15,
            _onHitForwardBackward: -30f,
            _onHitRightLeft: -30f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 12f + (0.1f * stat.STR) + (0.35f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesFloored: 120,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.1f, 300f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 30; //always remember to reset the idle counter
    }

    public void lightBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework1",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 10, //delay of 10 frames before the attack starts
            _attackDuration: 15, //15 frames within which the attack is active
            _attackEnd: 30, //and 30 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 5f + (0.1f * stat.STR) + (0.4f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 36,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 30f,
            _causesGuardStun: 15,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 10f + (0.1f * stat.STR) + (0.4f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _onHitForwardBackward: 200f,
            _onHitRightLeft: 50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this attack doesn't even hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //not does it hit airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.4f, 500f, -100f, -1500f, 100f, 2, 0.1f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void lightBladework2() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 7, //delay of 7 frames before the attack starts
            _attackDuration: 13, //13 frames within which the attack is active
            _attackEnd: 30, //and 30 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework2; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 4f + (0.1f * stat.STR) + (0.35f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 33,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 30f,
            _causesGuardStun: 20,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 10f + (0.1f * stat.STR) + (0.35f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 35,
            _onHitForwardBackward: 200f,
            _onHitRightLeft: 50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this attack doesn't even hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //nor does it hit airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.3f, 500f, 100f, -1200f, -100f, 2, 0.15f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void lightBladework3() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 7, //delay of 7 frames before the attack starts
            _attackDuration: 13, //13 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only;
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework3; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 3f + (0.05f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 20f,
            _causesGuardStun: 10,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //no additional properties on vulnerable hit

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't even hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //nor does it hit airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.2f, 300f, 0f, 300f, 0f, 2, 0.4f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 32, //delay of 32 frames before the attack starts
            _attackDuration: 15, //15 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 3f + (0.2f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 14,
            _onHitForwardBackward: -150f,
            _onHitRightLeft: 150f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 7,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 100f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //no additional properties on vulnerable hit

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't even hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //nor does it hit airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        //although the hitbox animation needs to be played for this new attack, the previous attack has already dealt with the gfx animation

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointToTarget(); doesn't need to be called again as the previous 'attack' has already done that
        //similarly, no additional movement needs to be performed

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void lightBladework4() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 10, //delay of 10 frames before the attack starts
            _attackDuration: 10, //10 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne and standing.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework4; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 3f + (0.2f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 15f,
            _causesGuardStun: 20,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //no additional effects on vulnerable hit

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this attack doesn't even hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 3f + (0.2f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.2f, 600f, 0f, -2500f, 0f, 2, 0.6f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 30, //delay of 30 frames before the attack starts
            _attackDuration: 10, //10 frames within which the attack is active
            _attackEnd: 35, //and 35 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 3f + (0.05f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 30f,
            _causesGuardStun: 30,
            _onHitForwardBackward: -450f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //no additional effects on vulnerable hit

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this attack doesn't even hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //nor does it hit airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        //although the hitbox animation needs to be played for this new attack, the previous attack has already dealt with the gfx animation

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointToTarget(); doesn't need to be called again as the previous 'attack' has already done that
        //similarly, no additional movement needs to be performed

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void heavyBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework1",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 25, //delay of 25 frames before the attack starts
            _attackDuration: 30, //30 frames within which the attack is active
            _attackEnd: 7, //and 7 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, and standing.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 15f + (0.45f * stat.STR) + (0.15f * stat.DEX);
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
            _SPcost: 10f,
            _causesGuardStun: 30,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 20f + (0.5f * stat.STR) + (0.15f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 70,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: 100f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't hit floored targets so just give it something to have

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 12f + (0.35f * stat.STR) + (0.15f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: 200f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointToTarget(); //this move takes perfect directional input, meaning you can even direct it outside of the space within which you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.1f, -100f, -30f, 50f, 10f, 12, 0.05f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void heavyBladework2() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 10, //delay of 20 frames before the attack starts
            _attackDuration: 15, //25 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework2; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 8f + (0.25f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 16,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 40,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 8f + (0.25f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: -100f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this attack doesn't hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //this attack doesn't hit airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(15f); //this move allows only 15 degrees of rotational adjustment
        StartCoroutine(movement.motor.timedBurst(0.1f, 300f, 0f, 8f, 0f, 16, 0.05f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 45, //delay of 20 frames before the attack starts
            _attackDuration: 20, //25 frames within which the attack is active
            _attackEnd: 15, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 8f + (0.2f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 15,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 14f + (0.25f * stat.STR) + (0.25f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = currentAttack.onHit; //this attack doesn't have additional properties on vulnerable hit

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //this move doesn't hit airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        //although the hitbox animation needs to be played for this new attack, the previous attack has already dealt with the gfx animation

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointToTarget(); doesn't need to be called again as the previous 'attack' has already done that
        //similarly, no additional movement needs to be performed

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void heavyBladework3() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 22, //delay of 22 frames before the attack starts
            _attackDuration: 25, //25 frames within which the attack is active
            _attackEnd: 15, //and 15 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne and standing.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework3; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 15f + (0.55f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesAirborne: 60,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 17,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 20f + (0.6f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 90,
            _causesAirborne: 90,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: 100f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this attack doesn't hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 12f + (0.4f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _causesAirborne: 30,
            _onHitForwardBackward: -600f,
            _onHitRightLeft: 200f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(25f); //this move allows 25 degrees of directional adjustment
        StartCoroutine(movement.motor.timedBurst(0.7f, 400f, -50f, 0f, 0f, 0, 0f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void heavyBladework4() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 5, //delay of 20 frames before the attack starts
            _attackDuration: 20, //25 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework4; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 5f + (0.2f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 30,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: -40f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 8f + (0.25f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: -50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        tempDamage.damageAmount = 5f + (0.2f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 5f + (0.2f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.3f, 800f, 0f, -50f, 0f, 12, 0.08f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStatus: status, //here's the status manager
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _attackDelay: 38, //delay of 38 frames before the attack starts
            _attackDuration: 25, //25 frames within which the attack is active
            _attackEnd: 20, //and 20 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        tempDamage.damageAmount = 12f + (0.3f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -1200f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 40,
            _onHitForwardBackward: -1000f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        tempDamage.damageAmount = 15f + (0.35f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 70,
            _causesFloored: 120,
            _onHitForwardBackward: -1200f,
            _onHitRightLeft: -100f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        tempDamage.damageAmount = 10f + (0.3f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _causesFloored: 100,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        tempDamage.damageAmount = 18f + (0.35f * stat.STR) + (0.05f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 70,
            _causesFloored: 120,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        //although the hitbox animation needs to be played for this new attack, the previous attack has already dealt with the gfx animation

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointToTarget(); doesn't need to be called again as the previous 'attack' has already done that
        //similarly, no additional movement needs to be performed

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    // Update is called once per frame
    void Update () {
        if (instantiatedAttacks.Count <= 0) { //if there are no attacks
            status.attackLock = false; //then wahey you aren't attack locked
        }
        else {
            status.attackLock = true; //if there are then you are, kind of makes sense
        }
        if (debug) {
            showHurtBox();
        }
    }

    private void FixedUpdate() {

        //call the attack progression and return to idle functions that have been overridden in this class
        attackProgression(); 
        returnToIdle();
    }
}
