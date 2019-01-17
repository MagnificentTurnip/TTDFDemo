﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour {

    public Rigidbody rb;
    float speed;

    public void SetSpeed(float inSpeed) { //sets the speed of the motor, self-explanatory
        speed = inSpeed;
    }

    public void forwardGradual() { //gradual force, the kind of thing you put down to happen every frame while something's true or whatever.
        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime);
    }

    public void instantBurst(float forwardBack, float rightLeft) { //
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(transform.forward * forwardBack);
        rb.AddForce(transform.right * rightLeft);
    }

    public IEnumerator timedBurst(float timeBeforeStart, float initialFB, float initialRL, float incrementFB, float incrementRL, int totalKeyframes, float keyframeInterval) {
        yield return new WaitForSeconds(timeBeforeStart);
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(transform.forward * initialFB);
        rb.AddForce(transform.right * initialRL);
        yield return new WaitForSeconds(keyframeInterval);
        if (totalKeyframes > 1) {
            StartCoroutine(timedBurst(0f, initialFB + incrementFB, initialRL + incrementRL, incrementFB, incrementRL, totalKeyframes - 1, keyframeInterval));
        }
    }

    /*
    public void leftBurst() {
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(transform.right * -speed);
    }

    public void backBurst() {
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(transform.right * -speed);
    }
    */

    // Use this for initialization
    void Start () {
        speed = 0f;
	}

}
