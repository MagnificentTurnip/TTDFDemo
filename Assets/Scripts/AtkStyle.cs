using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyle : MonoBehaviour {

    public enum attackStates { idle, fParry, bParry}
    public attackStates state;
    public int idleCounter; //the current number of frames until the entity returns to idle after a move

    public StatusManager status;
    public StatSheet stat;
    public Movement movement;

    public bool debug;
    public Mesh cube; //for testing hitboxes
    public Mesh sphere; //for testing hitboxes

    public Attack currentAttack;
    public List<Attack> instantiatedAttacks;
    public GameObject attack;
    public Animator animator;
    public RuntimeAnimatorController hitboxAnimatorController;

    // Use this for initialization
    void Start () {
		
	}

    public void fParry() {
        
    }

    public void bParry() {

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
