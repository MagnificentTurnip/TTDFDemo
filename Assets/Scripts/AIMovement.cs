using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : Movement {

    public GameObject target;
    public float toTargetAngle;
    public float targetDifAngle;

	// Use this for initialization
	void Start () {
		
	}

    public override void pointToTarget() {
        transform.LookAt(target.transform);
    }

    public override void pointTowardTarget(float maxTurn) {
        toTargetAngle = Vector3.SignedAngle(transform.forward, target.transform.position - transform.position, Vector3.up);
        
        if (toTargetAngle > maxTurn) { //the angle could be over the maximum, and to the unit's right
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y + maxTurn, 0)); //in which case, turn it the maximum amount allowed to the right
        }
        else if (toTargetAngle < -maxTurn) { //or it could also be over the maximum to the left
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y - maxTurn, 0)); //in which case, turn it the maximum amount to the left
        }
        else { //if neither of these, then the target is within the maximum turning angle
            pointToTarget();
        }
    }

    public  void pointAwayFromTarget(float maxTurn) {
        toTargetAngle = Vector3.SignedAngle(transform.forward, target.transform.position - transform.position, Vector3.up);

        if (toTargetAngle >= 0 && toTargetAngle < (180 - maxTurn)) { //the target might be to the right
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y - maxTurn, 0)); //in which case, turn it the maximum amount allowed to the left
        }
        else if (toTargetAngle < 0 && toTargetAngle < (-180 + maxTurn)) { //or it could also be to the left
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y + maxTurn, 0)); //in which case, turn it the maximum amount to the right
        }
        else if (toTargetAngle > 0) { //but if it's already almost facing completely away then we don't need to turn it the maximum amount
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y - (180 + toTargetAngle), 0)); //if it's to the right, just give it a small nudge to make it face completely away
        }
        else {
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y + (180 - toTargetAngle), 0)); //and vice-versa to the left
        }
    }

    // Update is called once per frame
    void Update () {
        
    }

    // FixedUpdate is called once per logical step;
    private void FixedUpdate() {

    }
}
