using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBridgeWish : PlayerBridge { //bridges between player input and the attacks in the Wish attacking style. Used so that it can be bridged via AI instead for different stuff if need be.

    public AtkStyleWish style;
    public override AtkStyle Style {
        get {
            return style;
        }
        set {
            throw new System.NotImplementedException();
        }
    }

    // Use this for initialization
    void Start () {
        drawCount = 0;
        style.bladeTracker = 0;
    }
	
	// Update is called once per frame
	void Update () {
        style.animator.SetBool("sheathed", status.sheathed);

        //handling defences (see PlayerBridge)
        handleDefences();

        //unsheathing
        if (status.sheathed == true && playIn.button1 && status.canAttack()) {
            if (status.sprinting) { // the player performs an unsheathe attack if sprinting at the point of drawing their weapon
                style.advancingBladework();
                style.bladeTracker = 0;
                status.sheathed = false;
                style.source.PlayOneShot(style.drawClip, 0.8f);
            } else {
                style.state = AtkStyleWish.attackStates.drawing;
                drawCount = 10;
            }
            style.animator.Play("MainBladeOn", 1, 0f);//play the unsheathing animation
            style.bladeTracker = 0;
        }

        if (status.sheathed == false) { //only do attack style stuff when unsheathed
            //evades. Wish style has four directional evades out of attacks.
            if ((lck == buffer.evade || playIn.evade) && status.canRoll() && style.state != AtkStyleWish.attackStates.idle && style.state != AtkStyleWish.attackStates.fEvade && style.state != AtkStyleWish.attackStates.bEvade && style.state != AtkStyleWish.attackStates.lEvade && style.state != AtkStyleWish.attackStates.rEvade) {
                if (playIn.pointForward == true) {
                    print(style.state);
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

            //manage regular movement rolling transitioning momentarily into the fEvade state
            if ((lck == buffer.evade || playIn.evade) && status.canRoll() && !status.sprinting && style.state == AtkStyleWish.attackStates.idle && style.state != AtkStyleWish.attackStates.fEvade && style.state != AtkStyleWish.attackStates.bEvade && style.state != AtkStyleWish.attackStates.lEvade && style.state != AtkStyleWish.attackStates.rEvade) {
                style.state = AtkStyleWish.attackStates.fEvade; //set the attack state;
                style.idleCounter = 30; //always remember to reset the idle counter
            }

            //attacks
            if (status.canAttack()) {
                switch (style.state) {
                    
                    case AtkStyleWish.attackStates.idle:
                        if (playIn.sheathe) { //from idle you can sheathe again
                            status.sheathed = true;
                            style.animator.Play("MainBladeOff", 1, 0f); //play the sheathing animation
                            style.bladeTracker = 0;
                            style.source.PlayOneShot(style.sheathClip, 1f);
                        }
                        if (status.sprinting && (cmdOut == buffer.fwdA || lck == buffer.fwdA)) {
                            style.advancingBladework();
                            style.bladeTracker = 0;
                        } else
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework1();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.bakA || lck == buffer.bakA) {
                            style.backwardBladework();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework1();
                            style.bladeTracker = 0;
                        }
                        if (status.sprinting && (cmdOut == buffer.fwdD || lck == buffer.fwdD || cmdOut == buffer.fwdAD || lck == buffer.fwdAD)) {
                            style.overhead();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 2) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                                style.animator.Play("SecBladeOff", 1, 0f);
                            }
                            style.bladeTracker = 1;
                        }
                        else
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework1();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.attackStates.fEvade:
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework3();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.attackStates.bEvade:
                        break;

                    case AtkStyleWish.attackStates.rEvade:
                        break;

                    case AtkStyleWish.attackStates.lEvade:
                        break;

                    case AtkStyleWish.attackStates.fParry:
                        if (cmdOut == buffer.fwdAS || lck == buffer.fwdAS) {
                            style.advancingBladework();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework3();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.attackStates.bParry:
                        if (cmdOut == buffer.fwdAD || lck == buffer.fwdAD) {
                            style.overhead();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 2) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                                style.animator.Play("SecBladeOff", 1, 0f);
                            }
                            style.bladeTracker = 1;
                        }
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework2();
                            style.bladeTracker = 0;
                        }
                        //if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                        //    style.lightBladework3();
                        //}
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework2();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework1:
                        if (cmdOut == buffer.fwdAD || lck == buffer.fwdAD) {
                            style.overhead();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 2) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                                style.animator.Play("SecBladeOff", 1, 0f);
                            }
                            style.bladeTracker = 1;
                        }
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework2();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework2();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework2();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework2:
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework3();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework3:
                        if (cmdOut == buffer.fwdA || lck == buffer.fwdA) {
                            style.standardBladework4();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdS || lck == buffer.fwdS) {
                            style.lightBladework4();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == buffer.fwdD || lck == buffer.fwdD) {
                            style.heavyBladework4();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.attackStates.bladework4:
                        break;
                }

            }
            //end of attacks
        }

        //tooltip things 
        unsheathedTooltip.ClearElements();
        if (status.sheathed) {
            sheathedTooltip.gameObject.SetActive(true);
            unsheathedTooltip.gameObject.SetActive(false);
            /*
            if (status.canAttack()) {
                if (status.sprinting) {
                    sheathedTooltip.NewElement(playIn.button1Control[0], "Advancing Bladework");
                } else {
                    sheathedTooltip.NewElement(playIn.button1Control[0], "Draw weapon");
                }
            }
            */
        } else {
            unsheathedTooltip.gameObject.SetActive(true);
            sheathedTooltip.gameObject.SetActive(false);
            switch (style.state) {
                case AtkStyleWish.attackStates.idle:
                    if (status.sprinting) {
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Advancing Bladework", 0);
                        unsheathedTooltip.NewElement(playIn.button3Control[0], "Overhead", 0);
                    } else {
                        unsheathedTooltip.NewElement(playIn.sheatheControl[0], "Sheathe weapon", 0);
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 1", TooltipElement.InputDirection.forward);
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Backward Bladework", TooltipElement.InputDirection.back);
                        unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 1", TooltipElement.InputDirection.forward);
                        unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 1", TooltipElement.InputDirection.forward);
                    }
                    break;

                case AtkStyleWish.attackStates.fEvade:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 3", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.attackStates.bEvade:
                    break;

                case AtkStyleWish.attackStates.rEvade:
                    break;

                case AtkStyleWish.attackStates.lEvade:
                    break;

                case AtkStyleWish.attackStates.fParry:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button1Control[0] + " + " + playIn.button2Control[0], "Advancing Bladework", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.attackStates.bParry:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button1Control[0] + " + " + playIn.button3Control[0], "Overhead", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.attackStates.bladework1:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button1Control[0] + " + " + playIn.button3Control[0], "Overhead", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.attackStates.bladework2:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 3", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.attackStates.bladework3:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 4", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 4", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 4", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.attackStates.bladework4:
                    break;
            }
        }

    }

    void FixedUpdate() { 
        if (drawCount > 0) {
            drawCount--;
        }
        if (style.state == AtkStyleWish.attackStates.drawing && drawCount <= 0) {
            style.state = AtkStyleWish.attackStates.idle;
            status.sheathed = false;
            style.source.PlayOneShot(style.drawClip, 0.8f);
        }

        if (status.sheathed == false && drawCount <= 0) {
            cmdBuffer();
        }

        //print(style.state);
        lckBuffer();
    }


}
