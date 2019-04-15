using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour {
    
    public StatSheet stat;
    public Hittable hittable;

    public bool deathInit; //certain things should only be set once upon death

    public int framesUntilDeleted; //make sure this is above zero if you don't want the gameobject deleted instantly upon death;

    public virtual void StartDie() {
        
    }

    public virtual void ContinueDie() {

    }

    public virtual void KO() {

    }

    // Use this for initialization
    public virtual void Start () {
        deathInit = false;
	}

    // FixedUpdate is called once per logical step
    public virtual void FixedUpdate() {
        if (stat.HP < 0) { //if HP is less than zero, then you die
            if (!deathInit) {
                StartDie();
                deathInit = true;
            }
            ContinueDie();
        }
        else if (stat.HP == 0) { //if HP is exactly zero, then you just get knocked out. 
            KO();
        }
        if (stat.status.slain) {
            if (framesUntilDeleted <= 0) {
                Destroy(this.gameObject);
            }
            framesUntilDeleted--;
        }
    }
}
