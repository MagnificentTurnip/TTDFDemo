using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWolfShaman : MonoBehaviour {

    public enum goalStates {attack, cast, big, approach, retreat, evade, guard, parry}; //states for the wolf's current goal
    public enum behaviourStates {defensive, offensive, mixed, defeated, pacified}; //states for the wolf's current behaviour
    public goalStates goal; 
    public behaviourStates behaviour;
    public float decider; //a variable that holds a random value that decides which option to cohose

    public int goalCounter; //tracks the time spent in the current goal
    public int behaviourCounter; //tracks the time spent in the current behaviour

    public bool goalStarted; //whether the current goal's action has started
    public bool goalComplete; //whether the current goal is complete
    public int goalDelay; //an int that, while it is above zero, ticks down each frame and prevents certain goal conditions

    public float attackPref; //preference values for goals
    public float castPref;
    public float bigPref;
    public float approachPref;
    public float retreatPref;
    public float evadePref;
    public float guardPref;
    public float parryPref;

    public float defensivePref; //preference values for behaviours
    public float offensivePref;
    public float mixedPref; //defeated and pacified are left out because they are manually set and are not based on preference

    public float quickBitePref; //preference values for attacks
    public float forwardBitePref;
    public float lSwipePref;
    public float rSwipePref;
    public float pouncePref;

    public AtkStyleWolf style; //the attack style
    public CCResistHittable ccres; //the hittable script for this unit
    public NavMeshAgent navMesh; //the navmeshagent for this unit

    public bool big; //whether or not the wolf is in its big astral-projection-y form
    public int bigCounter; //how long the wolf will be big

    public Animator smallAnimator; //animators for the small(normal) and big forms of the wolf
    public Animator bigAnimator;

    public float bigStopDistance; //stopping distances for both big and small forms
    public float smallStopDistance;

    public GameObject target; //the curent target of the wolf, along with its status and statsheet
    private StatusManager targetStatus;
    private StatSheet targetStat;

    public int pointTargetFrames; //when above zero, this will tick down - for every frame it is above zero, the wolf will point toward its target

    public BoxCollider hitBox; //the hitbox of the wolf
    public GameObject bigWolfGFX; //and the objects holding its graphical representation for both big and small forms
    public GameObject smallWolfGFX;

    public GameObject rgtPawBoneS; //relevant bones for plopping into the attack style, right and left paw, jaw and chest for both big and small forms
    public GameObject lftPawBoneS;
    public GameObject jawBoneS;
    public GameObject chestS;
    public GameObject rgtPawBoneL;
    public GameObject lftPawBoneL;
    public GameObject jawBoneL;
    public GameObject chestL;

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
        style.chest = chestL;
        bigWolfGFX.SetActive(true);
        style.stat.Level = 10;
        hitBox.center = new Vector3(0, 4, -0.5f);
        hitBox.size = new Vector3(3.3f, 8, 9);
        ccres.flinchLimit = 5;
        ccres.slashingTaken = 0.5f;
        ccres.impactTaken = 0.5f;
        ccres.piercingTaken = 0.5f;
        ccres.fireTaken = 0.75f;
        ccres.coldTaken = 0.75f;
        ccres.causticTaken = 0.5f;
        style.animator = style.movement.animator = style.status.animator = bigAnimator;
    }

    public void BecomeSmall() {
        big = false;
        style.rgtPawBone = rgtPawBoneS;
        style.lftPawBone = lftPawBoneS;
        style.jawBone = jawBoneS;
        style.chest = chestS;
        bigWolfGFX.SetActive(false);
        style.stat.Level = 2;
        hitBox.center = new Vector3(0, 1.2f, 0);
        hitBox.size = new Vector3(1, 2.4f, 3.2f);
        ccres.flinchLimit = 1;
        ccres.slashingTaken = 1f;
        ccres.impactTaken = 1f;
        ccres.piercingTaken = 1f;
        ccres.fireTaken = 1f;
        ccres.coldTaken = 1f;
        ccres.causticTaken = 1f;
        style.animator = style.movement.animator = style.status.animator = smallAnimator;
    }

    public void CalculateGoalPref() {
        attackPref = castPref = bigPref = approachPref = retreatPref = evadePref = guardPref = parryPref = 100; //set all preferences to 100, the base

        //generally wants to go after a different goal than last time
        switch (goal) {
            case goalStates.attack:
                attackPref -= 20;
                break;
            case goalStates.cast:
                castPref -= 50;
                break; //bigPref not included because if it was the last thing then you're already big so you can't go big again because you already are
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
            bigPref += 30;
        }

        //if MP is under half then have a little bit of aversion to casting, parrying and going big
        if (style.stat.MP < 250) {
            castPref -= 20;
            parryPref -= 20;
            bigPref -= 30;
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
        bigPref *= 0.5f;
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
        if (bigPref < 0) {
            bigPref = 0;
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

        //if you're already big then you don't need to go big
        if (big) {
            bigPref = 0;
        }

        //if SP is low, don't evade. If MP is low, don't parry or cast, and be averse to going Big
        if (style.stat.SP < 100) {
            evadePref = 0;
        }
        if (style.stat.MP < 100) {
            castPref = 0;
            parryPref = 0;
            bigPref = 0;
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

        if (big) { //favour going big if currently offensive
            bigPref += 30;
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

        //size-based preference - the wolf doesn't like swipes so much when she isn't big but prefers them when she is
        if (!big) {
            lSwipePref -= 50;
            rSwipePref -= 50;
        } else {
            lSwipePref += 30;
            rSwipePref += 30;
        }

        //proximity-based preference - try not to use certain moves when you're out of range
        if (Vector3.Distance(transform.position, target.transform.position) < 3) {
            quickBitePref += 20;
            pouncePref -= 100;
            forwardBitePref -= 50;
        }
        if (Vector3.Distance(transform.position, target.transform.position) < 6) {
            pouncePref -= 80;
            forwardBitePref -= 20;
            lSwipePref += 20;
            rSwipePref += 20;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 6) {
            pouncePref += 20;
            forwardBitePref += 20;
            quickBitePref -= 80;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 10) {
            pouncePref += 50;
            forwardBitePref -= 80;
            quickBitePref -= 150;
            lSwipePref -= 100;
            rSwipePref -= 100;
        }

        //set states to a minimum of 0
        if (quickBitePref < 0) {
            quickBitePref = 0;
        }
        if (lSwipePref < 0) {
            lSwipePref = 0;
        }
        if (rSwipePref < 0) {
            rSwipePref = 0;
        }
        if (forwardBitePref < 0) {
            forwardBitePref = 0;
        }
        if (pouncePref < 0) {
            pouncePref = 0;
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
        else if (decider <= attackPref + castPref + bigPref) {
            goal = goalStates.big;
            goalDelay = 30;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref) {
            goal = goalStates.approach;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref + retreatPref) {
            goal = goalStates.retreat;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref + retreatPref + evadePref) {
            goal = goalStates.evade;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref + retreatPref + evadePref + guardPref) {
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
            pointTargetFrames = 80;
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
        navMesh.isStopped = true;
    }
	
	// Update is called once per frame
	void Update () {
        //set speedPercent for both big and small animators
        bigAnimator.SetFloat("speedPercent", navMesh.velocity.magnitude / (navMesh.speed), .1f, Time.deltaTime);
        smallAnimator.SetFloat("speedPercent", navMesh.velocity.magnitude / (navMesh.speed), .1f, Time.deltaTime);
    }

    // FixedUpdate is called once per physics frame
    private void FixedUpdate() {

        print(goal);
        
        if (big) {
            bigCounter--;
            style.stat.MP -= (style.stat.MPregen + 10) / 60;
            if (bigCounter <= 0 || style.stat.MP <= 0) {
                BecomeSmall();
            }
        }

        if (goalDelay > 0) {
            goalDelay--;
        }

        if (pointTargetFrames > 0) {
            pointTargetFrames--;
            transform.LookAt(target.transform);
        }

        behaviourCounter--;
        goalCounter--;

        if (behaviourCounter <= 0) { //behaviour changes every 30 seconds
            CalculateBehaviourPref();
            ChangeBehaviour();
            behaviourCounter = 1800;
        }

        if (goalCounter <= 0 || goalComplete) { //goals change every 10 seconds or every time a goal is completed
            navMesh.isStopped = true;
            CalculateGoalPref();
            ChangeGoal();
            goalCounter = 600;
            goalComplete = false;
            goalStarted = false;
        }

        if (style.status.isFloored()) {
            ChangeGoal(goalStates.evade);
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
                        goalDelay = 30;
                    }
                    break;
                case goalStates.cast:
                    if (style.status.canCast()) { 
                        goalStarted = true; //WOLFO NEEDS A SPELLBOOK
                        goalDelay = 30;
                    }
                    break;
                case goalStates.big:
                    if (style.status.canCast() && goalDelay <= 0) {
                        goalStarted = true;
                        BecomeBig();
                        bigCounter = Random.Range(10*60, 30*60); //become big for anywhere between 10 and 30 seconds
                    }
                    break;
                case goalStates.approach:
                    if (style.status.canMove()) {
                        navMesh.isStopped = false;
                        navMesh.SetDestination(target.transform.position);
                        navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        goalStarted = true;
                    }
                    break;
                case goalStates.retreat:
                    if (style.status.canMove()) {
                        navMesh.isStopped = false;
                        navMesh.SetDestination(transform.position + Vector3.Normalize(transform.position - target.transform.position) * 25);
                        navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        goalStarted = true;
                    }
                    break;
                case goalStates.evade:
                    if (style.status.canRoll()) {
                        transform.LookAt(target.transform, Vector3.up); //face the target
                        transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y + Random.Range(30, 330), 0); //lock rotation on x and z and add turn around a lot
                        style.movement.evade(2000, 0, 0.5f);
                        bigAnimator.Play("jump");
                        smallAnimator.Play("jump");
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
                    if (style.status.canAttack()) {
                        goalComplete = true; //once the boss can attack, the goal is complete
                    }
                    break;
                case goalStates.cast:
                    goalComplete = true; //WOLFO NEEDS A SPELLBOOK
                    break;
                case goalStates.big:
                    goalComplete = true; //becoming big happens instantaneously so the goal is completed
                    break;
                case goalStates.approach: 
                    if (style.status.canMove()) {
                        navMesh.isStopped = false;
                    } else {
                        navMesh.isStopped = true;
                    }
                    if ((big && Vector3.Distance(transform.position, target.transform.position) < 8) || (!big && Vector3.Distance(transform.position, target.transform.position) < 4) || Vector3.Distance(transform.position, navMesh.destination) < 4) {
                        if (big) {
                            print("8");
                        } else {
                            print("4");
                        }
                        navMesh.isStopped = true;
                        goalComplete = true; //if the target is more-or-less reached, goal complete
                    }
                    break;
                case goalStates.retreat:
                    if (style.status.canMove()) {
                        navMesh.isStopped = false;
                    }
                    else {
                        navMesh.isStopped = true;
                    }
                    if (Vector3.Distance(transform.position, target.transform.position) > 25 || Vector3.Distance(transform.position, navMesh.destination) < 4) {
                        print("distReached");
                        navMesh.isStopped = true;
                        goalComplete = true; //similar to approach but the wolf has to be far away
                    } else if (goalCounter < 200) { //this goal can potentially be recognised as a failure, in which case try to salvage
                        print("abortRetreat");
                        navMesh.isStopped = true;
                        CalculateGoalPref();
                        castPref = 1; //very low chance of casting
                        approachPref = 0; //why approach when you can't even get away?
                        guardPref += 20;//prefer guard and evade
                        evadePref += 20;
                        ChangeGoal();
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
