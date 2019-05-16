using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWolfShaman : MonoBehaviour {

    public enum GoalStates {attack, cast, big, approach, retreat, evade, guard, parry, nothing}; //states for the wolf's current goal
    public enum BehaviourStates {preBattle, defensive, offensive, mixed, defeated, pacified}; //states for the wolf's current behaviour
    public GoalStates goal; 
    public BehaviourStates behaviour;
    public float decider; //a variable that holds a random value that decides which option to choose

    public float maxSpeed;

    public int goalCounter; //tracks the time spent in the current goal
    public int behaviourCounter; //tracks the time spent in the current behaviour
    public int pacifiedCounter; //tracks progress to pacification

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

    public AIMovement movementAI;
    public AtkStyleWolf style; //the attack style
    public CCResistHittable ccres; //the hittable script for this unit
    public NavMeshAgent navMesh; //the navmeshagent for this unit

    public bool big; //whether or not the wolf is in its big astral-projection-y form
    public int bigCounter; //how long the wolf will be big

    public Animator smallAnimator; //animators for the small(normal) and big forms of the wolf
    public Animator bigAnimator;

    public float bigStopDistance; //stopping distances for both big and small forms
    public float smallStopDistance;

    public float evadeSpeed;
    public float retreatDist;

    public GameObject stump;

    public GameObject target; //the curent target of the wolf, along with its status and statsheet
    private StatusManager targetStatus;
    private StatSheet targetStat;

    //defensive bubbles
    public MeshRenderer guardBubbleS;
    public MeshRenderer parryBubbleS;
    public MeshRenderer guardBubbleL;
    public MeshRenderer parryBubbleL;

    //spellcasting things
    public List<GameObject> spellsKnown;
    public List<GameObject> runningSpells;
    public Spell runningSpellScript;
    public Spell currentSpell;
    public int chosenSpellNumber;
    public string currentSpellName;
    public GameObject toDestroy;
    public bool repeatSpell; //bool that checks for a repeated spell - no copies of the same spell existing at a time, thanks

    public GameObject healingSanctuary; //a bonus spell
    public bool castSanctuary;

    public int pointTargetFrames; //when above zero, this will tick down - for every frame it is above zero, the wolf will point toward its target
    public int attackFrames; //frames to count down to an attack
    public int evadeFrames; //frames to count down to an evade
    public int guardFrames; //frames for guarding

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

    public AudioSource source;

    //audio clips
    public AudioClip howlClipShort;
    public AudioClip howlClipLong;
    public AudioClip whimperClip;
    public AudioClip jumpClip;
    public AudioClip moveClip;

    public AudioClip bossMusic;

    public void SelectTarget(GameObject newTarget) {
        target = newTarget;
        movementAI.target = target;
        targetStatus = target.GetComponent<StatusManager>();
        targetStat = target.GetComponent<StatSheet>();
    }

    public void StopCasting() {
        for (int i = runningSpells.Count - 1; i >= 0; i--) {
            if (runningSpells[i] != null) {
                if (runningSpells[i].GetComponent<Spell>().castTime > 0) {
                    toDestroy = runningSpells[i];
                    runningSpells.Remove(runningSpells[i]);
                    toDestroy.GetComponent<Spell>().DestroySpell();
                }
            }
        }
    }

    public void BecomeBig() {
        navMesh.speed = maxSpeed;
        navMesh.radius = 10;
        big = true;
        evadeSpeed = 3000;
        style.status.guardBubble = guardBubbleL;
        style.status.parryBubble = parryBubbleL;
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
        bigAnimator.Play("howl", 0, 0f);
        smallAnimator.Play("howl", 0, 0f);
    }

    public void BecomeSmall() {
        navMesh.speed = maxSpeed;
        navMesh.radius = 5;
        big = false;
        evadeSpeed = 2000;
        style.status.guardBubble = guardBubbleS;
        style.status.parryBubble = parryBubbleS;
        style.rgtPawBone = rgtPawBoneS;
        style.lftPawBone = lftPawBoneS;
        style.jawBone = jawBoneS;
        style.chest = chestS;
        bigWolfGFX.SetActive(false);
        style.stat.Level = 2;
        hitBox.center = new Vector3(0, 1.2f, 0);
        hitBox.size = new Vector3(1, 2.4f, 3.2f);
        ccres.flinchLimit = 0;
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
            case GoalStates.attack:
                attackPref -= 20;
                break;
            case GoalStates.cast:
                castPref -= 50;
                break; //bigPref not included because if it was the last thing then you're already big so you can't go big again because you already are
            case GoalStates.approach:
                approachPref -= 40;
                retreatPref -= 30; //moving back and forth constantly looks kind of weird so don't do that too much
                break;
            case GoalStates.retreat:
                retreatPref -= 40;
                approachPref -= 30;
                evadePref -= 40; //evading is also kind of retreating
                break;
            case GoalStates.evade:
                evadePref -= 40;
                break;
            case GoalStates.guard:
                guardPref -= 40;
                break;
            case GoalStates.parry:
                parryPref = 0;
                break;
        }

        //behavioural adjustment
        if (behaviour == BehaviourStates.defensive) {
            attackPref -= 30;
            castPref -= 50;
            approachPref -= 50;
        }
        if (behaviour == BehaviourStates.offensive) {
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
        if (goal == GoalStates.retreat) {
            castPref += 30;
        } else {
            castPref -= 30;
        }

        //similar stuff for approaching - attacking is good after approaching, casting isn't.
        if (goal == GoalStates.approach) {
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

        //and just don't retreat at all if beyond the maximum retreating distance
        if (Vector3.Distance(transform.position, target.transform.position) >= 28) {
            retreatPref = 0;
        }

        //alter preference based on state of the player (opponent)
        if (targetStatus.IsVulnerable()) {
            attackPref += 30;
            approachPref += 30;
        }
        if (targetStatus.IsStunned()) {
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
        evadePref *= 0.7f;
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
            case BehaviourStates.defensive:
                defensivePref -= 50;
                break;
            case BehaviourStates.offensive:
                offensivePref -= 50;
                break;
            case BehaviourStates.mixed:
                mixedPref -= 50;
                break;
        }

    }

    public void CalculateAttackPref() {
        quickBitePref = forwardBitePref = lSwipePref = rSwipePref = pouncePref = 100; //set preferences to the default

        switch (behaviour) { //certain attacks fit offense better than defense and vice-versa
            case BehaviourStates.defensive:
                quickBitePref += 40;
                lSwipePref += 20;
                rSwipePref += 20;
                break;
            case BehaviourStates.offensive:
                forwardBitePref += 20;
                pouncePref += 20;
                break;
        }

        //size-based preference - the wolf doesn't like swipes so much when she isn't big but prefers them when she is
        if (!big) {
            lSwipePref -= 70;
            rSwipePref -= 70;
        } else {
            lSwipePref += 30;
            rSwipePref += 30;
        }

        //proximity-based preference - try not to use certain moves when you're out of range
        if (Vector3.Distance(transform.position, target.transform.position) < 6) {
            quickBitePref += 20;
            pouncePref -= 100;
            forwardBitePref -= 50;
        }
        if (Vector3.Distance(transform.position, target.transform.position) < 12) {
            pouncePref -= 80;
            forwardBitePref -= 20;
            lSwipePref += 20;
            rSwipePref += 20;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 12) {
            pouncePref += 20;
            forwardBitePref += 20;
            quickBitePref -= 80;
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 20) {
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


    public void ChangeGoal(GoalStates inGoal) {
        goal = inGoal;
    }

    public void ChangeGoal() {
        decider = Random.Range(0, attackPref+castPref+bigPref+approachPref+retreatPref+evadePref+guardPref+parryPref);
        if (decider >= 0 && decider <= attackPref) {
            goal = GoalStates.attack;
        }
        else if (decider <= attackPref + castPref) {
            goal = GoalStates.cast;
        }
        else if (decider <= attackPref + castPref + bigPref) {
            goal = GoalStates.big;
            goalDelay = 30;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref) {
            goal = GoalStates.approach;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref + retreatPref) {
            goal = GoalStates.retreat;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref + retreatPref + evadePref) {
            goal = GoalStates.evade;
        }
        else if (decider <= attackPref + castPref + bigPref + approachPref + retreatPref + evadePref + guardPref) {
            goal = GoalStates.guard;
        }
        else {
            goal = GoalStates.parry;
        }
    }

    public void ChangeBehaviour(BehaviourStates inBehaviour) {
        behaviour = inBehaviour;
    }

    public void ChangeBehaviour() {
        decider = Random.Range(0, defensivePref + offensivePref + mixedPref);
        if (decider >= 0 && decider <= defensivePref) {
            behaviour = BehaviourStates.defensive;
        } else if (decider <= defensivePref + offensivePref) {
            behaviour = BehaviourStates.offensive;
        } else {
            behaviour = BehaviourStates.mixed;
        }
    }

    public void Attack() {
        decider = Random.Range(0, quickBitePref + forwardBitePref + lSwipePref + rSwipePref + pouncePref);
        if (decider >= 0 && decider <= quickBitePref) {
            style.QuickBite();
        }
        else if (decider <= quickBitePref + forwardBitePref) {
            if (big) {
                style.ForwardBite(2);
            } else {
                style.ForwardBite(0);
            }
        }
        else if (decider <= quickBitePref + forwardBitePref + lSwipePref) {
            if (big) {
                style.LSwipe(2);
            }
            else {
                style.LSwipe(0);
            }
        }
        else if (decider <= quickBitePref + forwardBitePref + lSwipePref + rSwipePref) {
            if (big) {
                style.RSwipe(2);
            }
            else {
                style.RSwipe(0);
            }
        }
        else {
            if (big) {
                style.Pounce(3);
            }
            else {
                style.Pounce(2);
            }
            pointTargetFrames = 80;
        }
    }


    // Use this for initialization
    void Start () {
        movementAI.target = target;
        targetStatus = target.GetComponent<StatusManager>();
        targetStat = target.GetComponent<StatSheet>();

        goal = GoalStates.nothing;
        behaviour = BehaviourStates.preBattle;
        goalComplete = false;
        goalStarted = false;
        if (navMesh.enabled) {
            navMesh.isStopped = true;
        }
        navMesh.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        //set speedPercent for both big and small animators
        if (big) {
            bigAnimator.SetFloat("speedPercent", navMesh.velocity.magnitude / (maxSpeed), .1f, Time.deltaTime);
        }
        smallAnimator.SetFloat("speedPercent", navMesh.velocity.magnitude / (maxSpeed), .1f, Time.deltaTime);
    }

    // FixedUpdate is called once per physics frame
    private void FixedUpdate() {

        //print(goal);

        if (style.status.slain || style.status.unconscious) {
            BecomeSmall();
            pointTargetFrames = 0;
            attackFrames = 0;
            evadeFrames = 0;
            guardFrames = 0; 
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
            goal = GoalStates.nothing;
        }

        if (big && style.stat.MP < 10) {
            BecomeSmall();
        }

        if (goalDelay > 0) {
            goalDelay--;
        }

        if (pointTargetFrames > 0) {
            pointTargetFrames--;
            movementAI.PointTowardTarget(45);
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

        if (evadeFrames > 0 && style.status.CanMove()) {
            evadeFrames--;
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
        }

        if (guardFrames > 0) {
            guardFrames--;
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
        }

        behaviourCounter--;
        goalCounter--;

        if (big) {
            bigAnimator.SetBool("curious", false);
        }
        smallAnimator.SetBool("curious", false);
        
        if (behaviour == BehaviourStates.defeated && !(style.status.slain || style.status.unconscious)) {
            if (big) {
                BecomeSmall();
            }
            style.status.guarding = false;
            navMesh.speed = maxSpeed * 0.2f;
            ChangeGoal(GoalStates.retreat);
            navMesh.enabled = true;
            navMesh.isStopped = false;
            goalComplete = false;
            goalStarted = false;
            if (Vector3.Distance(transform.position, target.transform.position) > 20 && targetStatus.sheathed && !targetStatus.casting) {
                pacifiedCounter--;
                ChangeGoal(GoalStates.nothing);
                if (big) {
                    bigAnimator.SetBool("curious", true);
                }
                smallAnimator.SetBool("curious", true);
                if (navMesh.enabled) {
                    navMesh.isStopped = true;
                }
                navMesh.enabled = false;
                movementAI.PointTowardTarget(4);
            }
            if ((Vector3.Distance(transform.position, target.transform.position) < 5 || !targetStatus.sheathed || targetStatus.casting) && pacifiedCounter < 600) {
                pacifiedCounter++;
            }
            if (pacifiedCounter <= 0) {
                ChangeBehaviour(BehaviourStates.pacified);
            }
        }

        if (behaviour == BehaviourStates.pacified && !(style.status.slain || style.status.unconscious)) {
            if (big) {
                BecomeSmall();
            }
            gameObject.tag = "TgtAlyBosIns";
            style.status.guarding = false;
            //navMesh.speed = maxSpeed * 0.2f;
            target = stump;
            movementAI.target = target;
            if (!castSanctuary) {
                source.PlayOneShot(howlClipLong, Random.Range(0.4f, 0.6f));
                currentSpell = Instantiate(healingSanctuary).GetComponent<Spell>();
                currentSpell.duration = 14400; //the Shamanwolf's healing sanctuary is longer, lasting 4 minutes
                currentSpell.mouseTarget = false;
                currentSpell.target = target;
                currentSpell.status = style.status;
                currentSpell.stat = style.stat;
                currentSpell.movement = style.movement;
                currentSpell.animator = style.animator;
                currentSpell.transform.position = transform.position;
                currentSpell.transform.rotation = transform.rotation;
                currentSpell.active = true;
                currentSpell.ready = 0;
                currentSpell.Start();
                currentSpell.castTime += currentSpell.spellCode.Length * 15;
                currentSpell.duration = 14400; //the Shamanwolf's healing sanctuary is longer, lasting 4 minutes
                style.status.castLock = currentSpell.castTime + currentSpell.postCastTime;
                style.status.channelLock = currentSpell.castTime + currentSpell.postCastTime + currentSpell.channelTime;
                runningSpells.Add(currentSpell.gameObject);
                style.ForceSpellcast(currentSpell.castTime + currentSpell.postCastTime + 5); //set the spellcast state for a while
                castSanctuary = true;
            } else {
                if (Vector3.Distance(transform.position, target.transform.position) > 6) {
                    navMesh.enabled = true;
                    navMesh.isStopped = false;
                    goal = GoalStates.approach;
                    goalComplete = false;
                    goalStarted = false;
                }
                else {
                    if (navMesh.enabled) {
                        navMesh.isStopped = true;
                    }
                    navMesh.enabled = false;
                    goal = GoalStates.nothing;
                    if (big) {
                        bigAnimator.SetBool("pacified", true);
                    }
                    smallAnimator.SetBool("pacified", true);
                    //have a bit of a lie down
                }
            }
        } else {
            castSanctuary = false;
        }

        if (style.status.IsFloored() && !(style.status.slain || style.status.unconscious)) {
            ChangeGoal(GoalStates.evade);
            goalStarted = false;
            //goalDelay = Random.Range(0, 120);
        }

        if (big) {
            bigCounter--;
            style.stat.MP -= (style.stat.MPregen + 10) / 60;
            if ((bigCounter <= 0 || style.stat.MP <= 0) && goalStarted == false) {
                BecomeSmall();
            }
        }

        if (style.stat.HP < 80 && behaviour != BehaviourStates.pacified) {
            ChangeBehaviour(BehaviourStates.defeated);
        }
        
        if (behaviour == BehaviourStates.preBattle && (Vector3.Distance(transform.position, target.transform.position) < 32 || style.stat.HP < style.stat.MaxHP)) {
            ChangeBehaviour();
            ChangeGoal();
        }
        
        if (targetStatus.unconscious || targetStatus.slain) {
            ChangeBehaviour(BehaviourStates.pacified);
        }

        if (behaviour != BehaviourStates.defeated && behaviour != BehaviourStates.pacified && behaviour != BehaviourStates.preBattle && !(style.status.slain || style.status.unconscious)) {
            if (behaviourCounter <= 0) { //behaviour changes every 30 seconds
                CalculateBehaviourPref();
                ChangeBehaviour();
                behaviourCounter = 1800;
            }

            if ((goalCounter <= 0 || goalComplete) && goalDelay <= 0) { //goals change every 10 seconds or every time a goal is completed
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
        }

        if ((!goalStarted) && goalDelay <= 0) {
            switch (goal) {
                case GoalStates.attack:
                    if (style.status.CanMove()) {
                        if (big) {
                            bigAnimator.Play("turn", 0, 0f);
                        }
                        smallAnimator.Play("turn", 0, 0f);
                    }
                    attackFrames = 40;
                    goalStarted = true;
                    break;
                case GoalStates.cast:
                    if (style.status.CanCast()) {
                        source.PlayOneShot(howlClipShort, Random.Range(0.4f, 0.6f));
                        goalStarted = true;
                        goalDelay = 40;
                    }
                    break;
                case GoalStates.big:
                    if (style.status.CanCast()) {
                        source.PlayOneShot(howlClipShort, Random.Range(0.4f, 0.6f));
                        goalStarted = true;
                        BecomeBig();
                        bigCounter = Random.Range(10*60, 30*60); //become big for anywhere between 10 and 30 seconds
                        goalDelay = 120;
                    }
                    break;
                case GoalStates.approach:
                    if (style.status.CanMove()) {
                        navMesh.enabled = true;
                        navMesh.isStopped = false;
                        navMesh.SetDestination(target.transform.position);
                        navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        goalStarted = true;
                    }
                    break;
                case GoalStates.retreat:
                    if (style.status.CanMove()) {
                        navMesh.enabled = true;
                        navMesh.isStopped = false;
                        retreatDist = Random.Range(Vector3.Distance(transform.position, target.transform.position), 28f);
                        navMesh.SetDestination(transform.position + Vector3.Normalize(transform.position - target.transform.position) * retreatDist);
                        navMesh.SetDestination(new Vector3(navMesh.destination.x, 0, navMesh.destination.z)); //get rid of y
                        goalStarted = true;
                    }
                    break;
                case GoalStates.evade:
                    if (style.status.CanMove()) {
                        if (big) {
                            bigAnimator.Play("turn", 0, 0f);
                        }
                        smallAnimator.Play("turn", 0, 0f);
                    }
                    evadeFrames = 15;
                    goalStarted = true;
                    break;
                case GoalStates.guard:
                    guardFrames = Random.Range(60, 180); //guard for between 1 and 2.5 seconds
                    goalStarted = true;
                    break;
                case GoalStates.parry:
                    if (style.status.CanParry()) {
                        movementAI.PointTowardTarget(60);
                        decider = Random.Range(0, 2);
                        if (decider > 1) {
                            style.FParry();
                        }
                        else {
                            style.BParry();
                        }
                        goalStarted = true;
                    }
                    break;
            }
        }

        if (goalStarted && !goalComplete) {
            switch (goal) {
                case GoalStates.attack:
                    if (attackFrames > 1) {
                        movementAI.PointTowardTarget(12);
                    }
                    else {
                        if (attackFrames > 0) {
                            if (style.status.CanAttack()) {
                                CalculateAttackPref();
                                Attack();
                            }
                        }
                        if (style.instantiatedAttacks.Count <= 0) {
                            goalComplete = true; //once the boss has attacked, the goal is complete
                        }
                    }
                    break;
                case GoalStates.cast:
                    chosenSpellNumber = Random.Range(0, spellsKnown.Count - 1);
                    if (style.status.CanCast() && style.status.castLock <= 0 && !style.status.casting) {
                        style.status.casting = true;
                    }
                    if (style.status.casting && style.status.channelLock <= 0) {
                        style.ForceSpellcast(1);
                        currentSpellName = spellsKnown[chosenSpellNumber].GetComponent<Spell>().spellName;
                        repeatSpell = false;
                        for (int i2 = 0; i2 < runningSpells.Count; i2++) {
                            if (currentSpellName == runningSpells[i2].GetComponent<Spell>().spellName) {
                                repeatSpell = true;
                            }
                        }
                        if (!repeatSpell) {
                            currentSpell = Instantiate(spellsKnown[chosenSpellNumber]).GetComponent<Spell>();
                            currentSpell.mouseTarget = false;
                            currentSpell.target = target;
                            currentSpell.status = style.status;
                            currentSpell.stat = style.stat;
                            currentSpell.movement = style.movement;
                            currentSpell.animator = style.animator;
                            currentSpell.transform.position = transform.position;
                            currentSpell.transform.rotation = transform.rotation;
                            currentSpell.active = true;
                            currentSpell.ready = 0;
                            currentSpell.Start();
                            currentSpell.castTime += currentSpell.spellCode.Length * 15;
                            style.status.castLock = currentSpell.castTime + currentSpell.postCastTime;
                            style.status.channelLock = currentSpell.castTime + currentSpell.postCastTime + currentSpell.channelTime;
                            runningSpells.Add(currentSpell.gameObject);
                            style.ForceSpellcast(currentSpell.castTime + currentSpell.postCastTime + 5); //set the spellcast state for a while
                        }
                    }
                    if (style.status.castLock > 0) {
                        goalComplete = true;
                        style.status.casting = false;
                    }
                    break;
                case GoalStates.big:
                    if (big) { 
                        goalComplete = true; //becoming big happens instantaneously so the goal is completed
                    }
                    break;
                case GoalStates.approach: 
                    if (style.status.CanMove()) {
                        navMesh.enabled = true;
                        navMesh.isStopped = false;
                    } else {
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                    }
                    if ((big && Vector3.Distance(transform.position, target.transform.position) < 8) || (!big && Vector3.Distance(transform.position, target.transform.position) < 4) || Vector3.Distance(transform.position, navMesh.destination) < 4) {
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                        goalComplete = true; //if the target is more-or-less reached, goal complete
                    } else if (goalCounter < 180 || navMesh.isPathStale/* || navMesh.pathStatus != NavMeshPathStatus.PathComplete*/) { //this goal can potentially be recognised as a failure, in which case try to salvage
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
                case GoalStates.retreat:
                    if (style.status.CanMove()) {
                        navMesh.enabled = true;
                        navMesh.isStopped = false;
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
                    } else if (goalCounter < 150 || navMesh.isPathStale || navMesh.pathStatus != NavMeshPathStatus.PathComplete) { //this goal can potentially be recognised as a failure, in which case try to salvage
                        print("abortRetreat");
                        if (navMesh.enabled) {
                            navMesh.isStopped = true;
                        }
                        navMesh.enabled = false;
                        CalculateGoalPref();
                        if (!big) {
                            bigPref += 50; //big chance of going big if not big
                        }
                        castPref = 1; //very low chance of casting
                        approachPref = 10; //why approach when you can't even get away?
                        retreatPref = 10; //why retreat when you already know you can't? Might have just messed up on distance so fair
                        guardPref += 20;//prefer guard
                        evadePref = 10; //low chance of evading since that's often backwards
                        ChangeGoal();
                        goalCounter = 600;
                        goalStarted = false;
                    }
                    break;
                case GoalStates.evade:
                    if (evadeFrames > 0 && style.status.CanMove()) {
                        movementAI.PointAwayFromTarget(Random.Range(3, 5));
                    } else {
                        if (style.status.CanRoll()) {
                            style.movement.Evade(evadeSpeed, 0, 0.5f);
                            if (big) {
                                bigAnimator.Play("jump", 0, 0f);
                            }
                            smallAnimator.Play("jump", 0, 0f);
                            goalDelay = 45;
                            goalComplete = true; //evade complete
                        }
                    }
                    break;
                case GoalStates.guard:
                    if (guardFrames <= 0) { 
                        goalComplete = true;
                        style.status.guarding = false;
                    } else {
                        if (style.status.CanGuard()) {
                            style.status.guarding = true;
                            if (!style.status.IsGuardStunned()) {
                                movementAI.PointTowardTarget(3);
                            }
                        } else {
                            style.status.guarding = false;
                        }
                    }
                    break;
                case GoalStates.parry:
                    goalComplete = true; //parry happens instantaneously so it autocompletes
                    break;
            }


        }

        if (navMesh.enabled) {
            if (!source.isPlaying) {
                source.Play();
            }
            source.loop = true;
            if (behaviour == BehaviourStates.defeated) {
                source.clip = whimperClip;
            } else {
                source.clip = moveClip;
            }
        } else {
            source.clip = null;
            source.loop = false;
        }

        if (!style.status.CanGuard()) {
            style.status.guarding = false;
        }

        if (big) {
            guardBubbleS.material.color = new Color(guardBubbleS.material.color.r, guardBubbleS.material.color.g, guardBubbleS.material.color.b, 0f);
            parryBubbleS.material.color = new Color(parryBubbleS.material.color.r, parryBubbleS.material.color.g, parryBubbleS.material.color.b, 0f);
        } else {
            guardBubbleL.material.color = new Color(guardBubbleL.material.color.r, guardBubbleL.material.color.g, guardBubbleL.material.color.b, 0f);
            parryBubbleL.material.color = new Color(parryBubbleL.material.color.r, parryBubbleL.material.color.g, parryBubbleL.material.color.b, 0f);
        }

        if ((!style.status.CanMove())) {
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
        }

        if (style.status.casting || style.status.channelLock > 0) {
            if (big) {
                bigAnimator.SetBool("trueCasting", true);
            }
            smallAnimator.SetBool("trueCasting", true);
            if (navMesh.enabled) {
                navMesh.isStopped = true;
            }
            navMesh.enabled = false;
            movementAI.PointTowardTarget(30); //point to the target while casting
        } else {
            if (big) {
                bigAnimator.SetBool("trueCasting", false);
            }
            smallAnimator.SetBool("trueCasting", false);
        }
        if (style.status.spellFlinchTrigger) {
            StopCasting();
            style.status.spellFlinchTrigger = false;
        }


        for (int i = 0; i < runningSpells.Count; i++) {
            runningSpellScript = runningSpells[i].GetComponent<Spell>();
            runningSpellScript.castTime--;
            if (runningSpellScript.castTime <= 0 && runningSpellScript.ready == 0) {
                runningSpellScript.ready = 1;
                if (!runningSpellScript.costDeducted) {
                    style.stat.MP -= runningSpellScript.cost;
                    runningSpellScript.costDeducted = true;
                }
            }
            if (runningSpellScript.castTime <= 0 && runningSpellScript.postCastTime > 0) {
                runningSpellScript.postCastTime--;
            }
            if (runningSpellScript.castTime <= 0 && runningSpellScript.postCastTime <= 0 && runningSpellScript.channelTime > 0) {
                runningSpellScript.channelTime--;
            }
            if (runningSpellScript.castTime <= 0 && runningSpellScript.postCastTime <= 0 && runningSpellScript.channelTime <= 0) {
                runningSpellScript.duration--;
            }
            if (runningSpellScript.duration <= 0) {
                toDestroy = runningSpells[i];
                runningSpells.Remove(runningSpells[i]);
                toDestroy.GetComponent<Spell>().DestroySpell();
            }
            else if (runningSpellScript.ready == 1) {
                runningSpellScript.CastSpell();
            }
        }

    }
}
