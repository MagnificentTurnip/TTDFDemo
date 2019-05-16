using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using TMPro;

public class CommInputManager : MonoBehaviour {

    public PlayerInput playIn;
    public StatusManager playStatus;
    public Flowchart flow;

    public PauseMenu pauseMenu;

    public bool b3New;
    public bool b4New;
    public bool b3release;
    public bool b4release;

    public bool dialogueInstantiated;
    public bool inspectInstantiated;
    public bool interactInstantiated;
    public bool menuInstantiated;

    public bool resetOnce;

    public TextMeshProUGUI helperText;

    public Writer dlgWriter;
    public bool dlgContinue;
    public bool dlgEnd;
    public bool dlgInterrupt;
    public bool dlgPauseGame;
    public Image dlgBtnContinue;
    public Image dlgBtnEnd;
    public Image dlgBtnInterrupt;
    public Image dlgBtnPauseGame;
    RectTransform dlgBtnContinueRect;
    RectTransform dlgBtnEndRect;
    RectTransform dlgBtnInterruptRect;
    RectTransform dlgBtnPauseGameRect;

    public Writer intWriter;
    public bool intContinue;
    public bool intEnd;
    public bool intPauseGame;
    public Image intBtnContinue;
    public Image intBtnEnd;
    public Image intBtnPauseGame;
    RectTransform intBtnContinueRect;
    RectTransform intBtnEndRect;
    RectTransform intBtnPauseGameRect;

    public Writer insWriter;
    public bool insContinue;
    public bool insEnd;
    public Image insBtnContinue;
    public Image insBtnEnd;
    RectTransform insBtnContinueRect;
    RectTransform insBtnEndRect;

    public void StopBlocks() {
        flow.StopAllBlocks();
        
    }

    public bool MouseIntersects(RectTransform transformRect) {
        Rect rect = new Rect(new Vector2(transformRect.position.x - (transformRect.rect.width / 4), transformRect.position.y - (transformRect.rect.height / 4)), new Vector2 (transformRect.rect.width/2, transformRect.rect.height/2));
        if (rect.Contains(Input.mousePosition)) {
            return true;
        } else {
            return false;
        }
    }

    // Use this for initialization
    void Start () {
        b3release = true;
        b4release = true;

        dlgBtnContinueRect = dlgBtnContinue.gameObject.GetComponent<RectTransform>();
        dlgBtnEndRect = dlgBtnEnd.gameObject.GetComponent<RectTransform>();
        dlgBtnInterruptRect = dlgBtnInterrupt.gameObject.GetComponent<RectTransform>();
        dlgBtnPauseGameRect = dlgBtnPauseGame.gameObject.GetComponent<RectTransform>();

        intBtnContinueRect = intBtnContinue.gameObject.GetComponent<RectTransform>();
        intBtnEndRect = intBtnEnd.gameObject.GetComponent<RectTransform>();
        intBtnPauseGameRect = intBtnPauseGame.gameObject.GetComponent<RectTransform>();

        insBtnContinueRect = insBtnContinue.gameObject.GetComponent<RectTransform>();
        insBtnEndRect = insBtnEnd.gameObject.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {

        //communications input has its own input to allow it to work while paused
        //though it also needs to be disallowed on death/unconsciousness

        if (!playStatus.unconscious && !playStatus.slain) {
            b3New = false;
            for (int i = 0; i < playIn.button3Control.Count; i++) {
                if (Input.GetAxis(playIn.button3Control[i]) > 0f) {
                    b3New = true;
                }
            }

            b4New = false;
            for (int i = 0; i < playIn.button4Control.Count; i++) {
                if (Input.GetAxis(playIn.button4Control[i]) > 0f) {
                    b4New = true;
                }
            }
        }

        dialogueInstantiated = flow.GetBooleanVariable("dlgInstantiated");
        inspectInstantiated = flow.GetBooleanVariable("insInstantiated");
        interactInstantiated = flow.GetBooleanVariable("intInstantiated");
        menuInstantiated = flow.GetBooleanVariable("menuInstantiated");


        if ((b3New && b3release) || Input.GetMouseButtonDown(0)) {
            if (playStatus.sheathed && MouseIntersects(dlgBtnContinueRect)) {
                flow.SetBooleanVariable("dlgContinue", true);
            }
            if (playStatus.sheathed && MouseIntersects(dlgBtnEndRect)) {
                flow.SetBooleanVariable("dlgEnd", true);
            }
            if (playStatus.sheathed && MouseIntersects(dlgBtnInterruptRect)) {
                flow.SetBooleanVariable("dlgInterrupt", true);
            }
            if (playStatus.sheathed && MouseIntersects(dlgBtnPauseGameRect)) {
                if (!PauseMenu.menuPaused) {
                    if (PauseMenu.paused) {
                        pauseMenu.MiniResumeGame();
                        flow.SetBooleanVariable("dlgPauseGame", false);
                    }
                    else {
                        pauseMenu.MiniPauseGame();
                        flow.SetBooleanVariable("dlgPauseGame", true);
                    }
                }
            }

            b3release = false;
        }

        if ((b4New && b4release) || Input.GetMouseButtonDown(0)) {
            if (playStatus.sheathed && MouseIntersects(intBtnContinueRect)) {
                flow.SetBooleanVariable("intContinue", true);
            }
            if (playStatus.sheathed && MouseIntersects(intBtnEndRect)) {
                flow.SetBooleanVariable("intEnd", true);
            }
            if (playStatus.sheathed && MouseIntersects(intBtnPauseGameRect)) {
                if (!PauseMenu.menuPaused) {
                    if (PauseMenu.paused) {
                        pauseMenu.Resume();
                        flow.SetBooleanVariable("intPauseGame", false);
                    }
                    else {
                        pauseMenu.Pause();
                        flow.SetBooleanVariable("intPauseGame", true);
                    }
                }
            }

            if (playStatus.sheathed && MouseIntersects(insBtnContinueRect)) {
                flow.SetBooleanVariable("insContinue", true);
            }
            if (playStatus.sheathed && MouseIntersects(insBtnEndRect)) {
                flow.SetBooleanVariable("insEnd", true);
            }

            b4release = false;
        }


        if (!b3New) {
            b3release = true;
        }
        if (!b4New) {
            b4release = true;
        }

        if (!playStatus.sheathed) { //skip through text while unsheathed
            flow.SetBooleanVariable("dlgContinue", true);
            flow.SetBooleanVariable("intContinue", true);
            flow.SetBooleanVariable("insContinue", true);

        } else { //highlight buttons on hover

            if (dialogueInstantiated) {
                if (MouseIntersects(dlgBtnContinueRect)) {
                    dlgBtnContinue.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button3Control[0];
                }
                else {
                    dlgBtnContinue.color = new Color(1, 1, 1, 0.6f);
                }
                if (MouseIntersects(dlgBtnEndRect)) {
                    dlgBtnEnd.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button3Control[0];
                }
                else {
                    dlgBtnEnd.color = new Color(1, 1, 1, 0.6f);
                }
                if (MouseIntersects(dlgBtnInterruptRect)) {
                    dlgBtnInterrupt.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button3Control[0];
                }
                else {
                    dlgBtnInterrupt.color = new Color(1, 1, 1, 0.6f);
                }
                if (MouseIntersects(dlgBtnPauseGameRect)) {
                    dlgBtnPauseGame.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button3Control[0];
                }
                else {
                    dlgBtnPauseGame.color = new Color(1, 1, 1, 0.6f);
                }
            }

            if (interactInstantiated) {
                if (MouseIntersects(intBtnContinueRect)) {
                    intBtnContinue.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button4Control[0];
                }
                else {
                    intBtnContinue.color = new Color(1, 1, 1, 0.6f);
                }
                if (MouseIntersects(intBtnEndRect)) {
                    intBtnEnd.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button4Control[0];
                }
                else {
                    intBtnEnd.color = new Color(1, 1, 1, 0.6f);
                }
                if (MouseIntersects(intBtnPauseGameRect)) {
                    intBtnPauseGame.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button4Control[0];
                }
                else {
                    intBtnPauseGame.color = new Color(1, 1, 1, 0.6f);
                }
            }

            if (inspectInstantiated) {
                if (MouseIntersects(insBtnContinueRect)) {
                    insBtnContinue.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button4Control[0];
                }
                else {
                    insBtnContinue.color = new Color(1, 1, 1, 0.6f);
                }
                if (MouseIntersects(insBtnEndRect)) {
                    insBtnEnd.color = new Color(1, 1, 1, 0.8f);
                    helperText.text = playIn.button4Control[0];
                }
                else {
                    insBtnEnd.color = new Color(1, 1, 1, 0.6f);
                }
            }
        }

        if (!(MouseIntersects(dlgBtnContinueRect) || MouseIntersects(dlgBtnEndRect) ||  MouseIntersects(dlgBtnInterruptRect) || MouseIntersects(dlgBtnPauseGameRect) 
            || MouseIntersects(intBtnContinueRect) || MouseIntersects(intBtnEndRect) ||  MouseIntersects(intBtnPauseGameRect) 
            || MouseIntersects(insBtnContinueRect) || MouseIntersects(insBtnEndRect))) {
            helperText.text = "";
        }


        if (!dialogueInstantiated) {
            flow.SetBooleanVariable("dlgContinue", false);
            flow.SetBooleanVariable("dlgEnd", false);
            flow.SetBooleanVariable("dlgInterrupt", false);
            flow.SetBooleanVariable("dlgPauseGame", false);
        }
        if (!interactInstantiated) {
            flow.SetBooleanVariable("intContinue", false);
            flow.SetBooleanVariable("intEnd", false);
            flow.SetBooleanVariable("intPauseGame", false);
        }
        if (!inspectInstantiated) {
            flow.SetBooleanVariable("insContinue", false);
            flow.SetBooleanVariable("insEnd", false);
        }

        if (flow.GetBooleanVariable("intEnd") || flow.GetBooleanVariable("dlgEnd") || flow.GetBooleanVariable("insEnd")) {
            flow.SetBooleanVariable("dlgInstantiated", false);
            flow.SetBooleanVariable("insInstantiated", false);
            flow.SetBooleanVariable("intInstantiated", false);
            flow.StopAllBlocks();
            Flowchart.BroadcastFungusMessage("nothing");
        }

        if (!PauseMenu.paused){
            flow.SetBooleanVariable("dlgPauseGame", false);
            flow.SetBooleanVariable("intPauseGame", false);
        }
        
        if ((flow.GetBooleanVariable("dlgContinue") || flow.GetBooleanVariable("insContinue") || flow.GetBooleanVariable("intContinue")) && !flow.GetBooleanVariable("sayFinished") && playStatus.sheathed) {
            flow.SetBooleanVariable("dlgContinue", false);
            flow.SetBooleanVariable("intContinue", false);
            flow.SetBooleanVariable("insContinue", false);
            //do something to make the text print instantly
            dlgWriter.instantComplete = true;
            dlgWriter.inputFlag = true;
            intWriter.instantComplete = true;
            intWriter.inputFlag = true;
            insWriter.instantComplete = true;
            insWriter.inputFlag = true;
            resetOnce = true;
        } else if (flow.GetBooleanVariable("sayFinished") && resetOnce) {
            dlgWriter.instantComplete = false;
            dlgWriter.inputFlag = false;
            intWriter.instantComplete = false;
            intWriter.inputFlag = false;
            insWriter.instantComplete = false;
            insWriter.inputFlag = false;
            resetOnce = false;
        }
        

    }
}
