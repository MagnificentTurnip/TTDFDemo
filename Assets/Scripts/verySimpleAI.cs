﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verySimpleAI : MonoBehaviour {

    public AtkStyleWish style;
    public StatusManager status;
    public bool buh;
    public bool attacking;

    // Use this for initialization
    void Start () {
        buh = true;
        attacking = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (status.canAttack() && attacking && buh) {
            buh = false;
            StartCoroutine(bup());
        }
	}

    public IEnumerator bup() {
        yield return new WaitForSeconds(2);
        if (status.canAttack()) {
            style.standardBladework2();
        }
        buh = true;
    }
}
