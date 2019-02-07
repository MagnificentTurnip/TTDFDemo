using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBridge : MonoBehaviour {

    public PlayerInput playIn;
    public AtkStyle style;
    public StatusManager status;

    public enum buffer {
        nothing, spellcast, fparry, bparry, evade, sheathe,
        fwdA, fwdS, fwdD, fwdAS, fwdAD, fwdSD, fwdASD,
        bakA, bakS, bakD, bakAS, bakAD, bakSD, bakASD,
        rgtA, rgtS, rgtD, rgtAS, rgtAD, rgtSD, rgtASD,
        lftA, lftS, lftD, lftAS, lftAD, lftSD, lftASD } 

    public buffer cmdIn;
    public buffer cmdOut;
    public int cmdTime;
    public int cmdCounter;
    public bool cmdRunning;

    public buffer lck;
    public int lckTime;
    public int lckCounter;
    
    public int drawCount;

    public void handleDefences() { //be sure to call this in Update in any PlayerBridge
        //Guarding
        if ((playIn.guard && status.canGuard()) || status.isGuardStunned()) {
            status.guarding = true;
            status.guardLock = true;
            style.animator.SetBool("guarding", true);
        }
        else {
            status.guarding = false;
            status.guardLock = false;
            style.animator.SetBool("guarding", false);
        }


        //parrying
        if (playIn.fParry && status.canParry()) {
            style.fParry();
        }

        if (playIn.bParry && status.canParry()) {
            style.bParry();
        }
    }

    public void cmdBuffer() { //a buffer for attack-based moves so that multi-button inputs aren't overridden by single-button inputs. Call in FIXED UPDATE, and set cmdCounter equal to cmdTime in Start() or the inspector.

        cmdOut = buffer.nothing; //setting cmdOut to nothing at the beginning of the function ensures that cmdOut is only set to an active action for one frame
        //(this prevents the same input being repeated over and over again)

        //each potential input sets cmdIn's state. Inputs with more buttons come first in a series of else-ifs to ensure they take priority over the single-button inputs they contain.
        if (playIn.fwdASD.Check()) {
            cmdIn = buffer.fwdASD;
            cmdRunning = true;
        }
        else if (playIn.bakASD.Check()) {
            cmdIn = buffer.bakASD;
            cmdRunning = true;
        }
        else if (playIn.rgtASD.Check()) {
            cmdIn = buffer.rgtASD;
            cmdRunning = true;
        }
        else if (playIn.lftASD.Check()) {
            cmdIn = buffer.lftASD;
            cmdRunning = true;
        }

        else if (playIn.fwdAS.Check()) {
            cmdIn = buffer.fwdAS;
            cmdRunning = true;
        }
        else if (playIn.bakAS.Check()) {
            cmdIn = buffer.bakAS;
            cmdRunning = true;
        }
        else if (playIn.rgtAS.Check()) {
            cmdIn = buffer.rgtAS;
            cmdRunning = true;
        }
        else if (playIn.lftAS.Check()) {
            cmdIn = buffer.lftAS;
            cmdRunning = true;
        }

        else if (playIn.fwdAD.Check()) {
            cmdIn = buffer.fwdAD;
            cmdRunning = true;
        }
        else if (playIn.bakAD.Check()) {
            cmdIn = buffer.bakAD;
            cmdRunning = true;
        }
        else if (playIn.rgtAD.Check()) {
            cmdIn = buffer.rgtAD;
            cmdRunning = true;
        }
        else if (playIn.lftAD.Check()) {
            cmdIn = buffer.lftAD;
            cmdRunning = true;
        }

        else if (playIn.fwdSD.Check()) {
            cmdIn = buffer.fwdSD;
            cmdRunning = true;
        }
        else if (playIn.bakSD.Check()) {
            cmdIn = buffer.bakSD;
            cmdRunning = true;
        }
        else if (playIn.rgtSD.Check()) {
            cmdIn = buffer.rgtSD;
            cmdRunning = true;
        }
        else if (playIn.lftSD.Check()) {
            cmdIn = buffer.lftSD;
            cmdRunning = true;
        }

        else if (playIn.fwdA.Check()) {
            cmdIn = buffer.fwdA;
            cmdRunning = true;
        }
        else if (playIn.bakAS.Check()) {
            cmdIn = buffer.bakA;
            cmdRunning = true;
        }
        else if (playIn.rgtAS.Check()) {
            cmdIn = buffer.rgtA;
            cmdRunning = true;
        }
        else if (playIn.lftAS.Check()) {
            cmdIn = buffer.lftA;
            cmdRunning = true;
        }

        else if (playIn.fwdS.Check()) {
            cmdIn = buffer.fwdS;
            cmdRunning = true;
        }
        else if (playIn.bakS.Check()) {
            cmdIn = buffer.bakS;
            cmdRunning = true;
        }
        else if (playIn.rgtS.Check()) {
            cmdIn = buffer.rgtS;
            cmdRunning = true;
        }
        else if (playIn.lftS.Check()) {
            cmdIn = buffer.lftS;
            cmdRunning = true;
        }

        else if (playIn.fwdD.Check()) {
            cmdIn = buffer.fwdD;
            cmdRunning = true;
        }
        else if (playIn.bakD.Check()) {
            cmdIn = buffer.bakD;
            cmdRunning = true;
        }
        else if (playIn.rgtD.Check()) {
            cmdIn = buffer.rgtD;
            cmdRunning = true;
        }
        else if (playIn.lftD.Check()) {
            cmdIn = buffer.lftD;
            cmdRunning = true;
        }

        if (cmdRunning == true) { //the counter runs down every frame so long as the buffer has been activated
            cmdCounter -= 1;
        }

        if (cmdCounter <= 0) { //once the counter runs out, the current buffered input is chosen and everything is reset.
            cmdRunning = false;
            cmdCounter = cmdTime;
            cmdOut = cmdIn;
            cmdIn = buffer.nothing;
        }

    }

    public void lckBuffer() { //a buffer that saves inputted commands for later if they can't currently be performed. Call in FIXED UPDATE.
        if (lckCounter <= 0) {
            lck = buffer.nothing; //after a certain amount of time, buffered inputs are purged
        } else {
            lckCounter -= 1; //counter ticks down by 1 every frame
        }

        if (status.sheathed == false || drawCount > 0) { //only buffer attack commands if unsheathed so that just regular drawing without attacking is possible
            if (cmdOut != buffer.nothing) {
                lck = cmdOut;
                lckCounter = lckTime;
            }
        }

        if (playIn.evade) {
            lck = buffer.evade;
            lckCounter = lckTime;
        }

        if (playIn.guard || playIn.movement) { //purge the buffer if a different command that doesn't need to be buffered is registered
            lck = buffer.nothing;
        }
    }


}