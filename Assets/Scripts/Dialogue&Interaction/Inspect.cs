using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Inspect : MonoBehaviour {

    public CommManager comms;
    public PlayerInput playIn;
    public StatusManager playerStatus; //need status to check sheathed on everything, for the most part

    public string flowMessage;

    // Use this for initialization
    public virtual void Start() {
        comms.Ins = true;
    }

    // Update is called once per frame
    public virtual void Update() {
        if (playIn.currentTarget == this.gameObject && playIn.button4 && playerStatus.sheathed && !playerStatus.casting && !playerStatus.unconscious && !playerStatus.slain && !comms.commIn.interactInstantiated && !comms.commIn.dialogueInstantiated && !comms.commIn.inspectInstantiated) {
            Flowchart.BroadcastFungusMessage(flowMessage);
        }
        comms.Ins = true;
    }

}
