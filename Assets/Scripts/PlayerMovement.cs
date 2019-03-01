using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement {

    //variable declarations

    public PlayerInput playerInput; //going to need some input or else you won't be able to actually control anything
    Vector3 mousePos; //a vector for the current position of the mouse on screen
    Vector3 objectPos; //a vector for the current position of the object on screen
    public Camera cam; //going to need to put the main camera in here, because for some reason unity currently hates using Camera.main
    float a; //the angle at which to rotate the object


    public override void pointToTarget() { //a function that makes the player point toward the cursor
        transform.rotation = Quaternion.Euler(new Vector3(0, playerInput.toMouseAngle, 0)); //rotation happens on the Y axis
    }

    public override void pointTowardTarget(float maxTurn) {
        if (playerInput.mouseDifAngle > maxTurn) { //the angle could be over the maximum, and to the player's right
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y+maxTurn, 0)); //in which case, turn it the maximum amount allowed to the right
        } else if (playerInput.mouseDifAngle < -maxTurn) { //or it could also be over the maximum to the left
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y-maxTurn, 0)); //in which case, turn it the maximum amount to the left
        } else { //if neither of these, then the cursor is within the maximum turning angle
            transform.rotation = Quaternion.Euler(new Vector3(0, playerInput.toMouseAngle, 0)); //then we can rotate directly to the cursor as in pointToCursor()
        }
    }

    // Use this for initialization
    void Start () {
        status = gameObject.GetComponent<StatusManager>(); //get the status manager of the player object
        motor = gameObject.GetComponent<Motor>(); //get the motor of the player object
        playerInput = gameObject.GetComponent<PlayerInput>(); //get the input for the player
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        status.sneaking = false; //set sneaking to false so that it doesn't stay on when you stop sneaking
        status.sprinting = false; //same thing for sprinting
        if (playerInput.movement && status.canMove()) {
            if (status.guarding == true) { //player goes slow if guarding
                if (status.paralyzed > 0) {
                    motor.SetSpeed(creepSpeed / 2);
                }
                else {
                    motor.SetSpeed(creepSpeed);
                }
            }
            else if (playerInput.sprint && status.paralyzed <= 0) { //player sprints to go fast
                motor.SetSpeed(sprintSpeed);
                status.sprinting = true;
            }
            else if (playerInput.cursorDistance < Screen.width * 0.05) { //player creeps if the cursor is really close to the player
                if (status.paralyzed > 0) {
                    motor.SetSpeed(creepSpeed / 2);
                }
                else {
                    motor.SetSpeed(creepSpeed);
                }
                status.sneaking = true; //turn sneaking on if sneaking
            }
            else if (playerInput.cursorDistance < Screen.width * 0.1) { //player walks if the cursor is fairly close to the player
                if (status.paralyzed > 0) {
                    motor.SetSpeed(walkSpeed / 2);
                }
                else {
                    motor.SetSpeed(walkSpeed);
                }
            }
            else { //player jogs at further distances
                if (status.paralyzed > 0) {
                    motor.SetSpeed(jogSpeed / 2);
                } else {
                    motor.SetSpeed(jogSpeed);
                }
            }
            pointToTarget();
            motor.forwardGradual();
            status.moving = true;
        }
        else {
            status.moving = false;
        }

        if (playerInput.evade && status.canRoll()) {
            if (status.sprinting) {
                pointToTarget();
                evade(diveSpeed, 0f, diveTime);
                animator.Play("forwardDive");
            } else {
                pointToTarget();
                evade(rollSpeed, 0f, rollTime);
                animator.Play("forwardRoll");
            } //other evades can be done at other times, but they are handled under the current attack style of the player, and are treated similarly to attacks.

        } 



	}


    void Update() { //I'm using Update here mainly for animation states,
        //because the Unity animator thinks it's too good for using variables in/from scripts and it definitely needs ITS OWN SPECIAL LITTLE VARIABLES OOH LOOK AT ME. 
        //I REQUIRE EVERY STATE BOOL YOU HAVE TO HAVE AN ANIMATOR-SPECIFIC DUPLICATE WAHEY
        //Some animation bits have to go elsewhere in the script, however, so this isn't everything.
        animator.SetFloat("speedPercent", motor.rb.velocity.magnitude / (sprintSpeed / 150), .1f, Time.deltaTime);
        animator.SetBool("sneak", status.sneaking);
    }

}
