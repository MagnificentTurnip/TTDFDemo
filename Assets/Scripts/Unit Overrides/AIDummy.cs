using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDummy : EnemyAI {

    public AtkStyleWish style;
    public override AtkStyle Style {
        get {
            return style;
        }
        set {
            throw new System.NotImplementedException();
        }
    }
    public Animator animator;

    public enum goalStates { attack, approach, retreat, evade, guard, parry, nothing }; //states for the enemy's current goal
    public enum behaviourStates { defensive, offensive, mixed }; //states for the enemy's current behaviour
    public goalStates goal;
    public behaviourStates behaviour;

    public float attackPref; //preference values for goals
    public float approachPref;
    public float retreatPref;
    public float evadePref;
    public float guardPref;
    public float parryPref;

    public int atkStringLength; //from 1 to 4, how many attacks the dummy wants to perform in the attack string
    public int atkStringCounter; //how many attacks in the string the dummy has performed

    public float advancingBladeworkPref; //preference value for the leap attack
    public float standardBladeworkPref; //preference values for attack strings
    public float lightBladeworkPref;
    public float heavyBladeworkPref;

    public int attackFrames; //frames to count down to an attack
    public int guardFrames; //frames for guarding

    public float retreatDist; //distance to retreat to

    public int evadeDirection; //determines the evade's direction, 1/2/3/4 forward/back/left/right

    public GameObject rgtHandBone; //relevant bone for the attack style

    public override void CalculateGoalPref() {
        attackPref = approachPref = retreatPref = evadePref = guardPref = parryPref = 100; //set all preferences to 100, the base

        //generally wants to go after a different goal than last time
        switch (goal) {
            case goalStates.attack:
                attackPref -= 20;
                break;
            case goalStates.approach:
                approachPref -= 40;
                retreatPref -= 30; //moving back and forth constantly looks kind of weird so don't do that too much
                break;
            case goalStates.retreat:
                retreatPref -= 40;
                approachPref -= 30;
                evadePref -= 30; //evading is also kind of retreating
                break;
            case goalStates.evade:
                evadePref -= 40;
                break;
            case goalStates.guard:
                guardPref -= 60;
                break;
            case goalStates.parry:
                parryPref = 0;
                break;
        }

        //behavioural adjustment
        if (behaviour == behaviourStates.defensive) {
            attackPref -= 30;
            approachPref -= 50;
        }
        if (behaviour == behaviourStates.offensive) {
            guardPref -= 50;
            retreatPref -= 50;
            evadePref -= 30;
        }

        //attacking is good after approaching
        if (goal == goalStates.approach) {
            attackPref += 30;
        }

        //certain options aren't really necessary if enemy is far away
        if (Vector3.Distance(transform.position, target.transform.position) > 20) {
            guardPref -= 50;
            retreatPref -= 50;
            parryPref -= 50;
            attackPref -= 30;
            approachPref += 30;
        }

        //and just don't retreat at all if beyond the maximum retreating distance
        if (Vector3.Distance(transform.position, target.transform.position) >= 28) {
            retreatPref = 0;
        }

        //some multiplicative adjustment to drive more interesting actions
        attackPref *= 1.5f;
        approachPref *= 0.9f;
        retreatPref *= 0.9f;
        evadePref *= 0.5f;
        guardPref *= 0.5f;
        parryPref *= 0.2f; //parrying is just really rare

        //set states to a minimum of 0
        if (attackPref < 0) {
            attackPref = 0;
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

        //if SP is low, don't evade. If MP is low, don't parry.
        if (style.stat.SP < 100) {
            evadePref = 0;
        }
        if (style.stat.MP < 50) {
            parryPref = 0;
        }
    }

    public void CalculateAttackPref() {
        advancingBladeworkPref = standardBladeworkPref = lightBladeworkPref = heavyBladeworkPref = 100; //set preferences to the default

        switch (behaviour) { //certain attacks fit offense better than defense and vice-versa
            case behaviourStates.defensive:
                standardBladeworkPref += 20;
                lightBladeworkPref += 20;
                heavyBladeworkPref -= 20;
                advancingBladeworkPref -= 20;
                break;
            case behaviourStates.offensive:
                heavyBladeworkPref += 30;
                advancingBladeworkPref += 30;
                break;
        }

        //proximity-based preference - try not to use certain moves when you're out of range
        if (Vector3.Distance(transform.position, target.transform.position) < 5) {
            advancingBladeworkPref -= 50;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 10) {
            standardBladeworkPref -= 50;
            lightBladeworkPref -= 50;
            heavyBladeworkPref -= 50;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 20) {
            standardBladeworkPref = 0; //too far away for any of these
            lightBladeworkPref = 0;
            heavyBladeworkPref = 0;
        }

        //set states to a minimum of 0
        if (advancingBladeworkPref < 0) {
            advancingBladeworkPref = 0;
        }
        if (standardBladeworkPref < 0) {
            standardBladeworkPref = 0;
        }
        if (lightBladeworkPref < 0) {
            lightBladeworkPref = 0;
        }
        if (heavyBladeworkPref < 0) {
            heavyBladeworkPref = 0;
        }
    }

    public void ChangeGoal(goalStates inGoal) {
        goal = inGoal;
    }

    public override void ChangeGoal() {
        decider = Random.Range(0, attackPref + approachPref + retreatPref + evadePref + guardPref + parryPref);
        if (decider >= 0 && decider <= attackPref) {
            goal = goalStates.attack;
        }
        else if (decider <= attackPref + approachPref) {
            goal = goalStates.approach;
        }
        else if (decider <= attackPref + approachPref + retreatPref) {
            goal = goalStates.retreat;
        }
        else if (decider <= attackPref + approachPref + retreatPref + evadePref) {
            goal = goalStates.evade;
        }
        else if (decider <= attackPref + approachPref + retreatPref + evadePref + guardPref) {
            goal = goalStates.guard;
        }
        else {
            goal = goalStates.parry;
        }
    }

    public void ChangeBehaviour(behaviourStates inBehaviour) {
        behaviour = inBehaviour;
    }

    public void Attack() {
        if (style.state != AtkStyleWish.attackStates.idle) {
            advancingBladeworkPref = 0;
        }
        decider = Random.Range(0, standardBladeworkPref + lightBladeworkPref + heavyBladeworkPref + advancingBladeworkPref);
        if (decider >= 0 && decider <= standardBladeworkPref) {
            switch (style.state) {
                case AtkStyleWish.attackStates.idle:
                    style.standardBladework1();
                    break;
                case AtkStyleWish.attackStates.bladework1:
                    style.standardBladework2();
                    break;
                case AtkStyleWish.attackStates.bladework2:
                    style.standardBladework3();
                    break;
                case AtkStyleWish.attackStates.bladework3:
                    style.standardBladework4();
                    break;
            }
        }
        else if (decider <= standardBladeworkPref + lightBladeworkPref) {
            switch (style.state) {
                case AtkStyleWish.attackStates.idle:
                    style.lightBladework1();
                    break;
                case AtkStyleWish.attackStates.bladework1:
                    style.lightBladework2();
                    break;
                case AtkStyleWish.attackStates.bladework2:
                    style.lightBladework3();
                    break;
                case AtkStyleWish.attackStates.bladework3:
                    style.lightBladework4();
                    break;
            }
        }
        else if (decider <= standardBladeworkPref + lightBladeworkPref + heavyBladeworkPref) {
            switch (style.state) {
                case AtkStyleWish.attackStates.idle:
                    style.heavyBladework1();
                    break;
                case AtkStyleWish.attackStates.bladework1:
                    style.heavyBladework2();
                    break;
                case AtkStyleWish.attackStates.bladework2:
                    style.heavyBladework3();
                    break;
                case AtkStyleWish.attackStates.bladework3:
                    style.heavyBladework4();
                    break;
            }
        }
        else {
            style.advancingBladework();
        }
        atkStringCounter++;
    }


    // Use this for initialization
    void Start () {
        SelectTarget(target);
        behaviour = (behaviourStates)Random.Range(0, 2);
	}

    public override void FixedUpdate() {

        if (style.status.slain || style.status.unconscious) {
            pointTargetFrames = 0;
            attackFrames = 0;
            guardFrames = 0;
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
            goal = goalStates.nothing;
        }

        if (navMesh.velocity.magnitude >= navMesh.speed - 5) { //sprinting
            style.status.sprinting = true;
        } else {
            style.status.sprinting = false;
        }

        if (goalDelay > 0) {
            goalDelay--;
        }

        goalCounter--;

        if (pointTargetFrames > 0) {
            pointTargetFrames--;
            movementAI.pointTowardTarget(turnSpeed);
        }

        if (attackFrames > 0) {
            attackFrames--;
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
            if (movementAI.toTargetAngle < 10f && movementAI.toTargetAngle > -10f && attackFrames > 2) {
                attackFrames = 2;
            }
        }

        if (guardFrames > 0) {
            guardFrames--;
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
        }

        if (style.status.isFloored() && !(style.status.slain || style.status.unconscious)) {
            ChangeGoal(goalStates.evade);
            goalStarted = false;
            goalDelay = Random.Range(0, 60);
        }

        if ((goalCounter <= 0 || goalComplete) && goalDelay <= 0 && !(style.status.slain || style.status.unconscious)) { //goals change every 10 seconds or every time a goal is completed
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
            CalculateGoalPref();
            ChangeGoal();
            goalCounter = 600;
            goalComplete = false;
            goalStarted = false;
        }

        if ((!goalStarted) && goalDelay <= 0) {
            switch (goal) {
                case goalStates.attack:
                    atkStringCounter = 0;
                    atkStringLength = Random.Range(1, 4);
                    attackFrames = 30;
                    goalStarted = true;
                    break;
                case goalStates.approach:
                    if (style.status.canMove()) {
                        navMesh.enabled = true;
                        if (navMesh.enabled) {
                            navMesh.isStopped = false;
                            navMesh.SetDestination(target.transform.position);
                            navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        }
                        goalStarted = true;
                    }
                    break;
                case goalStates.retreat:
                    if (style.status.canMove()) {
                        navMesh.enabled = true;
                        if (navMesh.enabled) {
                            navMesh.isStopped = false;
                            retreatDist = Random.Range(Vector3.Distance(transform.position, target.transform.position), 28f);
                            navMesh.SetDestination(transform.position + Vector3.Normalize(transform.position - target.transform.position) * retreatDist);
                            navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        }
                        goalStarted = true;
                    }
                    break;
                case goalStates.evade:
                    pointTargetFrames = 1;
                    goalStarted = true;
                    break;
                case goalStates.guard:
                    guardFrames = Random.Range(20, 60); //guard for between .33 and 1 second
                    goalStarted = true;
                    break;
                case goalStates.parry:
                    if (style.status.canParry()) {
                        movementAI.pointTowardTarget(60);
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
        }


        if (goalStarted && !goalComplete) {
            switch (goal) {
                case goalStates.attack:
                    if (attackFrames > 1) {
                        movementAI.pointTowardTarget(12);
                    }
                    else {
                        if (style.status.canAttack()) {
                            CalculateAttackPref();
                            Attack();
                        }
                        if (atkStringCounter >= atkStringLength) {
                            goalComplete = true; //once the enemy is finished with its attack string attacked, the goal is complete
                        }
                    }
                    break;
                case goalStates.approach:
                    if (style.status.canMove()) {
                        navMesh.enabled = true;
                        if (navMesh.enabled) {
                            navMesh.isStopped = false;
                        }
                    }
                    else {
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                    }
                    if (Vector3.Distance(transform.position, target.transform.position) < 4 || Vector3.Distance(transform.position, navMesh.destination) < 4) {
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                        goalComplete = true; //if the target is more-or-less reached, goal complete
                    }
                    else if (goalCounter < 180 || navMesh.isPathStale/* || navMesh.pathStatus != NavMeshPathStatus.PathComplete*/) { //this goal can potentially be recognised as a failure, in which case try to salvage
                        print("abortApproach");
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                        goalStarted = false;
                        CalculateGoalPref();
                        approachPref = 0; //welp, messed up last time
                        retreatPref = 10; //prolly don't want to retreat that much
                        ChangeGoal();
                        goalCounter = 600;
                    }
                    break;
                case goalStates.retreat:
                    if (style.status.canMove()) {
                        navMesh.enabled = true;
                        if (navMesh.enabled) {
                            navMesh.isStopped = false;
                        }
                    }
                    else {
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                    }
                    if (Vector3.Distance(transform.position, target.transform.position) > 25 || Vector3.Distance(transform.position, navMesh.destination) < 4) {
                        print("distReached");
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                        goalComplete = true; //similar to approach but the wolf has to be far away
                    }
                    else if (goalCounter < 150 || navMesh.isPathStale || navMesh.pathStatus != NavMeshPathStatus.PathComplete) { //this goal can potentially be recognised as a failure, in which case try to salvage
                        print("abortRetreat");
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                        CalculateGoalPref();
                        approachPref = 10; //why approach when you can't even get away?
                        retreatPref = 10; //why retreat when you already know you can't? Might have just messed up on distance so fair
                        guardPref += 20;//prefer guard
                        evadePref = 10; //low chance of evading since that's often backwards
                        ChangeGoal();
                        goalCounter = 600;
                        goalStarted = false;
                    }
                    break;
                case goalStates.evade:
                    if (style.status.canRoll()) {
                        if (Vector3.Distance(transform.position, target.transform.position) < 6) { //don't evade forwards if close
                            evadeDirection = Random.Range(2, 4);
                        }
                        else {
                            evadeDirection = Random.Range(1, 4);
                        }
                        if (evadeDirection == 1) {
                            style.evadeForward();
                        }
                        else if (evadeDirection == 2) {
                            style.evadeBack();
                        }
                        else if (evadeDirection == 3) {
                            style.evadeLeft();
                        }
                        else {
                            style.evadeRight();
                        }
                        goalDelay = 30;
                        goalComplete = true; //evade complete
                    }
                    break;
                case goalStates.guard:
                    if (guardFrames <= 0) {
                        goalComplete = true;
                        style.status.guarding = false;
                    }
                    else {
                        if (style.status.canGuard()) {
                            style.status.guarding = true;
                            if (!style.status.isGuardStunned()) {
                                movementAI.pointTowardTarget(3);
                            }
                        }
                        else {
                            style.status.guarding = false;
                        }
                    }
                    break;
                case goalStates.parry:
                    goalComplete = true; //parry happens instantaneously so it autocompletes
                    break;
            }


        }


        if (!style.status.canGuard()) {
            style.status.guarding = false;
        }

        if ((!style.status.canMove())) {
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
        }

    }

    // Update is called once per frame
    void Update () {
        animator.SetFloat("speedPercent", navMesh.velocity.magnitude / (navMesh.speed), .1f, Time.deltaTime);
    }
}
