using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour {

    //SELF-LOCKOUTS
    public bool rollLock; //the player is rolling, so they cannot act - though they can (should be able to) buffer attacks.
    public bool guardLock; //the player is guarding
    public int parryLock; //the player has just attempted to parry so they cannot use regular movement
    public bool attackLock; //the player is attacking so they cannot use regular movement
    public bool toIdleLock; //the player needs to wait to return to idle to use movement
    public bool casting; //the player is about to cast a spell so they will have to stop or cast something to act
    public int castLock; //the player is in the process of casting a spell, so they can't do anything for a little while
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
    public bool HPRegenEnabled;
    public bool SPRegenEnabled;
    public bool MPRegenEnabled;
    //END OF MISCELLANEOUS STATES

    //CONDITIONS
    public int invulnerable; //this is a good state for you! It means, essentially, that you can't really be hit by anything.
    public int floored; //the target is on the floor. The target can evade to get up and end the condition.
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

    public List<Effect> effects; //a list of effects that are currently active on this entity

    public AtkStyle atkStyle; //Reference to atkStyle for the sake of flinching (needs to return entity to idle)
    public StatSheet stat; //reference to the stat sheet to decrease SP mainly
    public Animator animator; //reference to the animator for the sake of animating

    public bool spellFlinchTrigger; //to be referenced by any associated spellcasting to stop it if the entity flinches

    // Use this for initialization
    void Start () {
        //initialise the variables to the defaults;
        rollLock = false;
        guardLock = false;
        parryLock = 0;
        attackLock = false;
        toIdleLock = false;
        casting = false;
        castLock = 0;
        channelLock = false;

        parryFrames = 0;
        guarding = false;

        moving = false;
        sprinting = false;
        sneaking = false;
        rolling = false;
        sheathed = true;
        HPRegenEnabled = true;
        SPRegenEnabled = true;
        MPRegenEnabled = true;

        invulnerable = 0;
        floored = 0;
        vulnerable = 0;
        silenced = 0;
        airborne = 0;
        stunned = 0;
        paralyzed = 0;
        guardStunned = 0;
        parryStunned = 0;
        grappled = 0;

        spellFlinchTrigger = false;
	}

    //Flinch (breaks the entity out of all current actions)
    public void flinch() {
        rollLock = false;
        guardLock = false;
        parryLock = 0;
        attackLock = false;
        toIdleLock = false;
        casting = false;
        castLock = 0;
        channelLock = false;

        atkStyle.forceIdle();
        atkStyle.destroyAllAttacks();
        atkStyle.movement.motor.rb.velocity *= 0.2f; //reduce velocity because wahey you're being hit

        if (GetComponent<Motor>()) { //if the flinching entity has a motor, ensure that motor.timedBurst won't cause unexpected movement
            GetComponent<Motor>().timeOut = true;
        }

        spellFlinchTrigger = true;

        animator.Play("flinch");//and maybe play the flinch animation I guess not sure
    }

    //STATE CHECKS

    public bool isAirborne() { //GUARD STUNNED
        if (airborne > 0) {
            return true;
        }
        return false;
    }

    public bool isFloored() { //FLOORED
        if (floored > 0 && !isAirborne()) { //you do not count as floored while airborne, though the condition will persist after airborne finishes
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
        if (vulnerable > 0 || sprinting || casting || castLock > 0 || isParryStunned() || parryLock > 0) {
            return true;
        }
        return false;
    }

    public bool canMove() { //CAN MOVE
        if (isFloored() || isStunned() || isGrappled() || isGuardStunned() || isParryStunned() || parryLock > 0 || parryFrames > 0 || rolling || attackLock || casting || castLock > 0 || toIdleLock) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool canRoll() { //CAN EVADE
        if (isStunned() || isGuardStunned() || isParryStunned() || parryLock > 0 || parryFrames > 0 || rollLock || attackLock || casting || castLock > 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool canAttack() { //CAN ATTACK
        if (isFloored() || isStunned() || isGuardStunned() || isParryStunned() || parryLock > 0 || parryFrames > 0 || rolling || attackLock || casting || castLock > 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool canGuard() {
        if (isFloored() || isStunned() || isParryStunned() || rollLock || attackLock || parryLock > 0 || parryFrames > 0 || casting || castLock > 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool canParry() {
        if (isFloored() || isStunned() || isGuardStunned() || isParryStunned() || rollLock || attackLock || parryLock > 0 || parryFrames > 0 || casting || castLock > 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool canCast() {
        if (isFloored() || isStunned() || isGuardStunned() || isParryStunned() || silenced > 0 || rollLock || attackLock || parryLock > 0 || parryFrames > 0 || castLock > 0) {
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
            if getcomponent in children attack? componentattack.delete();
            and maybe play the flinch animation I guess not sure
        }
        
    */

    // FixedUpdate is called once per frame consistently (not rendered frames like Update)
    void FixedUpdate() {

        //STATUS TRACKING / UPDATING (all of these statuses have frame timers. If they're above zero they're active, and they decrease by 1 each frame)
        if (parryFrames > 0) {
            parryFrames -= 1;
        }
        if (parryLock > 0) {
            parryLock -= 1;
        }
        if (castLock > 0) {
            castLock -= 1;
        }
        if (floored > 0) {
            floored -= 1;
        }
        if (vulnerable > 0) {
            vulnerable -= 1;
        }
        if (silenced > 0) {
            silenced -= 1;
        }
        if (airborne > 0) {
            airborne -= 1;
        }
        if (stunned > 0) {
            stunned -= 1;
        }
        if (paralyzed > 0) {
            paralyzed -= 1;
        }
        if (guardStunned > 0) {
            guardStunned -= 1;
        }
        if (parryStunned > 0) {
            parryStunned -= 1;
        }
        if (grappled > 0) {
            grappled -= 1;
        }
        //END OF STATUS TRACKING / UPDATING
        
    }

    // Update is called once per frame
    void Update () {
        //THINGS FOR THE ANIMATOR
        animator.SetBool("stunned", isStunned());
        animator.SetBool("guardStunned", isGuardStunned());
        animator.SetBool("floored", isFloored());
        animator.SetBool("casting", casting);
	}
}
