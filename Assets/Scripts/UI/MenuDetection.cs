using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class MenuDetection : MonoBehaviour {

    public StatusManager bossStatus;
    public StatusManager playerStatus;
    public AIWolfShaman bossAI;

    public Flowchart flow;

    public GameObject winCanvas;
    public GameObject failCanvas;
    public GameObject notFailCanvas;
    public GameObject watCanvas;
    public GameObject prettyMuchWinCanvas;

    public void TrueWin() {
        winCanvas.SetActive(true);
        failCanvas.SetActive(false);
        notFailCanvas.SetActive(false);
        watCanvas.SetActive(false);
        prettyMuchWinCanvas.SetActive(false);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (bossAI.behaviour == AIWolfShaman.behaviourStates.pacified && !bossStatus.slain && !bossStatus.unconscious) {
            flow.SetBooleanVariable("TrueWinPossible", true);
        } else {
            flow.SetBooleanVariable("TrueWinPossible", false);
        }
        if (bossStatus.slain || bossStatus == null) {
            notFailCanvas.SetActive(true);
            prettyMuchWinCanvas.SetActive(false);
            winCanvas.SetActive(false);
        } else if (bossStatus.unconscious) {
            prettyMuchWinCanvas.SetActive(true);
            winCanvas.SetActive(false);
        }
        if ((bossStatus.unconscious || bossStatus.slain || bossStatus == null) && (playerStatus.unconscious || playerStatus.slain)) {
            winCanvas.SetActive(false);
            failCanvas.SetActive(false);
            notFailCanvas.SetActive(false);
            watCanvas.SetActive(true);
            prettyMuchWinCanvas.SetActive(false);
        }
        if (!bossStatus.unconscious) {
            prettyMuchWinCanvas.SetActive(false);
        }
    }
}
