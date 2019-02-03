using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBridge : MonoBehaviour {

    public PlayerInput playIn;
    public AtkStyle style;
    public StatusManager status;
    
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

    //something something input buffer?


}