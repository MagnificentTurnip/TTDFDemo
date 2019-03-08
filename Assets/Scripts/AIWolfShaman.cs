using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWolfShaman : MonoBehaviour {

    public enum goalStates {attack, cast, retreat, evade, guard, parry};
    public enum behaviourStates {defensive, offensive, mixed, final};
    public goalStates goal;
    public behaviourStates behaviour;
    public float decider;

    public float attackPref;
    public float castPref;
    public float retreatPref;
    public float evadePref;
    public float guardPref;
    public float parryPref;

    public float defensivePref;
    public float offensivePref;
    public float mixedPref;

    public AtkStyleWolf style;

    public GameObject player;

    public bool big;


    public void CalculateGoalPref() {
        attackPref = castPref = retreatPref = evadePref = guardPref = parryPref = 100; //set all preferences to 100, the base

        //generally wants to go after a different goal than last time
        switch (goal) {
            case goalStates.attack:
                attackPref -= 20;
                break;
            case goalStates.cast:
                castPref -= 50;
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

        //certain defensive options aren't really necessary if enemy is far away
        

        //there are only two states that are actually attacks whereas four are defensive options, so some multiplicative adjustment should occur
        attackPref *= 1.5f;
        //castPref isn't altered
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

    public void ChangeGoal(goalStates inGoal) {
        goal = inGoal;
    }

    public void ChangeGoal() {
        decider = Random.Range(0, attackPref+castPref+retreatPref+evadePref+guardPref+parryPref);
        if (decider >= 0 && decider <= attackPref) {
            goal = goalStates.attack;
        }
        else if (decider <= castPref) {
            goal = goalStates.cast;
        }
        else if (decider <= retreatPref) {
            goal = goalStates.retreat;
        }
        else if (decider <= evadePref) {
            goal = goalStates.evade;
        }
        else if (decider <= guardPref) {
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
        } else if (decider <= offensivePref) {
            behaviour = behaviourStates.offensive;
        } else {
            behaviour = behaviourStates.mixed;
        }
    }


    // Use this for initialization
    void Start () {
        goal = goalStates.cast;
        behaviour = behaviourStates.mixed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
