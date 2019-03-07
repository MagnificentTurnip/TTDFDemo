using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyleWolf : AtkStyle {

    public enum attackStates { idle, fEvade, bEvade, rEvade, lEvade, guarding, spellcast, fParry, bParry, attack};
    public attackStates state;

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
        animator.Play("bParry");
        if (status.sheathed == false) {
            state = attackStates.bParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        status.parryLock = 60;
        status.parryFrames = 15;
        movement.motor.instantBurst(-700f, 100f);
    }

    public override void fParry() {
        animator.Play("fParry");
        if (status.sheathed == false) {
            state = attackStates.fParry; //set the attack state;
            idleCounter = 60; //always remember to reset the idle counter
        }
        status.parryLock = 60;
        status.parryFrames = 15;
        movement.motor.instantBurst(700f, -100f);
    }

    public void quickBite() {

    }

    public void forwardBite() {

    }

    public void lSwipe() {

    }

    public void rSwipe() {

    }

    public void pounce() {

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
    }

}
