using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StatusManager : MonoBehaviour {

    //SELF-LOCKOUTS
    public bool rollLock; //the unit is rolling, so they cannot act - though they can (should be able to) buffer attacks.
    public bool guardLock; //the unit is guarding
    public int parryLock; //the unit has just attempted to parry so they cannot use regular movement
    public bool attackLock; //the unit is attacking so they cannot use regular movement
    public bool toIdleLock; //the unit needs to wait to return to idle to use movement
    public bool casting; //the unit is about to cast a spell so they will have to stop or cast something to act
    public int castLock; //the unit is in the process of casting a spell, so they can't do anything for a little while
    public int channelLock; //the unit is in the middle of channeling a spell. They can move slowly and may not attack, guard or parry. Evading is possible but breaks the channel.
    public bool delayParryLock; //whether parrying has been stopped within the delay before it becomes active (see Parry())
    //END OF SELF-LOCKOUTS

    //DEFENSIVE STATES
    public int parryFrames; //the amount of frames the unit is parrying for
    public int parryStunFrames; //the amount of frames of parryStun the unit will cause to an attacker upon successful parry
    public bool guarding; //whether or not the unit is guarding
    public bool magicGuard; //whether the unit's guard is magical (has some properties of parrying)
    //END OF DEFENSIVE STATES

    //meshrenderers of objects to visualise the defensive states
    public MeshRenderer guardBubble;
    public MeshRenderer parryBubble;

    //MISCELLANEOUS STATES
    public bool moving;
    public bool sprinting;
    public bool sneaking;
    public bool rolling;
    public bool sheathed;
    public bool HPRegenEnabled;
    public bool SPRegenEnabled;
    public bool MPRegenEnabled;
    public int incorporealFrames; //not really a condition as it overlaps with a lot of other proper conditions - more of a mechanic
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

    //these are kind of conditions, but not really. They don't show up on sliders, at least.
    public bool slain; //bool for tracking if the unit has contracted the Death
    public bool unconscious; //bool for tracking if the unit has Death Lite™

    public List<Effect> effects; //a list of effects that are currently active on this entity

    public AtkStyle atkStyle; //Reference to atkStyle for the sake of flinching (needs to return entity to idle)
    public StatSheet stat; //reference to the stat sheet to decrease SP mainly
    public Animator animator; //reference to the animator for the sake of animating
    public Collider col; //reference to the entity's collider for the sake of going incorporeal

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
        channelLock = 0;
        delayParryLock = true;

        parryFrames = 0;
        parryStunFrames = 0;
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

    public void Incorporealise(int frames) {
        col.isTrigger = true;
        col.attachedRigidbody.useGravity = false;
        incorporealFrames = frames;
    }

    //Flinch (breaks the entity out of all current actions)
    public void Flinch() {
        //print("flinch");
        rollLock = false;
        guardLock = false;
        parryLock = 0;
        attackLock = false;
        toIdleLock = false;
        casting = false;
        delayParryLock = true;
        castLock = 0;
        channelLock = 0;
        parryFrames = 0;
        parryStunFrames = 0;
        unconscious = false;

        atkStyle.ForceIdle();
        atkStyle.DestroyAllAttacks();
        atkStyle.movement.motor.rb.velocity *= 0.2f; //reduce velocity because wahey you're being hit

        if (GetComponent<NavMeshAgent>()) {
            GetComponent<NavMeshAgent>().velocity *= 0.2f; //reduce velocity for navmeshes also
        }

        if (GetComponent<Motor>()) { //if the flinching entity has a motor, ensure that motor.timedBurst won't cause unexpected movement
            GetComponent<Motor>().timeOut = true;
        }

        spellFlinchTrigger = true;

        animator.Play("flinch", -1, 0f);//and play the flinch animation
    }

    public IEnumerator Parry(float timeBeforeStart, int _parryFrames, int _parryLock, int _parryStunFrames) { //ensures standard logic for parrying
        delayParryLock = false;
        parryLock = _parryLock;
        yield return new WaitForSeconds(timeBeforeStart);
        if (!delayParryLock) {
            parryStunFrames = _parryStunFrames;
            parryFrames = _parryFrames;
            delayParryLock = true;
        }
    }

    //STATE CHECKS

    public bool IsAirborne() { //GUARD STUNNED
        if (airborne > 0) {
            return true;
        }
        return false;
    }

    public bool IsFloored() { //FLOORED
        if ((floored > 0 || unconscious || slain) && !IsAirborne()) { //you do not count as floored while airborne, though the condition will persist after airborne finishes
            return true;
        }
        return false;
    }

    public bool IsStunned() { //STUNNED
        if (stunned > 0 || unconscious || slain) {
            return true;
        }
        return false;
    }

    public bool IsGrappled() { //GRAPPLED
        if (grappled > 0) {
            return true;
        }
        return false;
    }

    public bool IsGuardStunned() { //GUARD STUNNED
        if (guardStunned > 0) {
            return true;
        }
        return false;
    }

    public bool IsParryStunned() { //PARRY STUNNED
        if (parryStunned > 0) {
            return true;
        }
        return false;
    }

    public bool IsVulnerable() { //VULNERABLE
        if (vulnerable > 0 || sprinting || casting || castLock > 0 || IsParryStunned() || parryLock > 0) {
            return true;
        }
        return false;
    }

    public bool IsParalyzed() {
        if (paralyzed > 0 || channelLock > 0) {
            return true;
        }
        return false;
    }

    public bool CanMove() { //CAN MOVE
        if (IsFloored() || IsStunned() || IsGrappled() || IsGuardStunned() || IsParryStunned() || parryLock > 0 || parryFrames > 0 || rolling || attackLock || casting || castLock > 0 || toIdleLock) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool CanRoll() { //CAN EVADE
        if (IsStunned() || IsGuardStunned() || IsParryStunned() || parryLock > 0 || parryFrames > 0 || rollLock || attackLock || casting || castLock > 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool CanAttack() { //CAN ATTACK
        if (IsFloored() || IsStunned() || IsGuardStunned() || IsParryStunned() || guarding || parryLock > 0 || parryFrames > 0 || rolling || attackLock || casting || castLock > 0 || channelLock > 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool CanGuard() {
        if (IsFloored() || IsStunned() || IsParryStunned() || rollLock || attackLock || parryLock > 0 || casting || castLock > 0 || channelLock > 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool CanParry() {
        if (IsFloored() || IsStunned() || IsGuardStunned() || rollLock || attackLock || parryLock > 0 || parryFrames > 0 || casting || castLock > 0 || channelLock > 0 || !delayParryLock) {
            return false;
        }
        else {
            return true;
        }
    }

    public bool CanCast() {
        if (IsFloored() || IsStunned() || guarding || IsGuardStunned() || IsParryStunned() || silenced > 0 || rollLock || attackLock || parryLock > 0 || parryFrames > 0) {
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
        if (invulnerable > 0) {
            invulnerable -= 1;
        }
        if (parryFrames > 0) {
            parryFrames -= 1;
        }
        if (parryLock > 0) {
            parryLock -= 1;
        }
        if (castLock > 0) {
            castLock -= 1;
        }
        if (channelLock > 0) {
            channelLock -= 1;
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

        //track incorporeality too even if it's not a status it's kind of like one but not, don't question my naming conventions
        if (incorporealFrames > 0) {
            incorporealFrames -= 1;
        } else {
            col.isTrigger = false;
            col.attachedRigidbody.useGravity = true;
        }

    }

    // Update is called once per frame
    void Update () {
        //THINGS FOR THE ANIMATOR
        animator.SetBool("stunned", IsStunned());
        animator.SetBool("guardStunned", IsGuardStunned());
        animator.SetBool("floored", IsFloored());
        animator.SetBool("airborne", IsAirborne());
        animator.SetBool("casting", casting);
        animator.SetBool("paralyzed", paralyzed > 0);
        animator.SetBool("guarding", guarding);
        animator.SetBool("slain", slain);
        animator.SetBool("unconscious", unconscious);
        //animator.SetFloat("paralyzeFloat", 0.8f);

        //animating defensive bubble properties
        if ((guarding || IsGuardStunned()) && guardBubble != null) {
            guardBubble.material.color = new Color(guardBubble.material.color.r, guardBubble.material.color.g, guardBubble.material.color.b, 0.4f + (0.01f * guardStunned));
        } else {
            guardBubble.material.color = new Color(guardBubble.material.color.r, guardBubble.material.color.g, guardBubble.material.color.b, 0f);
        }
        if (parryBubble != null) {
            parryBubble.material.color = new Color(parryBubble.material.color.r, parryBubble.material.color.g, parryBubble.material.color.b, 0.1f * parryFrames);
        }
        if (magicGuard && parryBubble != null && guardBubble!= null) {
            guardBubble.material.color = new Color(guardBubble.material.color.r, guardBubble.material.color.g, guardBubble.material.color.b, 0.2f + (0.01f * guardStunned));
            parryBubble.material.color = new Color(parryBubble.material.color.r, parryBubble.material.color.g, parryBubble.material.color.b, 0.3f * parryFrames);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag.Contains("WalTal")) {
            col.isTrigger = false;
            col.attachedRigidbody.useGravity = true;
            incorporealFrames = 0;
        }
    }
}
