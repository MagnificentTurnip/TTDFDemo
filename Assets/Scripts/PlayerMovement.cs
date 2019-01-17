using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    //variable declarations
    StatusManager playerStatus; //going to need access to the player's status
    Motor playerMotor; //also going to need a motor to actually drive the player's movement
    PlayerInput playerInput; //going to need some input or else you won't be able to actually control anything
    public Animator animator; //the player's animator is needed to animate the character because I can't do it in a seperate script because I have grown to hate the unity animator for all of its annoyingness
    Vector3 mousePos; //a vector for the current position of the mouse on screen
    Vector3 objectPos; //a vector for the current position of the object on screen
    public Camera cam; //going to need to put the main camera in here, because for some reason unity currently hates using Camera.main
    float a; //the angle at which to rotate the object
    public float jogSpeed; //various speeds
    public float sprintSpeed;
    public float walkSpeed;
    public float creepSpeed;
    public float rollSpeed;
    public float rollTime;
    public float diveSpeed;
    public float diveTime;
    float speedPercent; //a variable for the animator

    public void pointToCursor() { //a function that makes the attached gameObject point toward the cursor
        transform.rotation = Quaternion.Euler(new Vector3(0, playerInput.toMouseAngle, 0)); //rotation happens on the Y axis
    }

    public void pointTowardCursor(float maxTurn) {
        if (playerInput.mouseDifAngle > maxTurn) { //the angle could be over the maximum, and to the player's right
            transform.rotation = Quaternion.Euler(new Vector3(0, maxTurn, 0)); //in which case, turn it the maximum amount allowed to the right
        } else if (playerInput.mouseDifAngle < -maxTurn) { //or it could also be over the maximum to the left
            transform.rotation = Quaternion.Euler(new Vector3(0, -maxTurn, 0)); //in which case, turn it the maximum amount to the left
        } else { //if neither of these, then the cursor is within the maximum turning angle
            transform.rotation = Quaternion.Euler(new Vector3(0, playerInput.toMouseAngle, 0)); //then we can rotate directly to the cursor as in pointToCursor()
        }
    }

    public void evade(float fbSpeed, float lrSpeed, float evadeTime) {
        playerStatus.rollLock = true;
        playerStatus.rolling = true;
        pointToCursor();
        playerMotor.instantBurst(fbSpeed, lrSpeed);
        StartCoroutine(endroll(evadeTime, evadeTime));
    }

    public IEnumerator endroll(float evadeTime, float delay) {
        yield return new WaitForSeconds(delay);
        playerStatus.rolling = false;
        Invoke("rollLockout", evadeTime);
    }

    public void rollLockout() {
        playerStatus.rollLock = false;
    }

    // Use this for initialization
    void Start () {
        playerStatus = gameObject.GetComponent<StatusManager>(); //get the status manager of the player object
        playerMotor = gameObject.GetComponent<Motor>(); //get the motor of the player object
        playerInput = gameObject.GetComponent<PlayerInput>(); //get the input for the player
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        playerStatus.sneaking = false; //set sneaking to false so that it doesn't stay on when you stop sneaking
        playerStatus.sprinting = false; //same thing for sprinting
        if (playerInput.movement && playerStatus.canMove()) {
            if (playerInput.sprint) { //player sprints to go fast
                playerMotor.SetSpeed(sprintSpeed);
                playerStatus.sprinting = true;
            }
            else if (playerInput.cursorDistance < Screen.width * 0.05) { //player creeps if the cursor is really close to the player
                playerMotor.SetSpeed(creepSpeed);
                playerStatus.sneaking = true; //turn sneaking on if sneaking
            }
            else if (playerInput.cursorDistance < Screen.width * 0.1) { //player walks if the cursor is fairly close to the player
                playerMotor.SetSpeed(walkSpeed);
            }
            else { //player jogs at further distances
                playerMotor.SetSpeed(jogSpeed);
            }
            pointToCursor();
            playerMotor.forwardGradual();
            playerStatus.moving = true;
        }
        else {
            playerStatus.moving = false;
        }

        if (playerInput.evade && playerStatus.canRoll() && playerStatus.canMove()) { //the reason why canMove() is included is because only this specific evade in the specific direction of the cursor can be done while moving
            if (playerStatus.sprinting) {
                evade(diveSpeed, 0f, diveTime);
                animator.Play("forwardDive");
            } else {
                evade(rollSpeed, 0f, rollTime);
                animator.Play("forwardRoll");
            } //other evades can be done at other times, but they are handled under the current attack style of the player, and are treated similarly to attacks.

        } 



	}


    void Update() { //I'm using Update here mainly for animation states,
        //because the Unity animator thinks it's too good for using variables in/from scripts and it definitely needs ITS OWN SPECIAL LITTLE VARIABLES OOH LOOK AT ME. 
        //I REQUIRE EVERY STATE BOOL YOU HAVE TO HAVE AN ANIMATOR-SPECIFIC DUPLICATE WAHEY
        //Some animation bits have to go elsewhere in the script, however, so this isn't everything.
        animator.SetFloat("speedPercent", playerMotor.rb.velocity.magnitude / (sprintSpeed / 150), .1f, Time.deltaTime);
        animator.SetBool("sneak", playerStatus.sneaking);
    }

}
