using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyle : MonoBehaviour {

    /* 
     Inheriting AtkStyle classes should contain something along the lines of:

        public enum attackStates { idle, fEvade, bEvade, standardAttack1, standardAttack2, weirdAttack3, geoff4 ...(etc.) };
        public attackStates state;
    */

    public int idleCounter; //the current number of frames until the entity returns to idle after a move

    public StatusManager status;
    public StatSheet stat;
    public Movement movement;

    public bool debug;
    public Mesh cube; //for testing hitboxes
    public Mesh sphere; //for testing hitboxes
    public Mesh capsule;

    public Attack currentAttack;
    public List<Attack> instantiatedAttacks;
    private Attack toDestroy; //going to need to remove an object from this list before destroying it
    public GameObject attack;
    public Animator animator;
    public RuntimeAnimatorController hitboxAnimatorController;

    // Use this for initialization
    void Start () {
		
	}

    public virtual void forceIdle() { //this function is for outside access, to set the attack state of inheriting attack styles to its idle position
        print("You need to override this function in a specific attack style that inherits from this class. The function should simply set the attack state to idle. Look at the existing AtkStyles.");
    }

    public virtual void returnToIdle() {
        print("You need to override this function in a specific attack style that inherits from this class. The function should manage returning to idle gradually over time. Look at the existing AtkStyles.");
    }

    public virtual void fParry() {
        print("You need to override this function in a specific attack style that inherits from this class. Look at the existing AtkStyles.");
    }

    public virtual void bParry() {
        print("You need to override this function in a specific attack style that inherits from this class. Look at the existing AtkStyles.");
    }

    public void attackProgression() { //manages attack progression from delay, to duration, to end, and eventually its automatic destruction
        for (int i = 0; i < instantiatedAttacks.Count; i++) {

            if (instantiatedAttacks[i].data.attackDelay > 0) {
                instantiatedAttacks[i].data.attackDelay -= 1;
                if (debug) {
                    instantiatedAttacks[i].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.3f, 1f, 0.3f, 0.8f));
                }
            }

            else if (instantiatedAttacks[i].data.attackDuration > 0) {
                instantiatedAttacks[i].data.attackDuration -= 1;
                if (debug) {
                    instantiatedAttacks[i].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1f, 0.3f, 0.3f, 0.8f));
                }
            }

            else if (instantiatedAttacks[i].data.attackEnd > 0) {
                instantiatedAttacks[i].data.attackEnd -= 1;
                if (debug) {
                    instantiatedAttacks[i].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.3f, 1f, 0.3f, 0.8f));
                }
            }
            if (instantiatedAttacks[i].data.attackEnd <= 0 && instantiatedAttacks[i].data.attackDuration <= 0 && instantiatedAttacks[i].data.attackDelay <= 0) {
                toDestroy = instantiatedAttacks[i];
                instantiatedAttacks.Remove(instantiatedAttacks[i]);
                Destroy(toDestroy.gameObject);
            }
        }
    }

    public void destroyAllAttacks() {
        for (int i = 0; i < instantiatedAttacks.Count; i++) {
            toDestroy = instantiatedAttacks[i];
            instantiatedAttacks.Remove(instantiatedAttacks[i]);
            Destroy(toDestroy.gameObject);
        }
    }

    public void showHurtBox() {
        gameObject.GetComponent<MeshFilter>().mesh = capsule;
    }
	
	// Update is called once per frame
	void Update () {
    }
}
