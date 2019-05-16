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
        HandleDefences();

        //unsheathing
        if (status.sheathed == true && playIn.button1 && status.CanAttack()) {
            if (status.sprinting) { // the player performs an unsheathe attack if sprinting at the point of drawing their weapon
                style.AdvancingBladework();
                style.bladeTracker = 0;
                status.sheathed = false;
                style.source.PlayOneShot(style.drawClip, 0.8f);
            } else {
                style.state = AtkStyleWish.AttackStates.drawing;
                drawCount = 10;
            }
            style.animator.Play("MainBladeOn", 1, 0f);//play the unsheathing animation
            style.bladeTracker = 0;
        }

        if (status.sheathed == false) { //only do attack style stuff when unsheathed
            //evades. Wish style has four directional evades out of attacks.
            if ((lck == Buffer.evade || playIn.evade) && status.CanRoll() && style.state != AtkStyleWish.AttackStates.idle && style.state != AtkStyleWish.AttackStates.fEvade && style.state != AtkStyleWish.AttackStates.bEvade && style.state != AtkStyleWish.AttackStates.lEvade && style.state != AtkStyleWish.AttackStates.rEvade) {
                if (playIn.pointForward == true) {
                    print(style.state);
                    style.EvadeForward();
                }
                if (playIn.pointBack == true) {
                    style.EvadeBack();
                }
                if (playIn.pointRight == true) {
                    style.EvadeRight();
                }
                if (playIn.pointLeft == true) {
                    style.EvadeLeft();
                } //all of these evades do not alter the direction that the player is facing.
            }

            //manage regular movement rolling transitioning momentarily into the fEvade state
            if ((lck == Buffer.evade || playIn.evade) && status.CanRoll() && !status.sprinting && style.state == AtkStyleWish.AttackStates.idle && style.state != AtkStyleWish.AttackStates.fEvade && style.state != AtkStyleWish.AttackStates.bEvade && style.state != AtkStyleWish.AttackStates.lEvade && style.state != AtkStyleWish.AttackStates.rEvade) {
                style.state = AtkStyleWish.AttackStates.fEvade; //set the attack state;
                style.idleCounter = 30; //always remember to reset the idle counter
            }

            //attacks
            if (status.CanAttack()) {
                switch (style.state) {
                    
                    case AtkStyleWish.AttackStates.idle:
                        if (playIn.sheathe) { //from idle you can sheathe again
                            status.sheathed = true;
                            style.animator.Play("MainBladeOff", 1, 0f); //play the sheathing animation
                            style.bladeTracker = 0;
                            style.source.PlayOneShot(style.sheathClip, 1f);
                        }
                        if (status.sprinting && (cmdOut == Buffer.fwdA || lck == Buffer.fwdA)) {
                            style.AdvancingBladework();
                            style.bladeTracker = 0;
                        }
                        else if (cmdOut == Buffer.fwdA || lck == Buffer.fwdA) {
                            style.StandardBladework1();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.bakA || lck == Buffer.bakA) {
                            style.BackwardBladework();
                            style.bladeTracker = 0;
                        }

                        if (status.sprinting && (cmdOut == Buffer.fwdS || lck == Buffer.fwdS)) {
                            style.TwisterA();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 1) {
                                style.animator.Play("GreatBladeOff", 1, 0f);
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            style.bladeTracker = 2;
                        }
                        else if (cmdOut == Buffer.fwdS || lck == Buffer.fwdS) {
                            style.LightBladework1();
                            style.bladeTracker = 0;
                        }
                        
                        if (status.sprinting && (cmdOut == Buffer.fwdD || lck == Buffer.fwdD || cmdOut == Buffer.fwdAD || lck == Buffer.fwdAD)) {
                            style.Overhead();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 2) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                                style.animator.Play("SecBladeOff", 1, 0f);
                            }
                            style.bladeTracker = 1;
                        }
                        else if (cmdOut == Buffer.fwdD || lck == Buffer.fwdD) {
                            style.HeavyBladework1();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.lftA || lck == Buffer.lftA) {
                            style.TyphoonA("Typhoon1");
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.rgtA || lck == Buffer.rgtA) {
                            style.TyphoonB("Typhoon1");
                            style.bladeTracker = 0;
                        }

                        if (cmdOut == Buffer.lftS || lck == Buffer.lftS) {
                            style.CycloneA();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 1) {
                                style.animator.Play("GreatBladeOff", 1, 0f);
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            style.bladeTracker = 2;
                        }
                        if (cmdOut == Buffer.rgtS || lck == Buffer.rgtS) {
                            style.CycloneB();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 1) {
                                style.animator.Play("GreatBladeOff", 1, 0f);
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            style.bladeTracker = 2;
                        }

                        break;

                    case AtkStyleWish.AttackStates.fEvade:
                        if (cmdOut == Buffer.fwdA || lck == Buffer.fwdA) {
                            style.StandardBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdS || lck == Buffer.fwdS) {
                            style.LightBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdD || lck == Buffer.fwdD) {
                            style.HeavyBladework3();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.AttackStates.bEvade:
                        break;

                    case AtkStyleWish.AttackStates.rEvade:
                        break;

                    case AtkStyleWish.AttackStates.lEvade:
                        break;

                    case AtkStyleWish.AttackStates.fParry:
                        if (cmdOut == Buffer.fwdAS || lck == Buffer.fwdAS) {
                            style.AdvancingBladework();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdSD || lck == Buffer.fwdSD) {
                            style.TwisterA();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdA || lck == Buffer.fwdA) {
                            style.StandardBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdS || lck == Buffer.fwdS) {
                            style.LightBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdD || lck == Buffer.fwdD) {
                            style.HeavyBladework3();
                            style.bladeTracker = 0;
                        }

                        break;

                    case AtkStyleWish.AttackStates.bParry:
                        if (cmdOut == Buffer.fwdAD || lck == Buffer.fwdAD) {
                            style.Overhead();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 2) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                                style.animator.Play("SecBladeOff", 1, 0f);
                            }
                            style.bladeTracker = 1;
                        }
                        if (cmdOut == Buffer.fwdA || lck == Buffer.fwdA) {
                            style.StandardBladework2();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdS || lck == Buffer.fwdS) {
                            style.LightBladework2();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdD || lck == Buffer.fwdD) {
                            style.HeavyBladework2();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.AttackStates.bladework1:
                        if (cmdOut == Buffer.fwdAD || lck == Buffer.fwdAD) {
                            style.Overhead();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 2) {
                                style.animator.Play("GreatBladeOn", 1, 0f);
                                style.animator.Play("SecBladeOff", 1, 0f);
                            }
                            style.bladeTracker = 1;
                        }
                        if (cmdOut == Buffer.fwdA || lck == Buffer.fwdA) {
                            style.StandardBladework2();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdS || lck == Buffer.fwdS) {
                            style.LightBladework2();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdD || lck == Buffer.fwdD) {
                            style.HeavyBladework2();
                            style.bladeTracker = 0;
                        }

                        if (cmdOut == Buffer.lftS || lck == Buffer.lftS) {
                            style.CycloneA();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 1) {
                                style.animator.Play("GreatBladeOff", 1, 0f);
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            style.bladeTracker = 2;
                        }
                        if (cmdOut == Buffer.rgtS || lck == Buffer.rgtS) {
                            style.CycloneB();
                            if (style.bladeTracker == 0) {
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            if (style.bladeTracker == 1) {
                                style.animator.Play("GreatBladeOff", 1, 0f);
                                style.animator.Play("SecBladeOn", 1, 0f);
                            }
                            style.bladeTracker = 2;
                        }
                        break;

                    case AtkStyleWish.AttackStates.bladework2:
                        if (cmdOut == Buffer.fwdA || lck == Buffer.fwdA) {
                            style.StandardBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdS || lck == Buffer.fwdS) {
                            style.LightBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdD || lck == Buffer.fwdD) {
                            style.HeavyBladework3();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.lftA || lck == Buffer.lftA) {
                            style.TyphoonA("Typhoon3");
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.rgtA || lck == Buffer.rgtA) {
                            style.TyphoonB("Typhoon3");
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.AttackStates.bladework3:
                        if (cmdOut == Buffer.fwdA || lck == Buffer.fwdA) {
                            style.StandardBladework4();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdS || lck == Buffer.fwdS) {
                            style.LightBladework4();
                            style.bladeTracker = 0;
                        }
                        if (cmdOut == Buffer.fwdD || lck == Buffer.fwdD) {
                            style.HeavyBladework4();
                            style.bladeTracker = 0;
                        }
                        break;

                    case AtkStyleWish.AttackStates.bladework4:
                        break;
                }

            }
            //end of attacks
        }

        //tooltip things 
        unsheathedTooltip.ClearElements();
        sheathedTooltip.ClearElements();
        if (status.sheathed) {
            sheathedTooltip.gameObject.SetActive(true);
            unsheathedTooltip.gameObject.SetActive(false);
            
            if (status.CanAttack()) {
                if (status.sprinting) {
                    sheathedTooltip.NewElement(playIn.button1Control[0], "Advancing Bladework", 0);
                } else {
                    sheathedTooltip.NewElement(playIn.button1Control[0], "Draw weapon", 0);
                }
            }
            if (!status.casting) {
                sheathedTooltip.NewElement(playIn.button3Control[0], "Initiate Dialogue", 0);
                sheathedTooltip.NewElement(playIn.button4Control[0], "Interact / Inspect", 0);
            }

        } else {
            unsheathedTooltip.gameObject.SetActive(true);
            sheathedTooltip.gameObject.SetActive(false);
            switch (style.state) {
                case AtkStyleWish.AttackStates.idle:
                    if (status.sprinting) {
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Advancing Bladework", 0);
                        unsheathedTooltip.NewElement(playIn.button3Control[0], "Overhead", 0);
                        unsheathedTooltip.NewElement(playIn.button2Control[0], "Twister", 0);
                    } else {
                        unsheathedTooltip.NewElement(playIn.sheatheControl[0], "Sheathe weapon", 0);
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 1", TooltipElement.InputDirection.forward);
                        unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 1", TooltipElement.InputDirection.forward);
                        unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 1", TooltipElement.InputDirection.forward);
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Typhoon A", TooltipElement.InputDirection.left);
                        unsheathedTooltip.NewElement(playIn.button2Control[0], "Cyclone A", TooltipElement.InputDirection.left);
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Typhoon B", TooltipElement.InputDirection.right);
                        unsheathedTooltip.NewElement(playIn.button2Control[0], "Cyclone B", TooltipElement.InputDirection.right);
                        unsheathedTooltip.NewElement(playIn.button1Control[0], "Backward Bladework", TooltipElement.InputDirection.back);
                    }
                    break;

                case AtkStyleWish.AttackStates.fEvade:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 3", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.AttackStates.bEvade:
                    break;

                case AtkStyleWish.AttackStates.rEvade:
                    break;

                case AtkStyleWish.AttackStates.lEvade:
                    break;

                case AtkStyleWish.AttackStates.fParry:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button1Control[0] + " + " + playIn.button2Control[0], "Advancing Bladework", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0] + " + " + playIn.button3Control[0], "Twister", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.AttackStates.bParry:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button1Control[0] + " + " + playIn.button3Control[0], "Overhead", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.AttackStates.bladework1:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 2", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button1Control[0] + " + " + playIn.button3Control[0], "Overhead", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Cyclone A", TooltipElement.InputDirection.left);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Cyclone B", TooltipElement.InputDirection.right);
                    break;

                case AtkStyleWish.AttackStates.bladework2:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 3", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Typhoon A", TooltipElement.InputDirection.left);
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Typhoon B", TooltipElement.InputDirection.right);
                    break;

                case AtkStyleWish.AttackStates.bladework3:
                    unsheathedTooltip.NewElement(playIn.button1Control[0], "Standard Bladework 4", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button2Control[0], "Light Bladework 4", TooltipElement.InputDirection.forward);
                    unsheathedTooltip.NewElement(playIn.button3Control[0], "Heavy Bladework 4", TooltipElement.InputDirection.forward);
                    break;

                case AtkStyleWish.AttackStates.bladework4:
                    break;
                    
            }
        }

    }

    void FixedUpdate() { 
        if (drawCount > 0) {
            drawCount--;
        }
        if (style.state == AtkStyleWish.AttackStates.drawing && drawCount <= 0) {
            style.state = AtkStyleWish.AttackStates.idle;
            status.sheathed = false;
            style.source.PlayOneShot(style.drawClip, 0.8f);
        }

        if (status.sheathed == false && drawCount <= 0) {
            CmdBuffer();
        }

        //print(style.state);
        LckBuffer();
    }


}
