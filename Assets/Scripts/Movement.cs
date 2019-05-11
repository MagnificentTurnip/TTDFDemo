﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public StatusManager status; //going to need access to the entity's status
    public StatSheet stat; //going to need access to the stat sheet to modify stamina
    public Motor motor; //also going to need a motor to actually drive movement
    public Animator animator; //the animator is needed to animate the character because I can't do it in a seperate script because I have grown to hate the unity animator for all of its annoyingness

    public AudioSource source;
    public AudioClip footStep;
    public AudioClip roll;

    public float jogSpeed; //various speeds
    public float sprintSpeed;
    public float walkSpeed;
    public float creepSpeed;
    public float rollSpeed;
    public float rollTime;
    public float diveSpeed;
    public float diveTime;
    public float speedPercent; //a variable for the animator

    void Start() {

    }

    void Update() {

    }

    public virtual void PointToTarget() {

    }

    public virtual void PointTowardTarget(float maxTurn) {

    }

    public void Evade(float fbSpeed, float lrSpeed, float evadeTime) {
        stat.SP -= 100;
        status.rollLock = true;
        status.rolling = true;
        status.floored = 0;
        status.channelLock = 0;
        motor.InstantBurst(fbSpeed, lrSpeed);
        StartCoroutine(Endroll(evadeTime, evadeTime));
        source.PlayOneShot(roll, Random.Range(0.1f, 0.2f));
        source.PlayOneShot(footStep, Random.Range(0.2f, 0.4f));
    }

    public IEnumerator Endroll(float evadeTime, float delay) {
        yield return new WaitForSeconds(delay);
        status.rolling = false;
        Invoke("RollLockout", evadeTime);
    }

    public void RollLockout() {
        status.rollLock = false;
    }

}