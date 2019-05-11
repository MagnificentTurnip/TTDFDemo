using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour {

    public Rigidbody rb;
    float speed;
    public bool timeOut;

    public void SetSpeed(float inSpeed) { //sets the speed of the motor, self-explanatory
        speed = inSpeed;
    }

    public void ForwardGradual() { //gradual force, the kind of thing you put down to happen every frame while something's true or whatever.
        rb.AddForce(transform.forward * speed * Time.fixedDeltaTime);
    }

    public void InstantBurst(float forwardBack, float rightLeft) {
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(transform.forward * forwardBack);
        rb.AddForce(transform.right * rightLeft);
    }

    public IEnumerator TimedBurst(float timeBeforeStart, float initialFB, float initialRL, float incrementFB, float incrementRL, int totalKeyframes, float keyframeInterval) {
        timeOut = false;
        yield return new WaitForSeconds(timeBeforeStart);
        if (timeOut == false) {
            rb.velocity = new Vector3(0, 0, 0);
            rb.AddForce(transform.forward * initialFB);
            rb.AddForce(transform.right * initialRL);
            yield return new WaitForSeconds(keyframeInterval);
            if (timeOut == false) {
                if (totalKeyframes > 1) {
                    StartCoroutine(TimedBurst(0f, initialFB + incrementFB, initialRL + incrementRL, incrementFB, incrementRL, totalKeyframes - 1, keyframeInterval));
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        speed = 0f;
	}

}
