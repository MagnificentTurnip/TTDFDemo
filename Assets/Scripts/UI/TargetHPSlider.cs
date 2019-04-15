using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//https://commons.wikimedia.org/wiki/Main_Page source for icons that aren't included in unity

public class TargetHPSlider : MonoBehaviour {

    public PlayerInput playIn;
    public StatusManager playerStatus;
    public GameObject savedTarget;
    public StatSheet targetStat;

    public Slider targetHPSlider;
    public TextMeshProUGUI targetHPText;
    public TextMeshProUGUI targetNameText;
    public Image targetingBeacon;

    public CommManager tgtComm;
    public Image dialogueBeacon;
    public Image interactBeacon;
    public Image inspectBeacon;

    public TextMeshProUGUI nonHPTarget;

    public bool beginInitialise;

    // Use this for initialization
    void Start () {
        targetHPSlider.gameObject.SetActive(false);
        nonHPTarget.gameObject.SetActive(false);
        targetingBeacon.gameObject.SetActive(false);
    }
	
	// FixedUpdate is called once per logical step
	void FixedUpdate () {
        if (playIn.currentTarget != null) {
            if (playIn.currentTarget.GetComponent<StatSheet>()) {
                targetHPSlider.gameObject.SetActive(true);

                if (!beginInitialise || savedTarget != playIn.currentTarget) {
                    targetStat = playIn.currentTarget.GetComponent<StatSheet>();
                    targetHPSlider.minValue = 0;
                    targetHPSlider.maxValue = Mathf.CeilToInt(targetStat.MaxHP);
                    beginInitialise = true;
                }

                targetingBeacon.gameObject.SetActive(true);

                if (playIn.currentTarget.tag.Contains("Enm")) {
                    targetingBeacon.color = new Color(1f, 0.6f, 0.6f, playIn.targetRefresh1 * 0.08f);
                } else if (playIn.currentTarget.tag.Contains("Aly")) {
                    targetingBeacon.color = new Color(0.6f, 0.6f, 1f, playIn.targetRefresh1 * 0.06f);
                } else if (playIn.currentTarget.tag.Contains("Pla")) {
                    targetingBeacon.color = new Color(1f, 1f, 1f, playIn.targetRefresh1 * 0.04f);
                } else {
                    targetingBeacon.color = new Color(0.1f, 0.1f, 0.1f, playIn.targetRefresh1 * 0.1f);
                }

                savedTarget = playIn.currentTarget;
            } else {
                targetingBeacon.gameObject.SetActive(false);
            }
            nonHPTarget.gameObject.SetActive(true);
            nonHPTarget.text = playIn.currentTarget.name;

            if (playIn.currentTarget.GetComponent<CommManager>()) {
                tgtComm = playIn.currentTarget.GetComponent<CommManager>();
                if (playIn.currentTarget.tag.Contains("Dlg")) {
                    dialogueBeacon.gameObject.SetActive(true);
                    if (tgtComm.Dlg) {
                        dialogueBeacon.color = new Color(0.9f, 0.9f, 0.9f, playIn.targetRefresh1 * 0.06f);
                    } else {
                        dialogueBeacon.color = new Color(0.9f, 0.9f, 0.9f, playIn.targetRefresh1 * 0.02f);
                    }
                } else {
                    dialogueBeacon.gameObject.SetActive(false);
                }
                if (playIn.currentTarget.tag.Contains("Int")) {
                    interactBeacon.gameObject.SetActive(true);
                    if (tgtComm.Int) {
                        interactBeacon.color = new Color(0.9f, 0.9f, 0.9f, playIn.targetRefresh1 * 0.06f);
                    }
                    else {
                        interactBeacon.color = new Color(0.9f, 0.9f, 0.9f, playIn.targetRefresh1 * 0.02f);
                    }
                } else {
                    interactBeacon.gameObject.SetActive(false);
                }
                if (playIn.currentTarget.tag.Contains("Ins")) {
                    inspectBeacon.gameObject.SetActive(true);
                    if (tgtComm.Ins) {
                        inspectBeacon.color = new Color(0.9f, 0.9f, 0.9f, playIn.targetRefresh1 * 0.06f);
                    }
                    else {
                        inspectBeacon.color = new Color(0.9f, 0.9f, 0.9f, playIn.targetRefresh1 * 0.02f);
                    }
                } else {
                    inspectBeacon.gameObject.SetActive(false);
                }
            } else {
                dialogueBeacon.gameObject.SetActive(false);
                interactBeacon.gameObject.SetActive(false);
                inspectBeacon.gameObject.SetActive(false);
            }

        } else {
            beginInitialise = false;
            nonHPTarget.gameObject.SetActive(false);
            if (playIn.targetRefresh2 <= 0) {
                targetHPSlider.gameObject.SetActive(false);
            }
        }
        if (targetStat != null) {
            targetHPSlider.value = Mathf.CeilToInt(targetStat.HP);
            targetNameText.text = targetStat.gameObject.name;
            targetHPText.text = Mathf.CeilToInt(targetStat.HP).ToString() + " / " + Mathf.CeilToInt(targetStat.MaxHP).ToString();
        }
        if (!playerStatus.sheathed) {
            nonHPTarget.gameObject.SetActive(false);
            dialogueBeacon.gameObject.SetActive(false);
            interactBeacon.gameObject.SetActive(false);
            inspectBeacon.gameObject.SetActive(false);
        }
        nonHPTarget.color = new Color(0.9f, 0.9f, 0.9f, playIn.targetRefresh1 * 0.1f);
    }
}
