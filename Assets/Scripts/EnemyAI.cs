using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour { //a default script for regular enemy AIs to inherit

    public AIMovement movementAI;
    public virtual AtkStyle Style { get; set; } //the attack style
    public NavMeshAgent navMesh; //the navmeshagent for this unit

    public GameObject target; //the curent target of the enemy, along with its status and statsheet
    private StatusManager targetStatus;
    private StatSheet targetStat;

    //public enum goalStates { attack, cast, big, approach, retreat, evade, guard, parry, nothing }; //states for the enemy's current goal
    //public enum behaviourStates { preBattle, defensive, offensive, mixed, defeated, pacified }; //states for the enemy's current behaviour
    //public goalStates goal;
    //public behaviourStates behaviour;
    //each AI should have something like this ^^^ so as to define what they do. Behaviour may not be entirely necessary.
    //remember that each behaviour and goal should have its own preference variable

    public float decider; //a variable that holds a random value that decides which option to choose

    public int goalCounter; //tracks the time spent in the current goal
    //public int behaviourCounter; //tracks the time spent in the current behaviour - this may or may not be needed, as an enemy might simply have an assigned behaviour when it is instantiated

    public bool goalStarted; //whether the current goal's action has started
    public bool goalComplete; //whether the current goal is complete
    public int goalDelay; //an int that, while it is above zero, ticks down each frame and prevents certain goal conditions

    public int pointTargetFrames; //when above zero, this will tick down - for every frame it is above zero, the enemy will point toward its target
    public int turnSpeed; //the number of degrees the enemy can turn each frame using pointTargetFrames;

    public void SelectTarget(GameObject newTarget) {
        target = newTarget;
        movementAI.target = target;
        targetStatus = target.GetComponent<StatusManager>();
        targetStat = target.GetComponent<StatSheet>();
    }

    public virtual void CalculateGoalPref() {

    }

    public virtual void ChangeGoal() {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// FixedUpdate is called once per physics frame
	public virtual void FixedUpdate () {


    }
}
