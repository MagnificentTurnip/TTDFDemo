using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Dialogue : MonoBehaviour {

    public CommManager comms;
    public PlayerInput playIn;
    public StatusManager playerStatus; //need status to check sheathed on everything, for the most part

    public string flowMessage;

    public float minDistance;

    // Use this for initialization
    public virtual void Start() {

    }

    // Update is called once per frame
    public virtual void Update() {
        if (playIn.currentTarget == this.gameObject && playIn.button3 && playerStatus.sheathed && !playerStatus.casting && !playerStatus.unconscious && !playerStatus.slain && Vector3.Distance(transform.position, playerStatus.gameObject.transform.position) <= minDistance && !comms.commIn.interactInstantiated && !comms.commIn.dialogueInstantiated && !comms.commIn.inspectInstantiated) {
            Flowchart.BroadcastFungusMessage(flowMessage);
        }
        if (Vector3.Distance(transform.position, playerStatus.gameObject.transform.position) > minDistance) {
            comms.Dlg = false;
        } else {
            comms.Dlg = true;
        }
    }
}
