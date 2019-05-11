using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyleWish : AtkStyle {

    public enum attackStates { drawing, idle, fEvade, bEvade, rEvade, lEvade, guarding, spellcast, fParry, bParry, bladework1, bladework2, bladework3, bladework4, overhead }; 
    public attackStates state;

    public GameObject rgtHndBone; //just a couple of relevant bones for the sake of making sonme simple animations easier
    public GameObject lftHndBone;

    public int wraithMode; //wish style has a powered-up state (which I may or may not have time to implement wahey)
    public int bladeTracker; //wish style usually uses multiple different weapon types. 0 for regular blade, 1 for big blade, 2 for dual blades.
    public float evadespeed; //you may want attack-break evades to be faster or slower than regular ones
    public float evadetime;

    public AudioSource source;

    public AudioClip drawClip;
    public AudioClip sheathClip;
    public AudioClip morphClip;
    public AudioClip fParryClip;
    public AudioClip bParryClip;

    public AudioClip advancingBladeworkClip1;
    public AudioClip advancingBladeworkClip2;
    public AudioClip standardBladework1Clip;
    public AudioClip standardBladework2Clip;
    public AudioClip standardBladework3Clip;
    public AudioClip standardBladework4Clip;
    public AudioClip lightBladework1Clip;
    public AudioClip lightBladework2Clip;
    public AudioClip lightBladework3Clip1;
    public AudioClip lightBladework3Clip2;
    public AudioClip lightBladework4Clip1;
    public AudioClip lightBladework4Clip2;
    public AudioClip heavyBladework1Clip;
    public AudioClip heavyBladework2Clip1;
    public AudioClip heavyBladework2Clip2;
    public AudioClip heavyBladework3Clip;
    public AudioClip heavyBladework4Clip1;
    public AudioClip heavyBladework4Clip2;
    public AudioClip overheadClip;

    // Use this for initialization
    void Start () {
        state = attackStates.idle;
        instantiatedAttacks = new List<Attack>();
	}

    public override void ForceIdle() { //forces the idle state
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        state = attackStates.idle;
    }

    public override void ForceGuarding(int counterIdle) { //forces a guarding state
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        state = attackStates.guarding;
        idleCounter = counterIdle;
    }

    public override void ForceSpellcast(int counterIdle) { //forces the spellcast state
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        state = attackStates.spellcast;
        idleCounter = counterIdle;
    }

    public override void ReturnToIdle() { //manages returning to idle naturally
        if (state != attackStates.idle) {
            if (state != attackStates.drawing && state != attackStates.guarding) {
                status.toIdleLock = true;
            }
            idleCounter--;
        }

        if (idleCounter <= 0 && state != attackStates.drawing) {
            if (bladeTracker == 1) {
                animator.Play("GreatBladeOff", 1, 0f);
            }
            if (bladeTracker == 2) {
                animator.Play("SecBladeOff", 1, 0f);
            }
            bladeTracker = 0;
            idleCounter = 0;
            state = attackStates.idle;
            status.toIdleLock = false;
        }
    }

    public override void BParry() { //backwards parry; a more defensive parry
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        movement.PointToTarget();
        animator.Play("bParry");
        if (status.sheathed == false) {
            state = attackStates.bParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        stat.MP -= 20;
        StartCoroutine(status.Parry(0f, 25, 60, 35));
        movement.motor.InstantBurst(-100f, 50f);
    }

    public override void FParry() { //forwards parry; a more offensive parry
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        movement.PointToTarget();
        animator.Play("fParry");
        if (status.sheathed == false) {
            state = attackStates.fParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        stat.MP -= 20;
        StartCoroutine(status.Parry(0.1f, 15, 60, 70));
        movement.motor.InstantBurst(700f, -100f);
    }

    public void DefaultAttack() { //an unused default attack
        currentAttack = Instantiate(attack).GetComponent<Attack>();
    }

    public void EvadeForward() {
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        movement.Evade(evadespeed, 0f, evadetime);
        animator.Play("fRoll");
        state = attackStates.fEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void EvadeBack() {
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        movement.Evade(-evadespeed, 0f, evadetime);
        animator.Play("bRoll", -1, 0.3f);
        state = attackStates.bEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void EvadeRight() {
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        movement.Evade(0f, evadespeed, evadetime);
        animator.Play("rRoll");
        state = attackStates.rEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void EvadeLeft() {
        if (bladeTracker == 1) {
            animator.Play("GreatBladeOff", 1, 0f);
        }
        if (bladeTracker == 2) {
            animator.Play("SecBladeOff", 1, 0f);
        }
        bladeTracker = 0;
        movement.Evade(0f, -evadespeed, evadetime);
        animator.Play("lRoll");
        state = attackStates.lEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    //IDEA FOR KNOCKBACK. Instead of just forwardback and leftright, include awaytowards force too. Awaytowards is the current implementation. forwardback and leftright are currentAttack.data.attackOwnerStatus.gameObject.transform.(forward or right) * force respectively.



    public void StandardBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = standardBladework1Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework1",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
            _attackDelay: 8, //delay of 35 frames before the attack starts
            _attackDuration: 17, //20 frames within which the attack is active
            _attackEnd: 5, //and 5 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true, //makes contact.
            _unblockable: 0); //and isn't unblockable (this is the default, so it doesn't need to be here but I like keeping it here as an example if I want to change it)

        if (debug == true){
        currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.8f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -300f, 
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost : 10f, 
            _causesGuardStun: 20, 
            _onHitForwardBackward: -450f, 
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.15f * charStat.STR) + (0.15f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.6f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 15,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.8f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 100,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //add an extra bit of impact damage
        if (charStat != null) {
            tempDamage.damageAmount = 2f + (0.05f * charStat.STR);
        }
        else {
            tempDamage.damageAmount = 2f + 0.1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Impact;
        currentAttack.onAirborneHit.damageInstances.Add(tempDamage);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointToTarget(); //this move takes perfect directional input, meaning you can even direct it outside of the space within which you can input the command for it
        
        StartCoroutine(movement.motor.TimedBurst(0.15f, 300f, 50f, 0f, 0f, 0, 0f)); //with this move you move forward a tiny bit

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void StandardBladework2() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = standardBladework2Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework2; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.1f * charStat.DEX);
        } else {
            tempDamage.damageAmount = 8f + 0.8f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 8f,
            _causesGuardStun: 5,
            _onHitForwardBackward: -1000f,
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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(20f); //this move doesn't follow the cursor extremely well but it's a wide attack so it doesn't need to that much
        StartCoroutine(movement.motor.TimedBurst(0.2f, 400f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 25; //always remember to reset the idle counter
    }

    public void StandardBladework3() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position; //this attack uses the right hand bone's default movement
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = standardBladework3Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework3; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.2f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 25,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 20,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.25f * charStat.STR) + (0.3f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _causesAirborne: 40,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't hit floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.15f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.8f*stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 15,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.2f, 400f, -50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 25; //always remember to reset the idle counter
    }

    public void StandardBladework4() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position; //this attack uses the right hand bone's default movement
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = standardBladework4Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
            _attackDelay: 5, //delay of 5 frames before the attack starts
            _attackDuration: 15, //15 frames within which the attack is active
            _attackEnd: 15, //and 15 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true); //and makes contact.

        if (debug == true){
        currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework4; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.1f * charStat.STR) + (0.35f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.9f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 16,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.1f * charStat.STR) + (0.35f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesFloored: 120,
            _onHitForwardBackward: -20f,
            _onHitRightLeft: -30f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.1f * charStat.STR) + (0.30f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.8f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 15,
            _onHitForwardBackward: -30f,
            _onHitRightLeft: -30f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.1f * charStat.STR) + (0.35f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesFloored: 120,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.1f, 300f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 30; //always remember to reset the idle counter
    }

    public void LightBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = lightBladework1Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework1",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
            _attackDelay: 7, //delay of 7 frames before the attack starts
            _attackDuration: 15, //15 frames within which the attack is active
            _attackEnd: 25, //and 25 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.1f * charStat.STR) + (0.4f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.9f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 30f,
            _causesGuardStun: 10,
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
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _onHitForwardBackward: 500f,
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
        movement.PointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.4f, 500f, -100f, -2000f, 100f, 2, 0.1f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void LightBladework2() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = lightBladework2Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
            _attackDelay: 7, //delay of 7 frames before the attack starts
            _attackDuration: 13, //13 frames within which the attack is active
            _attackEnd: 25, //and 25 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: false, //hits standing only.
            _hitsStanding: true,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework2; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 4f + (0.1f * charStat.STR) + (0.35f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 4f + 0.8f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 30f,
            _causesGuardStun: 20,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.1f * charStat.STR) + (0.35f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 0.9f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.3f, 500f, 100f, -2000f, -100f, 2, 0.15f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void LightBladework3() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = lightBladework3Clip1;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework3; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.05f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 3f + 0.5f*stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -150f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.2f, 300f, 0f, 300f, 0f, 2, 0.4f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = lightBladework3Clip2;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 3f + 0.3f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 14,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 150f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 7,
            _onHitForwardBackward: -150f,
            _onHitRightLeft: 150f);

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

    public void LightBladework4() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = lightBladework4Clip1;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework4; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 3f + 0.3f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
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
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.2f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 3f + 0.3f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.2f, 600f, 0f, -2500f, 0f, 2, 0.6f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = lightBladework4Clip2;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "lightBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 4f + (0.05f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 4f + 0.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _onHitForwardBackward: -100f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
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

    public void HeavyBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = heavyBladework1Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework1",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.45f * charStat.STR) + (0.15f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 30,
            _onHitForwardBackward: -250f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 20f + (0.5f * charStat.STR) + (0.15f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 20f + 1.7f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
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
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.35f * charStat.STR) + (0.15f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: 200f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointToTarget(); //this move takes perfect directional input, meaning you can even direct it outside of the space within which you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.1f, -100f, -30f, 50f, 10f, 12, 0.05f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void HeavyBladework2() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = heavyBladework2Clip1;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework2; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 16,
            _onHitForwardBackward: -800f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 40,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 40,
            _onHitForwardBackward: -800f,
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
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(15f); //this move allows only 15 degrees of rotational adjustment
        StartCoroutine(movement.motor.TimedBurst(0.1f, 300f, 0f, 8f, 0f, 16, 0.05f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = heavyBladework2Clip2;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework2",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.2f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -800f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 15,
            _onHitForwardBackward: -900f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 14f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 14f + 0.8f * stat.Level;
        }
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

    public void HeavyBladework3() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = heavyBladework3Clip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework3",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework3; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.55f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.6f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 60,
            _causesAirborne: 40,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 17,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 20f + (0.6f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 20f + 2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
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
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.4f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _causesAirborne: 30,
            _onHitForwardBackward: -600f,
            _onHitRightLeft: 200f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(25f); //this move allows 25 degrees of directional adjustment
        StartCoroutine(movement.motor.TimedBurst(0.7f, 400f, -50f, 0f, 0f, 0, 0f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void HeavyBladework4() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = heavyBladework4Clip1;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework4; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.2f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 10,
            _onHitForwardBackward: -600f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 30,
            _onHitForwardBackward: -700f,
            _onHitRightLeft: -40f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.6f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _onHitForwardBackward: -600f,
            _onHitRightLeft: -50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.2f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.2f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 5f + 0.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -600f,
            _onHitRightLeft: -50f);

        //add an extra bit of impact damage
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.05f * charStat.STR);
        }
        else {
            tempDamage.damageAmount = 3f + 0.1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Impact;
        currentAttack.onAirborneHit.damageInstances.Add(tempDamage);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointTowardTarget(45f); //this move allows you to adjust rotation within the space where you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.3f, 800f, 0f, -50f, 0f, 12, 0.08f)); //with this move, you jump forward and abruptly stop


        //THIS ATTACK HAS A SECOND ATTACK SLIGHTLY AFTER THE FIRST TAKE NOTE OF IT ----------------------------------------------------------------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = heavyBladework4Clip2;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "heavyBladework4",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
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

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.3f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 0.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -2500f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 40,
            _onHitForwardBackward: -1500f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.35f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 0.6f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 70,
            _causesFloored: 120,
            _onHitForwardBackward: -3000f,
            _onHitRightLeft: -100f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.3f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 0.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _causesFloored: 100,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit

        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.35f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 0.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 70,
            _causesFloored: 120,
            _onHitForwardBackward: -1000f,
            _onHitRightLeft: -50f);

        //add an extra bit of impact damage
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.05f * charStat.STR);
        }
        else {
            tempDamage.damageAmount = 3f + 0.1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Impact;
        currentAttack.onAirborneHit.damageInstances.Add(tempDamage);

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

    public void BackwardBladework() {
        print("test");
        StartCoroutine(movement.motor.TimedBurst(0f, -300f, -100f, 0f, 0f, 0, 0f)); //move back before you move forwards
        transform.Rotate(transform.up, 180);
        StandardBladework1();
        animator.Play("backwardBladework", 0, 0f); //this move is basically StandardBladework1 but with some minor edits made
    }

    public void AdvancingBladework() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = advancingBladeworkClip1;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "advancingBladework",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
            _attackDelay: 35, //delay of 35 frames before the attack starts
            _attackDuration: 17, //17 frames within which the attack is active
            _attackEnd: 5, //and 5 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true, //makes contact.
            _unblockable: 0); //and isn't unblockable (this is the default, so it doesn't need to be here but I like keeping it here as an example if I want to change it)

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.bladework1; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 10f + 1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 20,
            _onHitForwardBackward: -600f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 20,
            _onHitForwardBackward: -650f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 14f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 14f + 1.4f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -550f,
            _onHitRightLeft: 50f);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 8f + 0.8f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 15,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        if (charStat != null) {
            tempDamage.damageAmount = 12f + (0.25f * charStat.STR) + (0.25f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 12f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 100,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointToTarget(); //this move takes perfect directional input, meaning you can even direct it outside of the space within which you can input the command for it

        StartCoroutine(movement.motor.TimedBurst(0f, 0f, 0f, 3000f, 50f, 2, 0.3f)); //with this move, you jump forward


        //THIS ATTACK HAS A SECOND ATTACK, TECHNICALLY BEFORE THE FIRST BUT IT ISN'T THE MAIN PORTION AND EXISTS MAINLY FOR PREVENTING GUARD-CANCELLATION ---------------------------

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = advancingBladeworkClip2;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "advancingBladework",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.34f, //match the size of the blade, more or less
            _yScale: 0.34f,
            _zScale: 1.23f,
            _SPcost: 50f,
            _attackDelay: 14, //delay of 14 frames before the attack starts
            _attackDuration: 21, //21 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete even though it really doesn't matter in this case. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne only.
            _hitsStanding: false,
            _hitsFloored: false,
            _contact: true); //and makes contact.

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        //the attack state has already been set

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.1f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 3f + 0.1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Piercing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 5,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 10f,
            _causesGuardStun: 7,
            _onHitForwardBackward: -150f,
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
        currentAttack.onAirborneHit = currentAttack.onHit; //This attack only hits airborne targets, so just copy onHit but onHit itself will rarely if ever fire

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

    public void Overhead() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = rgtHndBone.transform.position;
        currentAttack.transform.parent = rgtHndBone.transform;
        currentAttack.clip = overheadClip;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "overhead",
            _HitboxAnimation: "rgtHndMatch", //just uses the default animation that matches the hitbox to the right hand bone
            _xScale: 0.4f, //match the size of the blade, more or less
            _yScale: 0.4f,
            _zScale: 1.5f,
            _MPcost: 15f,
            _attackDelay: 35, //delay of 35 frames before the attack starts
            _attackDuration: 22, //22 frames within which the attack is active
            _attackEnd: 20, //and 20 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits all.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true, //makes contact.
            _unblockable: 1); //and cannot be guarded (but can be parried or magically guarded)

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = cube; //for testing the hitbox
        }

        if (wraithMode > 0) {
            currentAttack.data.xScale += 0.02f;
            currentAttack.data.yScale += 0.02f;
            currentAttack.data.zScale += 0.3f;
        }

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics
        state = attackStates.overhead; //set the attack state;

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 14f + (0.5f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 14f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -3000f,
            _onHitRightLeft: 500f);

        //additional magic damage
        if (charStat != null) {
            tempDamage.damageAmount = 4f + (0.05f * charStat.STR) + (0.2f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 4f + 0.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Magic;
        currentAttack.onHit.damageInstances.Add(tempDamage);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _SPcost: 100f,
            _causesGuardStun: 30,
            _onHitForwardBackward: -2000f,
            _onHitRightLeft: 400f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        if (charStat != null) {
            tempDamage.damageAmount = 18f + (0.5f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 18f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 70,
            _causesFloored: 140,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: 100f);

        //additional magic damage
        if (charStat != null) {
            tempDamage.damageAmount = 8f + (0.05f * charStat.STR) + (0.3f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 8f + 0.3f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Magic;
        currentAttack.onVulnerableHit.damageInstances.Add(tempDamage);

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't hit floored targets so just give it something to have

        //set the attack's properties on floored hit
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.5f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onFlooredHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 30,
            _causesFloored: 120,
            _onHitForwardBackward: -400f,
            _onHitRightLeft: 50f);

        //additional magic damage
        if (charStat != null) {
            tempDamage.damageAmount = 5f + (0.05f * charStat.STR) + (0.2f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 5f + 0.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Magic;
        currentAttack.onFlooredHit.damageInstances.Add(tempDamage);

        //set the attack's properties on airborne hit
        if (charStat != null) {
            tempDamage.damageAmount = 15f + (0.5f * charStat.STR) + (0.05f * charStat.DEX);
        }
        else {
            tempDamage.damageAmount = 15f + 1.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onAirborneHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesFloored: 120,
            _onHitForwardBackward: -500f,
            _onHitRightLeft: 200f);

        //additional magic damage
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.05f * charStat.STR) + (0.2f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 3f + 0.2f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Magic;
        currentAttack.onAirborneHit.damageInstances.Add(tempDamage);

        //add an extra bit of impact damage
        if (charStat != null) {
            tempDamage.damageAmount = 3f + (0.05f * charStat.STR);
        }
        else {
            tempDamage.damageAmount = 3f + 0.1f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Impact;
        currentAttack.onAirborneHit.damageInstances.Add(tempDamage);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        animator.Play(currentAttack.data.GFXAnimation, 0, 0f);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.PointToTarget(); //this move takes perfect directional input, meaning you can even direct it outside of the space within which you can input the command for it
        StartCoroutine(movement.motor.TimedBurst(0.1f, -100f, -30f, 50f, 10f, 16, 0.05f)); //with this move, you move forward slowly over the course of a little while

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
            ShowHurtBox();
        }
    }

    private void FixedUpdate() {

        //call the attack progression and return to idle functions that have been overridden in this class
        AttackProgression(); 
        ReturnToIdle();
        NonSpellAtkStyle();
    }
}
