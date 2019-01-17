using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCombo { //Created by StarManta, viewable at http://wiki.unity3d.com/index.php/KeyCombo

    public PlayerInput playIn;
    public string[] buttons;
    private int currentIndex = 0; //moves along the array as buttons are pressed

    public float allowedTimeBetweenButtons = 0.3f; //tweak as needed
    private float timeLastButtonPressed;

    public KeyCombo(string[] b, GameObject playerInput) {
        buttons = b;
        playIn = playerInput.GetComponent<PlayerInput>();
    }

    //usage: call this once a frame. when the combo has been completed, it will return true
    public bool Check() {
        if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex = 0;
        {
            if (currentIndex < buttons.Length) {
                if ((buttons[currentIndex] == "a" 
                    && playIn.button1) ||
                (buttons[currentIndex] == "s" && playIn.button2) ||
                (buttons[currentIndex] == "d" && playIn.button3) ||
                (buttons[currentIndex] == "f" && playIn.button4) ||
                (buttons[currentIndex] == "fwd" && playIn.pointForward) ||
                (buttons[currentIndex] == "rgt" && playIn.pointRight) ||
                (buttons[currentIndex] == "lft" && playIn.pointLeft) ||
                (buttons[currentIndex] == "bak" && playIn.pointBack) ||

                (buttons[currentIndex] != "a" && buttons[currentIndex] != "s" && buttons[currentIndex] != "d" 
                && buttons[currentIndex] != "f" && buttons[currentIndex] != "fwd" && buttons[currentIndex] != "rgt" 
                && buttons[currentIndex] != "lft" && buttons[currentIndex] != "bak" && Input.GetButtonDown(buttons[currentIndex]))) {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;
                }
                if (currentIndex >= buttons.Length) {
                    currentIndex = 0;
                    return true;
                }
                else return false;
            }
        }

        return false;
    }
}


/*
 * 
 * 
if (currentIndex < buttons.Length) {
                if ((buttons[currentIndex] == "down" && Input.GetAxisRaw("Vertical") == -1) ||
                (buttons[currentIndex] == "up" && Input.GetAxisRaw("Vertical") == 1) ||
                (buttons[currentIndex] == "left" && Input.GetAxisRaw("Vertical") == -1) ||
                (buttons[currentIndex] == "right" && Input.GetAxisRaw("Horizontal") == 1) ||
                (buttons[currentIndex] != "down" && buttons[currentIndex] != "up" && buttons[currentIndex] != "left" && buttons[currentIndex] != "right" && Input.GetButtonDown(buttons[currentIndex]))) {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;
                }
                

                */
