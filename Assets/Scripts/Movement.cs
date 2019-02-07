using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public StatusManager status; //going to need access to the player's status
    public Motor motor; //also going to need a motor to actually drive the player's movement
    public Animator animator; //the animator is needed to animate the character because I can't do it in a seperate script because I have grown to hate the unity animator for all of its annoyingness

    public float jogSpeed; //various speeds
    public float sprintSpeed;
    public float walkSpeed;
    public float creepSpeed;
    public float rollSpeed;
    public float rollTime;
    public float diveSpeed;
    public float diveTime;
    float speedPercent; //a variable for the animator

    void Start() {

    }

    void Update() {

    }

    public virtual void pointToTarget() {

    }

    public virtual void pointTowardTarget(float maxTurn) {

    }

    public void evade(float fbSpeed, float lrSpeed, float evadeTime) {
        status.rollLock = true;
        status.rolling = true;
        motor.instantBurst(fbSpeed, lrSpeed);
        StartCoroutine(endroll(evadeTime, evadeTime));
    }

    public IEnumerator endroll(float evadeTime, float delay) {
        yield return new WaitForSeconds(delay);
        status.rolling = false;
        Invoke("rollLockout", evadeTime);
        print("resetroll");
    }

    public void rollLockout() {
        status.rollLock = false;
    }

}