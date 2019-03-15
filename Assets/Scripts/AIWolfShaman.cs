using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWolfShaman : MonoBehaviour {

    public enum goalStates {attack, cast, approach, retreat, evade, guard, parry};
    public enum behaviourStates {defensive, offensive, mixed, final};
    public goalStates goal;
    public behaviourStates behaviour;
    public float decider;

    public int goalCounter;
    public int behaviourCounter;

    public bool goalStarted;
    public bool goalComplete;

    public float attackPref;
    public float castPref;
    public float approachPref;
    public float retreatPref;
    public float evadePref;
    public float guardPref;
    public float parryPref;

    public float defensivePref;
    public float offensivePref;
    public float mixedPref;

    public float quickBitePref;
    public float forwardBitePref;
    public float lSwipePref;
    public float rSwipePref;
    public float pouncePref;

    public AtkStyleWolf style;

    public NavMeshAgent navMesh;

    public GameObject target;
    private StatusManager targetStatus;
    private StatSheet targetStat;

    public bool big;

    public BoxCollider hitBox;
    public GameObject bigWolfGFX;
    public GameObject smallWolfGFX;

    public int sizeCounter;

    public GameObject rgtPawBoneS;
    public GameObject lftPawBoneS;
    public GameObject jawBoneS;
    public GameObject rgtPawBoneL;
    public GameObject lftPawBoneL;
    public GameObject jawBoneL;

    public void SelectTarget(GameObject newTarget) {
        target = newTarget;
        targetStatus = target.GetComponent<StatusManager>();
        targetStat = target.GetComponent<StatSheet>();
    }

    public void BecomeBig() {
        big = true;
        style.rgtPawBone = rgtPawBoneL;
        style.lftPawBone = lftPawBoneL;
        style.jawBone = jawBoneL;
        bigWolfGFX.SetActive(true);
        smallWolfGFX.SetActive(false);

    }

    public void BecomeSmall() {
        big = false;
        style.rgtPawBone = rgtPawBoneS;
        style.lftPawBone = lftPawBoneS;
        style.jawBone = jawBoneS;
        bigWolfGFX.SetActive(false);
        smallWolfGFX.SetActive(true);
        hitBox.center.Set(0, 1.2f, 0);
        hitBox.size.Set(1, 2.4f, 3.2f);
    }

    public void CalculateGoalPref() {
        attackPref = castPref = approachPref = retreatPref = evadePref = guardPref = parryPref = 100; //set all preferences to 100, the base

        //generally wants to go after a different goal than last time
        switch (goal) {
            case goalStates.attack:
                attackPref -= 20;
                break;
            case goalStates.cast:
                castPref -= 50;
                break;
            case goalStates.approach:
                approachPref -= 40;
                break;
            case goalStates.retreat:
                retreatPref -= 40;
                break;
            case goalStates.evade:
                evadePref -= 20;
                break;
            case goalStates.guard:
                guardPref -= 40;
                break;
            case goalStates.parry:
                parryPref = 0;
                break;
        }

        //behavioural adjustment
        if (behaviour == behaviourStates.defensive) {
            attackPref -= 30;
            castPref -= 50;
            approachPref -= 50;
        }
        if (behaviour == behaviourStates.offensive) {
            guardPref -= 50;
            retreatPref -= 50;
            evadePref -= 50;
        }

        //if retreat was last, then it might be a good idea to cast. Otherwise it probably isn't.
        if (goal == goalStates.retreat) {
            castPref += 30;
        } else {
            castPref -= 30;
        }

        //similar stuff for approaching - attacking is good after approaching, casting isn't.
        if (goal == goalStates.approach) {
            attackPref += 30;
            castPref -= 30;
        }

        //certain defensive options aren't really necessary if enemy is far away
        if (Vector3.Distance(transform.position, target.transform.position) > 20) {
            guardPref -= 50;
            retreatPref -= 50;
            parryPref -= 50;
            approachPref += 30;
            castPref += 30;
        }

        //alter preference based on state of the player (opponent)
        if (targetStatus.isVulnerable()) {
            attackPref += 30;
            approachPref += 30;
        }
        if (targetStatus.isStunned()) {
            guardPref -= 50;
            parryPref -= 50;
            castPref += 30;
        }

        //some multiplicative adjustment to drive more interesting actions
        attackPref *= 1.5f;
        //castPref isn't altered
        approachPref *= 0.9f;
        retreatPref *= 0.9f;
        evadePref *= 0.8f;
        guardPref *= 0.6f;
        parryPref *= 0.2f; //parrying is just really rare

        //set states to a minimum of 0
        if (attackPref < 0) {
            attackPref = 0;
        }
        if (castPref < 0) {
            castPref = 0;
        }
        if (approachPref < 0) {
            approachPref = 0;
        }
        if (retreatPref < 0) {
            retreatPref = 0;
        }
        if (evadePref < 0) {
            evadePref = 0;
        }
        if (guardPref < 0) {
            guardPref = 0;
        }
        if (parryPref < 0) {
            parryPref = 0;
        }
    }

    public void CalculateBehaviourPref() {
        defensivePref = offensivePref = mixedPref = 100; //set preferences to the default

        if (style.stat.HP < style.stat.MaxHP / 2 && targetStat.HP > targetStat.MaxHP / 1.5) { //prefers defense when on the back foot
            defensivePref += 30;
        }

        if (style.stat.HP > style.stat.MaxHP / 1.5 && targetStat.HP < targetStat.MaxHP / 2) { //prefers offense when at an HP lead
            offensivePref += 30;
        }

        switch (behaviour) { //generally wants to switch behaviour if this is called, but won't necessarily always do so
            case behaviourStates.defensive:
                defensivePref -= 50;
                break;
            case behaviourStates.offensive:
                offensivePref -= 50;
                break;
            case behaviourStates.mixed:
                mixedPref -= 50;
                break;
        }

    }

    public void CalculateAttackPref() {
        quickBitePref = forwardBitePref = lSwipePref = rSwipePref = pouncePref = 100; //set preferences to the default

        switch (behaviour) { //certain attacks fit offense better than defense and vice-versa
            case behaviourStates.defensive:
                quickBitePref += 20;
                lSwipePref += 20;
                rSwipePref += 20;
                break;
            case behaviourStates.offensive:
                forwardBitePref += 20;
                pouncePref += 20;
                break;
        }


    }


    public void ChangeGoal(goalStates inGoal) {
        goal = inGoal;
    }

    public void ChangeGoal() {
        decider = Random.Range(0, attackPref+castPref+approachPref+retreatPref+evadePref+guardPref+parryPref);
        if (decider >= 0 && decider <= attackPref) {
            goal = goalStates.attack;
        }
        else if (decider <= attackPref + castPref) {
            goal = goalStates.cast;
        }
        else if (decider <= attackPref + castPref + approachPref) {
            goal = goalStates.approach;
        }
        else if (decider <= attackPref + castPref + approachPref + retreatPref) {
            goal = goalStates.retreat;
        }
        else if (decider <= attackPref + castPref + approachPref + retreatPref + evadePref) {
            goal = goalStates.evade;
        }
        else if (decider <= attackPref + castPref + approachPref + retreatPref + evadePref + guardPref) {
            goal = goalStates.guard;
        }
        else {
            goal = goalStates.parry;
        }
    }

    public void ChangeBehaviour(behaviourStates inBehaviour) {
        behaviour = inBehaviour;
    }

    public void ChangeBehaviour() {
        decider = Random.Range(0, defensivePref + offensivePref + mixedPref);
        if (decider >= 0 && decider <= defensivePref) {
            behaviour = behaviourStates.defensive;
        } else if (decider <= defensivePref + offensivePref) {
            behaviour = behaviourStates.offensive;
        } else {
            behaviour = behaviourStates.mixed;
        }
    }

    public void Attack() {
        decider = Random.Range(0, quickBitePref + forwardBitePref + lSwipePref + rSwipePref + pouncePref);
        if (decider >= 0 && decider <= quickBitePref) {
            style.quickBite();
        }
        else if (decider <= quickBitePref + forwardBitePref) {
            if (big) {
                style.forwardBite(2);
            } else {
                style.forwardBite(0);
            }
        }
        else if (decider <= quickBitePref + forwardBitePref + lSwipePref) {
            if (big) {
                style.lSwipe(2);
            }
            else {
                style.lSwipe(0);
            }
        }
        else if (decider <= quickBitePref + forwardBitePref + lSwipePref + rSwipePref) {
            if (big) {
                style.rSwipe(2);
            }
            else {
                style.rSwipe(0);
            }
        }
        else {
            if (big) {
                style.pounce(3);
            }
            else {
                style.pounce(2);
            }
        }
    }


    // Use this for initialization
    void Start () {
        targetStatus = target.GetComponent<StatusManager>();
        targetStat = target.GetComponent<StatSheet>();

        goal = goalStates.cast;
        behaviour = behaviourStates.mixed;
        goalComplete = false;
        goalStarted = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // FixedUpdate is called once per physics frame
    private void FixedUpdate() {
        
        behaviourCounter--;
        goalCounter--;

        if (behaviourCounter <= 0) { //behaviour changes every 30 seconds
            CalculateBehaviourPref();
            ChangeBehaviour();
            behaviourCounter = 1800;
        }

        if (goalCounter <= 0 || goalComplete) { //goals change every 10 seconds or every time a goal is completed
            CalculateGoalPref();
            ChangeGoal();
            goalCounter = 600;
            goalComplete = false;
            goalStarted = false;
        }

        if (!goalStarted) {
            switch (goal) {
                case goalStates.attack:
                    if (style.status.canAttack()) {
                        transform.LookAt(target.transform, Vector3.up); //face the target
                        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //lock rotation on x and z
                        CalculateAttackPref();
                        Attack();
                        goalStarted = true;
                    }
                    break;
                case goalStates.cast:
                    if (style.status.canCast()) { 
                        goalStarted = true; //WOLFO NEEDS A SPELLBOOK
                    }
                    break;
                case goalStates.approach:
                    if (style.status.canMove()) {
                        navMesh.SetDestination(target.transform.position + (Vector3.Normalize(transform.position - target.transform.position) * 3));
                        navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        goalStarted = true;
                    }
                    break;
                case goalStates.retreat:
                    if (style.status.canMove()) {
                        navMesh.SetDestination(new Vector3(0, 0, 0));
                        navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        goalStarted = true;
                    }
                    break;
                case goalStates.evade:
                    if (style.status.canRoll()) {
                        transform.LookAt(target.transform, Vector3.up); //face the target
                        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y + Random.Range(30, 330), 0); //lock rotation on x and z and add turn around a lot
                        style.movement.evade(3000, 0, 0.5f);
                        goalStarted = true;
                    }
                    break;
                case goalStates.guard:
                    if (style.status.canGuard()) {
                        transform.LookAt(target.transform, Vector3.up); //face the target
                        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //lock rotation on x and z
                        style.status.guardStunned = Random.Range(60, 150); //guard for between 1 and 2.5 seconds
                        goalStarted = true;
                    }
                    break;
                case goalStates.parry:
                    if (style.status.canParry()) {
                        transform.LookAt(target.transform, Vector3.up); //face the target
                        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //lock rotation on x and z
                        decider = Random.Range(0, 2);
                        if (decider > 1) {
                            style.fParry();
                        }
                        else {
                            style.bParry();
                        }
                        goalStarted = true;
                    }
                    break;
            }
        } else {
            switch (goal) {
                case goalStates.attack:
                    if (style.instantiatedAttacks.Count > 0) {
                        goalComplete = true; //once an attack has come out, the goal is completed
                    }
                    break;
                case goalStates.cast:
                    goalComplete = true; //WOLFO NEEDS A SPELLBOOK
                    break;
                case goalStates.approach:
                    if (Vector3.Distance(transform.position, navMesh.destination) < 3) { //if the destination is more-or-less reached, goal complete
                        goalComplete = true;
                    }
                    break;
                case goalStates.retreat:
                    if (Vector3.Distance(transform.position, navMesh.destination) < 3) { //same as approach, the destination itself is the only difference
                        goalComplete = true;
                    }
                    break;
                case goalStates.evade:
                    goalComplete = true; //evade happens instantaneously so it autocompletes
                    break;
                case goalStates.guard:
                    if (!style.status.isGuardStunned()) { //this AI makes guard work off of guardStunned so it'll be active for a fixed amount of time
                        goalComplete = true;
                    }
                    break;
                case goalStates.parry:
                    goalComplete = true; //parry happens instantaneously so it autocompletes
                    break;
            }
        }

    }
}
