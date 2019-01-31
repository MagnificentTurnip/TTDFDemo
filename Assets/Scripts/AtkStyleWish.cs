using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyleWish : AtkStyle {

    new public enum attackStates { drawing, idle, fEvade, bEvade, rEvade, lEvade, spellcast, fParry, bParry, bladework1, bladework2, bladework3, bladework4 };

    public bool debug;

    public StatusManager status;
    public CharStatSheet stat;

    public Mesh cube; //for testing hitboxes
    public Mesh sphere; //for testing hitboxes

    private Attack currentAttack;
    public List<Attack> instantiatedAttacks;
    public GameObject attack;
    public Animator playerAnimator;
    public RuntimeAnimatorController hitboxAnimatorController;
    public PlayerMovement movement;

    public Attack.damage tempDamage;

    new public attackStates state;
    public int wraithMode; //wish style has a powered-up state (which I may or may not have time to implement wahey)
    public float evadespeed; //you may want attack-break evades to be faster or slower than regular ones
    public float evadetime;

    private Attack toDestroy; //going to need to remove an object from this list before destroying it

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
        state = attackStates.fEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void evadeBack() {
        movement.evade(-evadespeed, 0f, evadetime);
        playerAnimator.Play("forwardRoll");
        state = attackStates.bEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void evadeRight() {
        movement.evade(0f, evadespeed, evadetime);
        playerAnimator.Play("forwardRoll");
        state = attackStates.rEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void evadeLeft() {
        movement.evade(0f, -evadespeed, evadetime);
        playerAnimator.Play("forwardRoll");
        state = attackStates.lEvade; //set the attack state;
        idleCounter = 30; //always remember to reset the idle counter
    }

    public void standardBladework1() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = this.gameObject.transform.position;
        currentAttack.transform.parent = this.gameObject.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework1",
            _HitboxAnimation: "standardBladework1", //having names for both animations seems fairly self-explanatory
            _attackDelay: 20, //delay of 20 frames before the attack starts
            _attackDuration: 25, //25 frames within which the attack is active
            _attackEnd: 10, //and 10 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
            _hitsAirborne: true, //hits airborne, standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: true); //and makes contact.

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
        movement.pointToCursor(); //this is a pretty accurately directed move so it goes right to the cursor's direction
        StartCoroutine(movement.playerMotor.timedBurst(0.3f, 3000f, 50f, -2000f, 0f, 1, 0.5f)); //with this move, you jump forward and abruptly stop

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 20; //always remember to reset the idle counter
    }

    public void standardBladework2() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = this.gameObject.transform.position;
        currentAttack.transform.parent = this.gameObject.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework2",
            _HitboxAnimation: "standardBladework2", //having names for both animations seems fairly self-explanatory
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
            _causesStun: 35,
            _onHitForwardBackward: -200f,
            _onHitRightLeft: -100f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 8f,
            _causesGuardStun: 30,
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
        playerAnimator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointTowardCursor(20f); //this move doesn't follow the cursor extremely well but it's a wide attack so it doesn't need to that much
        StartCoroutine(movement.playerMotor.timedBurst(0.2f, 400f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 25; //always remember to reset the idle counter
    }

    public void standardBladework3() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = this.gameObject.transform.position;
        currentAttack.transform.parent = this.gameObject.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework3",
            _HitboxAnimation: "standardBladework3", //having names for both animations seems fairly self-explanatory
            _attackDelay: 10, //delay of 10 frames before the attack starts
            _attackDuration: 20, //20 frames within which the attack is active
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
            _causesStun: 40,
            _onHitForwardBackward: -300f,
            _onHitRightLeft: 50f);

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
        tempDamage.damageAmount = 15f + (0.25f * stat.STR) + (0.3f * stat.DEX);
        tempDamage.damageType = Attack.typeOfDamage.Slashing;
        currentAttack.onVulnerableHit = new Attack.hitProperties(
            _damageInstances: new List<Attack.damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 50,
            _causesAirborne: 50,
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
            _causesStun: 30,
            _onHitForwardBackward: -350f,
            _onHitRightLeft: 50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        playerAnimator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointToCursor(); //this is a pretty accurately directed move so it goes right to the cursor's direction
        StartCoroutine(movement.playerMotor.timedBurst(0.2f, 400f, -50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 25; //always remember to reset the idle counter
    }

    public void standardBladework4() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = this.gameObject.transform.position;
        currentAttack.transform.parent = this.gameObject.transform;

        //set the attack data
        currentAttack.data = new Attack.atkData(
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<BoxCollider>(), //this attack uses a box collider
            _GFXAnimation: "standardBladework4",
            _HitboxAnimation: "standardBladework4", //having names for both animations seems fairly self-explanatory
            _attackDelay: 5, //delay of 5 frames before the attack starts
            _attackDuration: 20, //20 frames within which the attack is active
            _attackEnd: 15, //and 15 frames at the end before the attack is considered complete. attackCharge is left at the default of 0 as this attack doesn't charge.
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
            _causesStun: 50,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.hitProperties(
            _SPcost: 10f,
            _causesGuardStun: 50,
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
            _causesStun: 70,
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
            _causesStun: 30,
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
            _causesStun: 80,
            _causesFloored: 120,
            _onHitForwardBackward: -50f,
            _onHitRightLeft: -50f);

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        playerAnimator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
        movement.pointToCursor(); //this is a pretty accurately directed move so it goes right to the cursor's direction
        StartCoroutine(movement.playerMotor.timedBurst(0.1f, 300f, 50f, 0f, 0f, 0, 0f)); //this move moves you a smidge forward

        idleCounter = currentAttack.data.attackDelay + currentAttack.data.attackDuration + currentAttack.data.attackEnd + 30; //always remember to reset the idle counter
    }

    // Update is called once per frame
    void Update () {
        if (instantiatedAttacks.Count <= 0) { //if there are no attacks
            status.attackLock = false; //then wahey you aren't attack locked
        }
        else {
            status.attackLock = true; //if there are then you are, kind of makes sense
        }
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
            if (instantiatedAttacks[i].data.attackEnd <= 0 && instantiatedAttacks[i].data.attackDuration <= 0 && instantiatedAttacks[i].data.attackDelay <= 0) {
                toDestroy = instantiatedAttacks[i];
                instantiatedAttacks.Remove(instantiatedAttacks[i]);
                Destroy(toDestroy.gameObject);
            }
        }



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
}
