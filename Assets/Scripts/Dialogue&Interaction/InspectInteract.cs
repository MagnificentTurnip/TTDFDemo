using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class InspectInteract : MonoBehaviour {

    public CommManager comms;
    public PlayerInput playIn;
    public StatusManager playerStatus; //need status to check sheathed on everything, for the most part

    public string insMessage;
    public string intMessage;

    public float minDistance;

	// Use this for initialization
	public virtual void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
        if (playIn.currentTarget == this.gameObject && playerStatus.sheathed && !playerStatus.unconscious && !playerStatus.slain && Vector3.Distance(transform.position, playerStatus.gameObject.transform.position) <= minDistance) {
            comms.Ins = false;
            comms.Int = true;
            if (playIn.button4 && !playerStatus.casting && !comms.commIn.interactInstantiated && !comms.commIn.dialogueInstantiated && !comms.commIn.inspectInstantiated && !comms.commIn.menuInstantiated) {
                Flowchart.BroadcastFungusMessage(intMessage);
            }
        } else if (playIn.currentTarget == this.gameObject && playerStatus.sheathed && !playerStatus.unconscious && !playerStatus.slain) {
            comms.Ins = true;
            comms.Int = false;
            if (playIn.button4 && !playerStatus.casting && !comms.commIn.interactInstantiated && !comms.commIn.dialogueInstantiated && !comms.commIn.inspectInstantiated && !comms.commIn.menuInstantiated) {
                Flowchart.BroadcastFungusMessage(insMessage);
            }
        }

    }
}
