using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyleWolf : AtkStyle {

    public enum attackStates { idle, fEvade, bEvade, rEvade, lEvade, guarding, spellcast, fParry, bParry, attack};
    public attackStates state;

    public GameObject rgtPawBone; //4 relevant bones to follow to make simple animations easier
    public GameObject lftPawBone;
    public GameObject jawBone;
    public GameObject chest;

    // Use this for initialization
    void Start() {
        state = attackStates.idle;
        instantiatedAttacks = new List<Attack>();
    }

    public override void forceIdle() {
        state = attackStates.idle;
    }

    public override void forceGuarding(int counterIdle) {
        state = attackStates.guarding;
        idleCounter = counterIdle;
    }

    public override void forceSpellcast(int counterIdle) {
        state = attackStates.spellcast;
        idleCounter = counterIdle;
    }

    public override void returnToIdle() {
        //manage returning to idle
        if (state != attackStates.idle) {
            if (state != attackStates.guarding) {
                status.toIdleLock = true;
            }
            idleCounter--;
        }

        if (idleCounter <= 0) {
            idleCounter = 0;
            state = attackStates.idle;
            status.toIdleLock = false;
        }
    }

    public override void bParry() {
        animator.Play("bParry", 0, 0f);
        if (status.sheathed == false) {
            state = attackStates.bParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        stat.MP -= 20;
        StartCoroutine(status.Parry(0.3f, 15, 60, 60));
        movement.motor.instantBurst(-700f, 100f);
    }

    public override void fParry() {
        animator.Play("fParry", 0, 0f);
        if (status.sheathed == false) {
            state = attackStates.fParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        stat.MP -= 20;
        StartCoroutine(status.Parry(0.3f, 15, 60, 60));
        movement.motor.instantBurst(700f, -100f);
    }

    public void quickBite() {
        print("quickbite");
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = jawBone.transform.position;
        currentAttack.transform.parent = jawBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>() as BoxCollider, //this attack uses a box collider
            _GFXAnimation: "quickBite",
            _HitboxAnimation: "bite", //uses the bite hitbox animation
            _attackDelay: 10, //delay of 10 frames before the attack starts
            _attackDuration: 12, //12 frames within which the attack is active
            _attackEnd: 25, //and 25 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.attack; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.2f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.9f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.1f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.9f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.SPdamage;
        currentAttack.onGuard = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _SPcost: 30f,
            _causesGuardStun: 5,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.1f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 0.9f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.1f, 300f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void forwardBite(int unblock) { //this attack takes an int because it is sometimes unblockable
        print("forwardbite");
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = jawBone.transform.position;
        currentAttack.transform.parent = jawBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "forwardBite",
            _HitboxAnimation: "bite", //uses the bite hitbox animation
            _attackDelay: 40, //delay of 40 frames before the attack starts
            _attackDuration: 20, //20 frames within which the attack is active
            _attackEnd: 20, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true, //and makes contact.
            _unblockable: unblock); 

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.attack; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.1f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        if (charStat != null) {
            tempDamage.damageAmount = 40f + (0.1f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 40f + 1.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.SPdamage;
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 30f,
            _causesGuardStun: 5,
            _onHitForwardBackward: -700f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.1f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 0.9f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.5f, 700f, 50f, 0f, 0f, 0, 0f)); //this move moves you a fair bit forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void lSwipe(int unblock) { //this attack takes an int because it is sometimes unblockable
        print("lswipe");

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = lftPawBone.transform.position;
        currentAttack.transform.parent = lftPawBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lSwipe",
            _HitboxAnimation: "swipe", //uses the swipe hitbox animation
            _attackDelay: 40, //delay of 40 frames before the attack starts
            _attackDuration: 20, //20 frames within which the attack is active
            _attackEnd: 35, //and 35 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true, //and makes contact.
            _unblockable: unblock);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.attack; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.3f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 1.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 35,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: -1000f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        if (charStat != null) {
            tempDamage.damageAmount = 30f + (0.3f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 30f + 1.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.SPdamage;
        currentAttack.onGuard = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _SPcost: 30f,
            _causesGuardStun: 10,
            _onHitForwardBackward: -700f,
            _onHitRightLeft: -700f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.3f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _causesFloored: 120,
            _onHitForwardBackward: -1500f,
            _onHitRightLeft: -1800f);

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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.1f, 100f, 700f, 0f, 0f, 0, 0f)); //this move moves you a smidge right

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void rSwipe(int unblock) { //this attack takes an int because it is sometimes unblockable
        print("rswipe");

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtPawBone.transform.position;
        currentAttack.transform.parent = rgtPawBone.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "rSwipe",
            _HitboxAnimation: "swipe", //uses the swipe hitbox animation
            _attackDelay: 40, //delay of 40 frames before the attack starts
            _attackDuration: 20, //20 frames within which the attack is active
            _attackEnd: 35, //and 35 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true, //and makes contact.
            _unblockable: unblock);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.attack; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.3f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 1.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 35,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: 1000f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        if (charStat != null) {
            tempDamage.damageAmount = 30f + (0.3f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 30f + 1.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.SPdamage;
        currentAttack.onGuard = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _SPcost: 30f,
            _causesGuardStun: 10,
            _onHitForwardBackward: -700f,
            _onHitRightLeft: 700f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.3f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _causesFloored: 120,
            _onHitForwardBackward: -1500f,
            _onHitRightLeft: 1800f);

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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(0.6f, 100f, -700f, 0f, 0f, 0, 0f)); //this move moves you a smidge right

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void pounce(int unblock) { //this attack takes an int because it is sometimes unblockable
        print("pounce");

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        //currentAttack.transform.position = jawBone.transform.position;
        //currentAttack.transform.parent = jawBone.transform;
        currentAttack.transform.position = chest.transform.position;
        currentAttack.transform.parent = chest.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "pounce",
            _HitboxAnimation: "fullBody", //use the hitbox animation for full-body attacks
            _attackDelay: 100, //delay of 100 frames before the attack starts
            _attackDuration: 30, //30 frames within which the attack is active
            _attackEnd: 70, //and 70 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true, //and makes contact.
            _unblockable: unblock);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.attack; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 25f + (0.5f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 25f + 3f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Impact;
        currentAttack.onHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesFloored: 180,
            _onHitForwardBackward: -4000f,
            _onHitRightLeft: 0f);

        //add a smidge of slashing damage
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.1f * charStat.STR) + (0.3f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit.damageInstances.Add(tempDamage);
        
        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties


        //set the attack's properties on guard
        if (charStat != null) {
            tempDamage.damageAmount = 100f + (0.5f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 100f + 5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.SPdamage;
        currentAttack.onGuard = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _SPcost: 100f,
            _causesGuardStun: 5,
            _onHitForwardBackward: -1000f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 30f + (0.5f * charStat.STR) + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 30f + 3f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Impact;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 80,
            _causesFloored: 180,
            _onHitForwardBackward: -5000f,
            _onHitRightLeft: 50f);

        //add a smidge of slashing damage
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.2f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit.damageInstances.Add(tempDamage);

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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        //movement.pointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.timedBurst(2f, 5500f, 0f, -4100f, 0f, 2, 0.45f)); //this move moves launches you forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    // Update is called once per frame
    void Update() {
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
        nonSpellAtkStyle();
    }

}
