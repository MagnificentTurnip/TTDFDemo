using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStatSheet : StatSheet {

    public float STR;
    public float DEX;
    public float WIL;
    public float FOR;
    public float TEN;
    public float RES;
    public float VIT;
    public float END;
    public float ATT;
    public float CON;
    public float AGI;
    public float PRE;
    public float INT;
    public float CHA;
    public float FAT;

    // Use this for initialization
    void Start () {

        MaxHP = 500f + (25f * Level) + (5f * FOR) + (CON);
        MaxSP = 500f + (25f * Level) + (5f * TEN) + (AGI);
        MaxMP = 500f + (25f * Level) + (5f * RES) + (PRE);

        HP = MaxHP;
        SP = MaxSP;
        MP = MaxMP;

        HPregen = 1f + (0.1f * Level) + (0.1f * VIT) + (0.05f * CON);
        SPregen = 50f + (5f * Level) + (2f * END) + (0.5f * AGI);
        MPregen = 5f + (0.5f * Level) + (1f * ATT) + (0.2f * PRE);

    }
	
	// Update is called once per frame
	void Update () {
		
        

	}
}
