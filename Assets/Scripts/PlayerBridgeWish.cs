using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBridgeWish : MonoBehaviour { //bridges between player input and the attacks in the Wish attacking style. Used so that it can be bridged via AI instead for different stuff if need be.

    public PlayerInput playIn;
    public AtkStyleWish wish;
    public StatusManager status;




    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        wish.playerAnimator.SetBool("sheathed", status.sheathed);

        //unsheathing
        if (status.sheathed == true && playIn.button1 && status.canAttack()) {
            if (status.sprinting) { // the player performs an unsheathe attack if sprinting at the point of drawing their weapon
                wish.standardBladework1();
            } else {
                //play unsheathing animation for the body
            }
            //play unsheathing animation for the blade
            status.sheathed = false;
            
        }

        if (status.sheathed == false) { //only do attack style stuff when unsheathed
            //evades. Wish style has four directional evades out of attacks.
            if (playIn.evade && status.canRoll() && !status.canMove()) {
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
                            //play the sheathing animation
                        }
                        if (playIn.fwdA.Check()) {
                            wish.standardBladework1();
                        }
                        if (playIn.fwdS.Check()) {
                            wish.standardBladework1();
                        }
                        break;

                    case AtkStyleWish.attackStates.fEvade:
                        break;
                }

            }
            //end of attacks
        }

    }




}
