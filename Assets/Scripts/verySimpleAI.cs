using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verySimpleAI : MonoBehaviour {  //legacy script, no longer used

    public AtkStyleWish style;
    public StatusManager status;
    public bool buh;
    public bool attacking;

    // Use this for initialization
    void Start () {
        buh = true;
        attacking = true;
        style.animator.Play("MainBladeOn", 1);//play the unsheathing animation
    }
	
	// Update is called once per frame
	void Update () {
		if (status.CanAttack() && attacking && buh) {
            buh = false;
            StartCoroutine(bup());
        }
	}

    public IEnumerator bup() {
        yield return new WaitForSeconds(2);
        if (status.CanAttack()) {
            style.HeavyBladework4();
        }
        buh = true;
    }
}
