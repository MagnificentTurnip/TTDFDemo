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
        if (status.SPRegenEnabled && SP < MaxSP && !status.sprinting) { //it's easier to just negate SPregen while sprinting here
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

        if (status.sprinting) {
            SP--;
        }

        if (SP < 0) {
            if (status.vulnerable < 60) {
                status.vulnerable = 60;
            }
            if (status.paralyzed < 120) {
                status.paralyzed = 120;
            }
            SP = 0;
        }

        if (MP < 0) {
            if (status.silenced < 180) {
                status.silenced = 180;
            }
            SP += MP*2;
            if (HP + MP*2 > 0) {
                HP += MP*2;
            } else {
                HP = 1;
                if (status.stunned < status.castLock + 60) {
                    status.stunned = status.castLock + 60;
                }
            }
            MP = 0;
        }

        //consequences for HP loss are handled in an object's DeathManager, but by means of explanation:
        /*
        if (HP < 0) { //if HP is less than zero, then you die
            Die() or something
        } else if (HP == 0) { //if HP is exactly zero, then you just get knocked out. 
            KO() or something
        } //Very difficult to do outside of Impact or Astral damage, which automatically leave units at 0 who would otherwise be killed.
        */
    }
}
