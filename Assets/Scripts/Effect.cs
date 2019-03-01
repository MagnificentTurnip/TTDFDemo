using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    StatusManager status;
    CharStatSheet stat;

    void Apply(StatusManager applyTo) {
        status = applyTo;
        status.effects.Add(this);
    }

    void Apply(StatusManager applyTo, StatSheet _stat) {
        status = applyTo;
        status.effects.Add(this);
        stat = _stat as CharStatSheet;
    }

    void Apply(StatusManager applyTo, CharStatSheet _stat) {
        status = applyTo;
        status.effects.Add(this);
        stat = _stat;
    }

    public virtual void Flexible() {

    }

    // Use this for initialization
    public virtual void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}

    public virtual void FixedUpdate() {

    }
}
