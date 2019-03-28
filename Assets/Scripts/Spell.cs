using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : AtkStyle {

    public string spellCode; //the inputted spell code required to cast the spell
    public int castTime; //the time it takes to cast the spell after the spell code has been input
    public int cost; //the spell's cost in MP
    public int channelTime; //the time spent channelling the spell after it is cast - can be 0, and often is.
    public int duration; //the duration of the spell itself (transferred effects can have longer durations) - after this timer runs to 0 the spell object is destroyed
    public GameObject target; //the target for the spell, if applicable
    public bool mouseTarget; //some spells follow the mouse when used by the player and a target when used by other agents
    // public List<Effect> effects; //a spell may well have effects but doesn't a fair amount of the time. I think this can be added to each spell that has effects, but just in case this is here as a reminder.
    //if it does have this, remember that destroySpell() needs to be overriden with the destruction of the effects stored in the list and the clearing of it

    public bool active;
    public int ready;

    public virtual void destroySpell() {
        destroyAllAttacks();
        Destroy(this.gameObject);
    }

    public virtual void CastSpell() {
        ready = 2;
    }

	// Use this for initialization
	public virtual void Start () {
        //active = false;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}

    public virtual void FixedUpdate() {
        if (active) {
            attackProgression();
        }
    }
}
