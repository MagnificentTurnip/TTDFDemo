using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyle : MonoBehaviour {

    /* 
     Inheriting AtkStyle classes should contain something along the lines of:

        public enum attackStates { idle, fEvade, bEvade, guarding, spellcast, standardAttack1, standardAttack2, weirdAttack3, geoff4 ...(etc.) };
        public attackStates state;

        A reminder that idle, guarding and spellcast states need to be altered from outside the atkStyle script, and thus also have force functions.
    */

    public int idleCounter; //the current number of frames until the entity returns to idle after a move

    public StatusManager status;
    public StatSheet stat;
    public CharStatSheet charStat; //an attack style might use either a character stat sheet or just a regular one
    public Movement movement;
    public bool hitlag; //whether or not this style accepts hitlag;

    public bool debug;
    public Mesh cube; //for testing hitboxes
    public Mesh sphere; 
    public Mesh capsule;

    public bool hurtboxShown; //whether the hurtbox is displayed or not

    public Attack.damage tempDamage;

    public Attack currentAttack;
    public List<Attack> instantiatedAttacks;
    private Attack toDestroy; //going to need to remove an object from this list before destroying it
    public GameObject attack;
    public Animator animator;
    public RuntimeAnimatorController hitboxAnimatorController;

    // Use this for initialization
    void Start () {
        hurtboxShown = false;
	}

    public virtual void forceIdle() { //this function is for outside access, to set the attack state of inheriting attack styles to its idle position
        print("You need to override this function in a specific attack style that inherits from this class. The function should simply set the attack state to idle. Look at the existing AtkStyles.");
    }

    public virtual void forceGuarding(int counterIdle) { //this function is for outside access, to set the attack state of inheriting attack styles to the guarding state
        print("You need to override this function in a specific attack style that inherits from this class. The function should set the attack state to guarding and also set the idle counter to an appropriate amount (counterIdle). Look at the existing AtkStyles.");
    }

    public virtual void forceSpellcast(int counterIdle) { //this function is for outside access, to set the attack state of inheriting attack styles to the spellcast state
        print("You need to override this function in a specific attack style that inherits from this class.The function should set the attack state to spellcast and also set the idle counter to an appropriate amount (counterIdle). Look at the existing AtkStyles.");
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

    public void attackProgression() { //manages attack progression from delay, to duration, to end, and eventually its automatic destruction, with the added nature of stopping their movement components when they cannot be accessed
        for (int i = 0; i < instantiatedAttacks.Count; i++) {

            
                if (instantiatedAttacks[i].data.attackDelay > 0) {
                    instantiatedAttacks[i].data.attackDelay -= 1;
                    if (debug && instantiatedAttacks[i] != null) {
                        instantiatedAttacks[i].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.3f, 1f, 0.3f, 0.8f));
                    }
                }

                else if (instantiatedAttacks[i].data.attackDuration > 0) {
                    if (charStat != null) {
                        charStat.HP -= instantiatedAttacks[i].data.HPcost; //apply costs
                        charStat.MP -= instantiatedAttacks[i].data.MPcost;
                        charStat.SP -= instantiatedAttacks[i].data.SPcost;
                    }
                    else {
                        stat.HP -= instantiatedAttacks[i].data.HPcost; //apply costs
                        stat.MP -= instantiatedAttacks[i].data.MPcost;
                        stat.SP -= instantiatedAttacks[i].data.SPcost;
                    }
                    instantiatedAttacks[i].data.HPcost = 0; //costs are only applied once
                    instantiatedAttacks[i].data.MPcost = 0;
                    instantiatedAttacks[i].data.SPcost = 0;

                    instantiatedAttacks[i].data.attackDuration -= 1;

                    if (debug && instantiatedAttacks[i] != null) {
                        instantiatedAttacks[i].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1f, 0.3f, 0.3f, 0.8f));
                    }
                }

                else if (instantiatedAttacks[i].data.attackEnd > 0) {
                    if (debug && instantiatedAttacks[i] != null) {
                        instantiatedAttacks[i].GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.3f, 1f, 0.3f, 0.8f));
                    }
                    instantiatedAttacks[i].data.attackEnd -= 1;
                }
                if (instantiatedAttacks[i].data.attackEnd <= 0 && instantiatedAttacks[i].data.attackDuration <= 0 && instantiatedAttacks[i].data.attackDelay <= 0) {
                    toDestroy = instantiatedAttacks[i];
                    instantiatedAttacks.Remove(instantiatedAttacks[i]);
                    if (toDestroy != null) {
                        Destroy(toDestroy.gameObject);
                    }
                }
            
        }
        if (status.guarding || status.rolling) {
            StopAllCoroutines();
        }
    }

    public void destroyAllAttacks() {
        for (int i = 0; i < instantiatedAttacks.Count; i++) {
            toDestroy = instantiatedAttacks[i];
            //instantiatedAttacks.Remove(instantiatedAttacks[i]);
            Destroy(toDestroy.gameObject);
            StopAllCoroutines();
        }
        instantiatedAttacks.Clear();
    }

    public void nonSpellAtkStyle() { //if this attack style has nothing to do with spellcasting, then delete all of its attacks when spellcasting
        if (status.casting) {
            destroyAllAttacks();
        }
    }

    public void showHurtBox() {
        if (gameObject.GetComponent<CapsuleCollider>() && !hurtboxShown) {
            gameObject.GetComponent<MeshFilter>().mesh = capsule;
            hurtboxShown = true;
        }
        if (gameObject.GetComponent<BoxCollider>() && !hurtboxShown) {
            gameObject.GetComponent<MeshFilter>().mesh = cube;
            hurtboxShown = true;
        }
        if (gameObject.GetComponent<SphereCollider>() && !hurtboxShown) {
            gameObject.GetComponent<MeshFilter>().mesh = sphere;
            hurtboxShown = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
    }
}
