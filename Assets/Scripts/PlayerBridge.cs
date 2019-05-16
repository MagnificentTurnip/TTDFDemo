using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBridge : MonoBehaviour {

    public PlayerInput playIn;
    public virtual AtkStyle Style { get; set; }
    public StatusManager status;
    public PlayerSpellBook spellBook;
    public TooltipManager unsheathedTooltip;
    public TooltipManager sheathedTooltip;

    public bool canGuardCancel;

    public enum Buffer {
        nothing, spellcast, fparry, bparry, evade, sheathe,
        fwdA, fwdS, fwdD, fwdAS, fwdAD, fwdSD, fwdASD,
        bakA, bakS, bakD, bakAS, bakAD, bakSD, bakASD,
        rgtA, rgtS, rgtD, rgtAS, rgtAD, rgtSD, rgtASD,
        lftA, lftS, lftD, lftAS, lftAD, lftSD, lftASD } 

    public Buffer cmdIn;
    public Buffer cmdOut;
    public int cmdTime;
    public int cmdCounter;
    public bool cmdRunning;

    public Buffer lck;
    public int lckTime;
    public int lckCounter;
    
    public int drawCount;

    public void HandleDefences() { //be sure to call this in Update in any PlayerBridge
        //Guarding
        if ((playIn.guard && status.CanGuard()) || status.IsGuardStunned()) {
            if (playIn.guard && status.CanGuard()) {
                Style.movement.PointToTarget();
            }
            status.guarding = true;
            status.guardLock = true;
            Style.ForceGuarding(3);
        }
        else {
            status.guarding = false;
            status.guardLock = false;
        }

        //Guard-Cancelling
        canGuardCancel = true;
        for (int i = 0; i < Style.instantiatedAttacks.Count; i++) {
            if (Style.instantiatedAttacks[i].data.attackDelay <= 0) {
                canGuardCancel = false;
            }
        }
        if (status.IsFloored() || status.IsStunned() || status.IsParryStunned() || status.rollLock || status.parryLock > 0 || status.parryFrames > 0 || status.casting || status.castLock > 0 || status.channelLock > 0) {
            canGuardCancel = false; //basically all of canGuard but ignoring attackLock
        }
        if (canGuardCancel && playIn.guard) {
            Style.movement.motor.timeOut = true;
            Style.DestroyAllAttacks();
            status.guarding = true;
            status.guardLock = true;
            Style.ForceGuarding(10);
        }

        //parrying
        if (playIn.fParry && status.CanParry() && !PauseMenu.paused) {
            Style.FParry();
        }

        if (playIn.bParry && status.CanParry() && !PauseMenu.paused) {
            Style.BParry();
        }       

    }

    public void CmdBuffer() { //a buffer for attack-based moves so that multi-button inputs aren't overridden by single-button inputs. Call in FIXED UPDATE, and set cmdCounter equal to cmdTime in Start() or the inspector.

        cmdOut = Buffer.nothing; //setting cmdOut to nothing at the beginning of the function ensures that cmdOut is only set to an active action for one frame
        //(this prevents the same input being repeated over and over again)

        //each potential input sets cmdIn's state. Inputs with more buttons come first in a series of else-ifs to ensure they take priority over the single-button inputs they contain.
        if (playIn.fwdASD.Check()) {
            cmdIn = Buffer.fwdASD;
            cmdRunning = true;
        }
        else if (playIn.bakASD.Check()) {
            cmdIn = Buffer.bakASD;
            cmdRunning = true;
        }
        else if (playIn.rgtASD.Check()) {
            cmdIn = Buffer.rgtASD;
            cmdRunning = true;
        }
        else if (playIn.lftASD.Check()) {
            cmdIn = Buffer.lftASD;
            cmdRunning = true;
        }

        if (cmdIn != Buffer.fwdASD && cmdIn != Buffer.bakASD && cmdIn != Buffer.rgtASD && cmdIn != Buffer.lftASD) { //button commands with fewer inputs don't override button commands with more inputs
            if (playIn.fwdAS.Check()) {
                cmdIn = Buffer.fwdAS;
                cmdRunning = true;
            }
            else if (playIn.bakAS.Check()) {
                cmdIn = Buffer.bakAS;
                cmdRunning = true;
            }
            else if (playIn.rgtAS.Check()) {
                cmdIn = Buffer.rgtAS;
                cmdRunning = true;
            }
            else if (playIn.lftAS.Check()) {
                cmdIn = Buffer.lftAS;
                cmdRunning = true;
            }

            else if (playIn.fwdAD.Check()) {
                cmdIn = Buffer.fwdAD;
                cmdRunning = true;
            }
            else if (playIn.bakAD.Check()) {
                cmdIn = Buffer.bakAD;
                cmdRunning = true;
            }
            else if (playIn.rgtAD.Check()) {
                cmdIn = Buffer.rgtAD;
                cmdRunning = true;
            }
            else if (playIn.lftAD.Check()) {
                cmdIn = Buffer.lftAD;
                cmdRunning = true;
            }

            else if (playIn.fwdSD.Check()) {
                cmdIn = Buffer.fwdSD;
                cmdRunning = true;
            }
            else if (playIn.bakSD.Check()) {
                cmdIn = Buffer.bakSD;
                cmdRunning = true;
            }
            else if (playIn.rgtSD.Check()) {
                cmdIn = Buffer.rgtSD;
                cmdRunning = true;
            }
            else if (playIn.lftSD.Check()) {
                cmdIn = Buffer.lftSD;
                cmdRunning = true;
            }


            if (cmdIn != Buffer.fwdAS && cmdIn != Buffer.bakAS && cmdIn != Buffer.rgtAS && cmdIn != Buffer.lftAS 
                && cmdIn != Buffer.fwdAD && cmdIn != Buffer.bakAD && cmdIn != Buffer.rgtAD && cmdIn != Buffer.lftAD 
                && cmdIn != Buffer.fwdSD && cmdIn != Buffer.bakSD && cmdIn != Buffer.rgtSD && cmdIn != Buffer.lftSD) { //button commands with fewer inputs don't override button commands with more inputs
                if (playIn.fwdA.Check()) {
                    cmdIn = Buffer.fwdA;
                    cmdRunning = true;
                }
                else if (playIn.bakA.Check()) {
                    cmdIn = Buffer.bakA;
                    cmdRunning = true;
                }
                else if (playIn.rgtA.Check()) {
                    cmdIn = Buffer.rgtA;
                    cmdRunning = true;
                }
                else if (playIn.lftA.Check()) {
                    cmdIn = Buffer.lftA;
                    cmdRunning = true;
                }

                else if (playIn.fwdS.Check()) {
                    cmdIn = Buffer.fwdS;
                    cmdRunning = true;
                }
                else if (playIn.bakS.Check()) {
                    cmdIn = Buffer.bakS;
                    cmdRunning = true;
                }
                else if (playIn.rgtS.Check()) {
                    cmdIn = Buffer.rgtS;
                    cmdRunning = true;
                }
                else if (playIn.lftS.Check()) {
                    cmdIn = Buffer.lftS;
                    cmdRunning = true;
                }

                else if (playIn.fwdD.Check()) {
                    cmdIn = Buffer.fwdD;
                    cmdRunning = true;
                }
                else if (playIn.bakD.Check()) {
                    cmdIn = Buffer.bakD;
                    cmdRunning = true;
                }
                else if (playIn.rgtD.Check()) {
                    cmdIn = Buffer.rgtD;
                    cmdRunning = true;
                }
                else if (playIn.lftD.Check()) {
                    cmdIn = Buffer.lftD;
                    cmdRunning = true;
                }
            }
        }


        if (cmdRunning == true) { //the counter runs down every frame so long as the buffer has been activated
            cmdCounter -= 1;
        }

        if (cmdCounter <= 0) { //once the counter runs out, the current buffered input is chosen and everything is reset.
            cmdRunning = false;
            cmdCounter = cmdTime;
            cmdOut = cmdIn;
            cmdIn = Buffer.nothing;
        }

    }

    public void LckBuffer() { //a buffer that saves inputted commands for later if they can't currently be performed. Call in FIXED UPDATE.
        if (lckCounter <= 0) {
            lck = Buffer.nothing; //after a certain amount of time, buffered inputs are purged
        } else {
            lckCounter -= 1; //counter ticks down by 1 every frame
        }

        if (status.sheathed == false || drawCount > 0) { //only buffer attack commands if unsheathed so that just regular drawing without attacking is possible
            if (cmdOut != Buffer.nothing) {
                lck = cmdOut;
                lckCounter = lckTime;
            }
        }

        if (playIn.evade) {
            lck = Buffer.evade;
            lckCounter = lckTime;
        }

        if (playIn.guard || playIn.movement) { //purge the buffer if a different command that doesn't need to be buffered is registered
            lck = Buffer.nothing;
        }
    }


}