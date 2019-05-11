using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    Vector3 mousePos; //a vector for the current position of the mouse on screen
    Vector3 objectPos; //a vector for the current position of the object on screen
    public Camera cam; //going to need to put the main camera in here, because for some reason unity currently hates using Camera.main
    public float toMouseAngle;

    Ray ray;
    RaycastHit hit;
    public GameObject currentTarget;
    public int targetRefresh1; //2 target refreshes, the first falls off very shortly after mousing off
    public int targetRefresh2; //the other stays a while longer
    //targetRefresh1 is to be used for actual targetting of things, targetRefresh2 should be used mostly if not entirely for UI

    public float cursorDistance;
    public float mouseDifAngle;

    //the strings for the axes that control which controls actually effect certain game states
    public List<string> movementControl = new List<string>();
    public List<string> spellcastControl = new List<string>();
    public List<string> fParryControl = new List<string>();
    public List<string> bParryControl = new List<string>();
    public List<string> guardControl = new List<string>();
    public List<string> sprintControl = new List<string>();
    public List<string> evadeControl = new List<string>();
    public List<string> sheatheControl = new List<string>();
    public List<string> button1Control = new List<string>();
    public List<string> button2Control = new List<string>();
    public List<string> button3Control = new List<string>();
    public List<string> button4Control = new List<string>();

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

    void CalculateMouseAngle() { //calculates the angle between the mouse and the way the player is facing. This calculates two useful values.
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

        targetRefresh1 = 0;

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

        cursorDistance = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), Input.mousePosition); //get distance from player to cursor

        CalculateMouseAngle(); //calculate the angles for the mouse so we can then discern where it's pointing

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

        //targeting system
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, ~(1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Walls") | 1 << LayerMask.NameToLayer("Spells")))) { //Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Terrain"))
            if (hit.collider.gameObject.tag.Contains("Tgt")) {
                currentTarget = hit.collider.gameObject;
                targetRefresh1 = 8;
                targetRefresh2 = 600;
            }
        }

        //REGISTERING INPUT VVVV

        movement = false;
        for (int i = 0; i < movementControl.Count; i++) {
            if (Input.GetAxis(movementControl[i]) > 0f) {
                movement = true;
            }
        }

        spellcast = false;
        for (int i = 0; i < spellcastControl.Count; i++) {
            if (Input.GetAxis(spellcastControl[i]) > 0f) {
                spellcast = true;
            }
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
            fParry = false;
            for (int i = 0; i < fParryControl.Count; i++) {
                if (Input.GetAxis(fParryControl[i]) > 0f) {
                    fParry = true;
                }
            }
            bParry = false;
            for (int i = 0; i < bParryControl.Count; i++) {
                if (Input.GetAxis(bParryControl[i]) > 0f) {
                    bParry = true;
                }
            }
        }

        guard = false;
        for (int i = 0; i < guardControl.Count; i++) {
            if (Input.GetAxis(guardControl[i]) > 0f) {
                guard = true;
            }
        }

        sprint = false;
        for (int i = 0; i < sprintControl.Count; i++) {
            if (Input.GetAxis(sprintControl[i]) > 0f) {
                sprint = true;
            }
        }

        evade = false;
        for (int i = 0; i < evadeControl.Count; i++) {
            if (Input.GetAxis(evadeControl[i]) > 0f) {
                evade = true;
            }
        }

        sheathe = false;
        for (int i = 0; i < sheatheControl.Count; i++) {
            if (Input.GetAxis(sheatheControl[i]) > 0f) {
                sheathe = true;
            }
        }

        button1 = false;
        for (int i = 0; i < button1Control.Count; i++) {
            if (Input.GetAxis(button1Control[i]) > 0f) {
                button1 = true;
            }
        }

        button2 = false;
        for (int i = 0; i < button2Control.Count; i++) {
            if (Input.GetAxis(button2Control[i]) > 0f) {
                button2 = true;
            }
        }

        button3 = false;
        for (int i = 0; i < button3Control.Count; i++) {
            if (Input.GetAxis(button3Control[i]) > 0f) {
                button3 = true;
            }
        }

        button4 = false;
        for (int i = 0; i < button4Control.Count; i++) {
            if (Input.GetAxis(button4Control[i]) > 0f) {
                button4 = true;
            }
        }
    }

    private void FixedUpdate() {
        if (targetRefresh1 > 0) { 
            targetRefresh1--;
        } else {
            currentTarget = null;
        }
        if (targetRefresh2 > 0) { 
            targetRefresh2--;
        }
    }
}
