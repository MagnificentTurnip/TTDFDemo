using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectShamanwolf : Inspect {

    public AIWolfShaman ai;

    // Use this for initialization
    public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
        switch (ai.behaviour) {
            case AIWolfShaman.behaviourStates.preBattle:
                flowMessage = "InsWolfPre";
                break;
            case AIWolfShaman.behaviourStates.mixed:
                flowMessage = "InsWolfMix";
                break;
            case AIWolfShaman.behaviourStates.offensive:
                flowMessage = "InsWolfOff";
                break;
            case AIWolfShaman.behaviourStates.defensive:
                flowMessage = "InsWolfDef";
                break;
            case AIWolfShaman.behaviourStates.defeated:
                flowMessage = "InsWolfAft";
                break;
            case AIWolfShaman.behaviourStates.pacified:
                flowMessage = "InsWolfPac";
                break;
        }
        base.Update();
	}
}
