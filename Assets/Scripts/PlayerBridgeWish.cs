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

        if (status.unsheathed) { //only do attack style stuff when unsheathed
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
                if (wish.state == AtkStyleWish.attackStates.idle && playIn.fwdA.Check()) {
                    wish.standardBladework1();
                }

            }
            //end of attacks
        }

    }




}
