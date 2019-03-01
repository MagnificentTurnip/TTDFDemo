using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSheet : MonoBehaviour {

    public StatusManager status;

    public int Level;

    public float MaxHP;
    public float MaxSP;
    public float MaxMP;

    public float HP;
    public float SP;
    public float MP;

    public float HPregen;
    public float SPregen;
    public float MPregen;

    // Use this for initialization
    void Start () {
		
	}
	
	// FixedUpdate is called a fixed amount of frames over time
	void FixedUpdate () {

        //HP, SP AND MP should regenerate if allowed
        if (status.HPRegenEnabled && HP < MaxHP) {
            HP += HPregen / 60;
        }
        if (status.SPRegenEnabled && SP < MaxSP) {
            SP += SPregen / 60;
        }
        if (status.MPRegenEnabled && MP < MaxMP) {
            MP += MPregen / 60;
        }

        //but they should also be capped at their maximums
        if (HP > MaxHP) {
            HP = MaxHP;
        }
        if (SP > MaxSP) {
            SP = MaxSP;
        }
        if (MP > MaxMP) {
            MP = MaxMP;
        }

    }
}
