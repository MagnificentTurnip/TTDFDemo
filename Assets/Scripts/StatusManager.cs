using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour {

    //SELF-LOCKOUTS
    public bool rollLock; //the player is rolling, so they cannot act - though they can (should be able to) buffer attacks.
    public bool guardLock; //the player is guarding
    public bool parryLock; //the player has just parried someone so they cannot use regular movement
    public bool attackLock; //the player is attacking so they cannot use regular movement
    public bool castLock; //the player is casting a spell so they will have to stop or cast something to act
    public bool channelLock; //the player is in the middle of channeling a spell. They can move slowly and may not attack, guard or parry. Evading is possible but breaks the channel.
    //END OF SELF-LOCKOUTS

    //DEFENSIVE STATES
    public int parryFrames;
    public bool guarding;
    //END OF DEFENSIVE STATES

    //MISCELLANEOUS STATES
    public bool moving;
    public bool sprinting;
    public bool sneaking;
    public bool rolling;
    public bool sheathed;
    public bool unsheathed;
    public bool HPRegenEnabled;
    public bool SPRegenEnabled;
    public bool MPRegenEnabled;
    //END OF MISCELLANEOUS STATES

    //CONDITIONS
    public bool floored; //the target is on the floor. The target can evade to get up and end the condition.
    public int vulnerable; //this condition itself doesn't do anything bad, but being hit by most things generally ends up worse for someone who is vulnerable than someone who isn't.
    public int silenced; //the target is prevented from casting or channeling spells.
    public int airborne; //the target is up in the air. Nothing is inherently debilitating about this condition, but it carries with it interactions of various niceness.
    public int stunned; //the target is prevented from acting in any way - at least, normally. Some esoteric ability might be usable under stunning but eh, I haven't made one yet.
    public int paralyzed; //the target is slowed and they are unable to sprint. Evading also costs double SP.
    public int guardStunned; //applied to someone who is guarding - similar to a stun, but the victim remains guarding for the duration.
    public int parryStunned; //applied to someone who is parried. Similar to stun but does not prevent reactive parrying.
    public int grappled; //someone who is grappled cannot take action similar to a stun, but can evade out - usually with some consequence. It automatically breaks after the duration (usually long)
                         //or shortly after the grappler acts. ***Grapples can be made entirely inescapable by setting the value to a negative***
                         //**Alternatively, limited-time inescapable grapples are simply a matter of also applying Stun for however long the grapple is inescapable***
    //END OF CONDITIONS

    // Use this for initialization
    void Start () {
        //initialise the variables to the defaults;
        rollLock = false;
        guardLock = false;
        parryLock = false;
        attackLock = false;
        castLock = false;
        channelLock = false;

        parryFrames = 0;
        guarding = false;

        moving = false;
        sprinting = false;
        sneaking = false;
        rolling = false;
        sheathed = true;
        unsheathed = false;
        HPRegenEnabled = true;
        SPRegenEnabled = true;
        MPRegenEnabled = true;

        floored = false;
        vulnerable = 0;
        silenced = 0;
        airborne = 0;
        stunned = 0;
        paralyzed = 0;
        guardStunned = 0;
        parryStunned = 0;
        grappled = 0;
	}

    //STATE CHECKS
    public bool isFloored() { //FLOORED
        if (floored) {
            return true;
        }
        return false;
    }

    public bool isAirborne() { //GUARD STUNNED
        if (airborne > 0) {
            return true;
        }
        return false;
    }

    public bool isStunned() { //STUNNED
        if (stunned > 0) {
            return true;
        }
        return false;
    }

    public bool isGrappled() { //GRAPPLED
        if (grappled > 0) {
            return true;
        }
        return false;
    }

    public bool isGuardStunned() { //GUARD STUNNED
        if (guardStunned > 0) {
            return true;
        }
        return false;
    }

    public bool isParryStunned() { //PARRY STUNNED
        if (parryStunned > 0) {
            return true;
        }
        return false;
    }

    public bool isVulnerable() { //VULNERABLE
        if (vulnerable > 0 || sprinting || castLock || parryStunned > 0) {
            return true;
        }
        return false;
    }

    public bool canMove() { //CAN MOVE
        if (isFloored() || isStunned() || isGrappled() || isGuardStunned() || isParryStunned() || parryLock || rolling || attackLock || castLock) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool canRoll() { //CAN EVADE
        if (isStunned() || isGuardStunned() || isAirborne() || isParryStunned() || rollLock || attackLock || castLock) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool canAttack() { //CAN ATTACK
        if (isFloored() || isStunned() || isGuardStunned() || isAirborne() || isParryStunned() || rollLock || attackLock || castLock) {
            return false;
        }
        else {
            return true;
        }
    }
    //END OF STATE CHECKS
    /*  
    
        TEMPLATE STATUS TRACKING/REFRESHING



        bool isParrying(){
            if (parryFrames > 0){
                return true;
            }
            return false;
        }



        TEMPLATE ABILITY CHECKING

        bool canMove(){
            if (isFloored() || isStunned() || isGrappled() || isGuardStunned() || isParryStunned() || parryLock || rollLock || attackLock || castLock ) {
                return false;
            }
            else {
                return true;
            }
        }



        TEMPLATE FLINCH

        void flinch() {
            rollLock = false;
            guardLock = false;
            parryLock = false;
            attackLock = false;
            castLock = false;
            channelLock = false;
            and maybe play the flinch animation I guess not sure
        }
        
    */

    // Update is called once per frame
    void Update () {
		
	}
}
