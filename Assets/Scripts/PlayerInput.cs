using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    Vector3 mousePos; //a vector for the current position of the mouse on screen
    Vector3 objectPos; //a vector for the current position of the object on screen
    public Camera cam; //going to need to put the main camera in here, because for some reason unity currently hates using Camera.main
    public float toMouseAngle; 

    public float cursorDistance;
    public float mouseDifAngle;

    //the strings for the axes that control which controls actually effect certain game states
    public string movementControl;
    public string spellcastControl;
    public string fParryControl;
    public string bParryControl;
    public string guardControl;
    public string sprintControl;
    public string evadeControl;
    public string sheatheControl;
    public string button1Control;
    public string button2Control;
    public string button3Control;
    public string button4Control;

    public bool parryMode; //if true, then parry uses the scrollwheel input. Otherwise, it uses the axes under fParryControl and bParryControl.

    //the actual boolean states for those controls
    public bool movement;
    public bool spellcast;
    public bool fParry;
    public bool bParry;
    public bool guard;
    public bool sprint;
    public bool evade;
    public bool sheathe;
    public bool button1;
    public bool button2;
    public bool button3;
    public bool button4;
    public bool pointForward; //the pointing states are controls but they are always controlled by the mouse until controller support comes in but I'll figure that out when I get to it
    public bool pointRight;
    public bool pointLeft;
    public bool pointBack;

    //Combination controls, used by attack styles
    public KeyCombo fwdA;
    public KeyCombo fwdS;
    public KeyCombo fwdD;
    public KeyCombo fwdAS;
    public KeyCombo fwdAD;
    public KeyCombo fwdSD;
    public KeyCombo fwdASD;
    
    public KeyCombo bakA;
    public KeyCombo bakS;
    public KeyCombo bakD;
    public KeyCombo bakAS;
    public KeyCombo bakAD;
    public KeyCombo bakSD;
    public KeyCombo bakASD;
    
    public KeyCombo rgtA;
    public KeyCombo rgtS;
    public KeyCombo rgtD;
    public KeyCombo rgtAS;
    public KeyCombo rgtAD;
    public KeyCombo rgtSD;
    public KeyCombo rgtASD;
    
    public KeyCombo lftA;
    public KeyCombo lftS;
    public KeyCombo lftD;
    public KeyCombo lftAS;
    public KeyCombo lftAD;
    public KeyCombo lftSD;
    public KeyCombo lftASD;

    void calculateMouseAngle() { //calculates the angle between the mouse and the way the player is facing. This calculates two useful values.
        mousePos = Input.mousePosition;
        mousePos.z = 5.23f;
        objectPos = cam.WorldToScreenPoint(transform.position); //gets the player's position on screen
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        //those two useful values are:
        //the toMouseAngle, which is the angle of the mouse in relation to the centre of the screen, clockwise. Up is 0, right is 90, left is -90, back is +-180.
        toMouseAngle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
        //and:
        //the mouseDifAngle, the difference in degrees between the toMouseAngle and the direction the player faces. In front of the player is is +-0, their right is 90, their left is -90 and behind them is +-180.
        mouseDifAngle = Mathf.DeltaAngle(transform.localEulerAngles.y, toMouseAngle);

        //print(mouseDifAngle); //print things for testing
    }

	// Use this for initialization
	void Start () {
        //initialise all those bools to false
        movement = false;
        spellcast = false;
        fParry = false;
        bParry = false;
        guard = false;
        sprint = false;
        evade = false;
        button1 = false;
        button2 = false;
        button3 = false;
        button4 = false;
        pointForward = false;
        pointRight = false;
        pointLeft = false;
        pointBack = false;

        fwdA = new KeyCombo(new string[] { "fwd", "a" }, this.gameObject);
        fwdS = new KeyCombo(new string[] { "fwd", "s" }, this.gameObject);
        fwdD = new KeyCombo(new string[] { "fwd", "d" }, this.gameObject);
        fwdAS = new KeyCombo(new string[] { "fwd", "a", "s" }, this.gameObject);
        fwdAD = new KeyCombo(new string[] { "fwd", "a", "d" }, this.gameObject);
        fwdSD = new KeyCombo(new string[] { "fwd", "s", "d" }, this.gameObject);
        fwdASD = new KeyCombo(new string[] { "fwd", "a", "s", "d" }, this.gameObject);

        bakA = new KeyCombo(new string[] { "bak", "a" }, this.gameObject);
        bakS = new KeyCombo(new string[] { "bak", "s" }, this.gameObject);
        bakD = new KeyCombo(new string[] { "bak", "d" }, this.gameObject);
        bakAS = new KeyCombo(new string[] { "bak", "a", "s" }, this.gameObject);
        bakAD = new KeyCombo(new string[] { "bak", "a", "d" }, this.gameObject);
        bakSD = new KeyCombo(new string[] { "bak", "s", "d" }, this.gameObject);
        bakASD = new KeyCombo(new string[] { "bak", "a", "s", "d" }, this.gameObject);

        rgtA = new KeyCombo(new string[] { "rgt", "a" }, this.gameObject);
        rgtS = new KeyCombo(new string[] { "rgt", "s" }, this.gameObject);
        rgtD = new KeyCombo(new string[] { "rgt", "d" }, this.gameObject);
        rgtAS = new KeyCombo(new string[] { "rgt", "a", "s" }, this.gameObject);
        rgtAD = new KeyCombo(new string[] { "rgt", "a", "d" }, this.gameObject);
        rgtSD = new KeyCombo(new string[] { "rgt", "s", "d" }, this.gameObject);
        rgtASD = new KeyCombo(new string[] { "rgt", "a", "s", "d" }, this.gameObject);

        lftA = new KeyCombo(new string[] { "lft", "a" }, this.gameObject);
        lftS = new KeyCombo(new string[] { "lft", "s" }, this.gameObject);
        lftD = new KeyCombo(new string[] { "lft", "d" }, this.gameObject);
        lftAS = new KeyCombo(new string[] { "lft", "a", "s" }, this.gameObject);
        lftAD = new KeyCombo(new string[] { "lft", "a", "d" }, this.gameObject);
        lftSD = new KeyCombo(new string[] { "lft", "s", "d" }, this.gameObject);
        lftASD = new KeyCombo(new string[] { "lft", "a", "s", "d" }, this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        //print(fwdA.Check());
        //print(fwdS.Check());
        //print(fwdAS.Check());

        cursorDistance = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), Input.mousePosition); //get distance from player to cursor

        calculateMouseAngle(); //calculate the angles for the mouse so we can then discern where it's pointing

        //set the bools for input of where the cursor is pointed relative to the player for use in controls
        if (45 >= mouseDifAngle && mouseDifAngle > -45) { //between 45 and -45 degrees, point forward
            pointForward = true;
            pointRight = false;
            pointLeft = false;
            pointBack = false;
            //print("fwd");
        } else if (135 >= mouseDifAngle && mouseDifAngle > 45) { //between 45 and 135 degrees, point right
            pointForward = false;
            pointRight = true;
            pointLeft = false;
            pointBack = false;
            //print("rgt");
        } else if (-45 >= mouseDifAngle && mouseDifAngle > -135) { //between -45 and -135 degrees, point left
            pointForward = false;
            pointRight = false;
            pointLeft = true;
            pointBack = false;
            //print("left");
        } else  { //otherwise, point back
            pointForward = false;
            pointRight = false;
            pointLeft = false;
            pointBack = true;
            //print("bak");
        }


        if (Input.GetAxis(movementControl) > 0f) { //movement controls
            movement = true;
        } else {
            movement = false;
        }

        if (Input.GetAxis(spellcastControl) > 0f) { //spellcast controls
            spellcast = true;
        } else {
            spellcast = false;
        }

        if (parryMode) { //parrying controls (many, many controls)
            if (Input.GetAxis("Mouse ScrollWheel") > 0) { 
                fParry = true;
            } else {
                fParry = false;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                bParry = true;
            } else {
                bParry = false;
            }
        } else {
            if (Input.GetAxis(fParryControl) > 0f) {
                fParry = true;
            }
            else {
                fParry = false;
            }
            if (Input.GetAxis(bParryControl) > 0f) {
                bParry = true;
            }
            else {
                bParry = false;
            }
        }

        if (Input.GetAxis(guardControl) > 0f) { //guard controls
            guard = true;
        }
        else {
            guard = false;
        }

        if (Input.GetAxis(sprintControl) > 0f) { //sprint controls
            sprint = true;
        }
        else {
            sprint = false;
        }

        if (Input.GetAxis(evadeControl) > 0f) { //evasion controls
            evade = true;
        }
        else {
            evade = false;
        }

        if (Input.GetAxis(sheatheControl) > 0f) { //evasion controls
            sheathe = true;
        }
        else {
            sheathe = false;
        }

        if (Input.GetAxis(button1Control) > 0f) { //button 1 controls
            button1 = true;
        }
        else {
            button1 = false;
        }

        if (Input.GetAxis(button2Control) > 0f) { //button 2 controls
            button2 = true;
        }
        else {
            button2 = false;
        }

        if (Input.GetAxis(button3Control) > 0f) { //button 3 controls
            button3 = true;
        }
        else {
            button3 = false;
        }

        if (Input.GetAxis(button4Control) > 0f) { //button 4 controls
            button4 = true;
        }
        else {
            button4 = false;
        }

    }
}
