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
            case AIWolfShaman.BehaviourStates.preBattle:
                flowMessage = "InsWolfPre";
                break;
            case AIWolfShaman.BehaviourStates.mixed:
                flowMessage = "InsWolfMix";
                break;
            case AIWolfShaman.BehaviourStates.offensive:
                flowMessage = "InsWolfOff";
                break;
            case AIWolfShaman.BehaviourStates.defensive:
                flowMessage = "InsWolfDef";
                break;
            case AIWolfShaman.BehaviourStates.defeated:
                flowMessage = "InsWolfAft";
                break;
            case AIWolfShaman.BehaviourStates.pacified:
                flowMessage = "InsWolfPac";
                break;
        }
        base.Update();
	}
}
