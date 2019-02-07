using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBridgeWish : PlayerBridge { //bridges between player input and the attacks in the Wish attacking style. Used so that it can be bridged via AI instead for different stuff if need be.

    new public AtkStyleWish style;

    // Use this for initialization
    void Start () {
        drawCount = 0;
    }
	
	// Update is called once per frame
	void Update () {
        style.animator.SetBool("sheathed", status.sheathed);

        //handling defences (see PlayerBridge)
        handleDefences();

        //unsheathing
        if (status.sheathed == true && playIn.button1 && status.canAttack()) {
            if (status.sprinting) { // the player performs an unsheathe attack if sprinting at the point of drawing their weapon
                style.standardBladework1();
                status.sheathed = false;
            } else {
                style.state = AtkStyleWish.attackStates.drawing;
                drawCount = 10;
            }
            style.animator.Play("MainBladeOn", 1);//play the unsheathing animation
        }

        if (status.sheathed == false) { //only do attack style stuff when unsheathed
            //evades. Wish style has four directional evades out of attacks.
            if ((lck == buffer.evade || playIn.evade) && status.canRoll() && style.state != AtkStyleWish.attackStates.idle) {
                if (playIn.pointForward == true) {
                    style.evadeForward();
                }
                if (playIn.pointBack == true) {
                    style.evadeBack();
                }
                if (playIn.pointRight == true) {
                    style.evadeRight();
                }
                if (playIn.pointLeft == true) {
                    style.evadeLeft();
                } //all of these evades do not alter the direction that the player is facing.
            }

            //attacks
            if (status.canAttack()) {
                
                switch (style.state) {
                    
                    case AtkStyleWish.attackStates.idle:
                        if (playIn.sheathe) { //from idle you can sheathe again
                            status.sheathed = true;
                            style.animator.Play("MainBladeOff", 1); //play the sheathing animation
                        }
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework1();
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework1();
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework1();
                        }
                        break;

                    case AtkStyleWish.attackStates.fEvade:
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework3();
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework3();
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework3();
                        }
                        break;

                    case AtkStyleWish.attackStates.bEvade:
                        break;

                    case AtkStyleWish.attackStates.rEvade:
                        break;

                    case AtkStyleWish.attackStates.lEvade:
                        break;

                    case AtkStyleWish.attackStates.bladework1:
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework2();
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework2();
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework2();
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework2:
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework3();
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework3();
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework3();
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework3:
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework4();
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework4();
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework4();
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
        if (style.state == AtkStyleWish.attackStates.drawing && drawCount <= 0) {
            style.state = AtkStyleWish.attackStates.idle;
            status.sheathed = false;
        }

        if (status.sheathed == false && drawCount <= 0) {
            cmdBuffer();
        }

        //print(style.state);
        lckBuffer();
    }


}
