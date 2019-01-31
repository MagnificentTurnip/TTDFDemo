using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBridgeWish : MonoBehaviour { //bridges between player input and the attacks in the Wish attacking style. Used so that it can be bridged via AI instead for different stuff if need be.

    public PlayerInput playIn;
    public AtkStyleWish wish;
    public StatusManager status;

    public int drawCount;


    // Use this for initialization
    void Start () {
        drawCount = 0;
    }
	
	// Update is called once per frame
	void Update () {
        print(wish.state);
        wish.playerAnimator.SetBool("sheathed", status.sheathed);

        //unsheathing
        if (status.sheathed == true && playIn.button1 && status.canAttack()) {
            if (status.sprinting) { // the player performs an unsheathe attack if sprinting at the point of drawing their weapon
                wish.standardBladework1();
                status.sheathed = false;
            } else {
                wish.playerAnimator.Play("Draw", 1);//play the unsheathing animation
                wish.state = AtkStyleWish.attackStates.drawing;
                drawCount = 10;
                //play unsheathing animation for the body
            }
        }

        if (status.sheathed == false) { //only do attack style stuff when unsheathed
            //evades. Wish style has four directional evades out of attacks.
            if (playIn.evade && status.canRoll() && wish.state != AtkStyleWish.attackStates.idle) {
                if (playIn.pointForward == true) {
                    wish.evadeForward();
                }
                if (playIn.pointBack == true) {
                    wish.evadeBack();
                }
                if (playIn.pointRight == true) {
                    wish.evadeRight();
                }
                if (playIn.pointLeft == true) {
                    wish.evadeLeft();
                } //all of these evades do not alter the direction that the player is facing.
            }

            //attacks
            if (status.canAttack()) {
                
                switch (wish.state) {
                    
                    case AtkStyleWish.attackStates.idle:
                        if (playIn.sheathe) { //from idle you can sheathe again
                            status.sheathed = true;
                            wish.playerAnimator.Play("Sheathe", 1); //play the sheathing animation
                        }
                        if (playIn.fwdA.Check()) {
                            wish.standardBladework1();
                        }
                        if (playIn.fwdS.Check()) {
                            //wish.standardBladework1();
                        }
                        break;

                    case AtkStyleWish.attackStates.fEvade:
                        break;

                    case AtkStyleWish.attackStates.bEvade:
                        break;

                    case AtkStyleWish.attackStates.rEvade:
                        break;

                    case AtkStyleWish.attackStates.lEvade:
                        break;

                    case AtkStyleWish.attackStates.bladework1:
                        if (playIn.fwdA.Check()) {
                            wish.standardBladework2();
                            print("blade2");
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework2:
                        if (playIn.fwdA.Check()) {
                            wish.standardBladework3();
                            print("blade3");
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework3:
                        if (playIn.fwdA.Check()) {
                            wish.standardBladework4();
                            print("blade4");
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework4:
                        break;
                }

            }
            //end of attacks
        }

    }

    void FixedUpdate() { 
        if (drawCount > 0) {
            drawCount--;
        }
        if (wish.state == AtkStyleWish.attackStates.drawing && drawCount <= 0) {
            wish.state = AtkStyleWish.attackStates.idle;
            status.sheathed = false;
        }
    }


}
