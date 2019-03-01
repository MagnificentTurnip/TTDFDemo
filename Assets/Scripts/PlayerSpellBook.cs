using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellBook : MonoBehaviour {

    public List<GameObject> spellsKnown;
    public List<GameObject> runningSpells;
    public Spell currentSpell;
    public AtkStyle style;
    public PlayerInput playIn;
    public GameObject toDestroy;

    public string currentSpellCode;
    public int reclickCounter;
    public bool repeatSpell; //bool that checks for a repeated spell - no copies of the same spell existing at a time, thanks

    public bool b1release;
    public bool b2release;
    public bool b3release;
    public bool b4release;

    public void stopCasting() {
        for (int i = 0; i < runningSpells.Count; i++) {
            if (runningSpells[i].GetComponent<Spell>().castTime > 0) {
                runningSpells[i].GetComponent<Spell>().destroySpell();
            }
        }
    }

    // Use this for initialization
    void Start () {
        //?currentSpell = Instantiate(currentSpell).GetComponent<Spell>();
        b1release = true;
        b2release = true;
        b3release = true;
        b4release = true;
        reclickCounter = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (playIn.spellcast && style.status.canCast() && !style.status.casting && reclickCounter <= 0) { 
            currentSpellCode = "";
            reclickCounter = 15; //can't exit spellcasting for 15 frames 
            style.status.casting = true;
        }

        if (style.status.casting) {

            if (style.status.castLock <= 0) {
                style.movement.pointToTarget(); //point to the mouse cursor while casting (unless castlocked)
            }
            style.forceSpellcast(1); //set the spellcast state while casting

            //Spell input management - needs to be only on press to prevent large strings of the same command on a single tap of the button/key
            if (playIn.button1 && b1release) {
                currentSpellCode = currentSpellCode + "1"; //add the button's code to the spellCode
                b1release = false;
            }
            if (playIn.button2 && b2release) {
                currentSpellCode = currentSpellCode + "2"; //add the button's code to the spellCode
                b2release = false;
            }
            if (playIn.button3 && b3release) {
                currentSpellCode = currentSpellCode + "3"; //add the button's code to the spellCode
                b3release = false;
            }
            if (playIn.button4 && b4release) {
                currentSpellCode = currentSpellCode + "4"; //add the button's code to the spellCode
                b4release = false;
            }
            if (!playIn.button1) {
                b1release = true;
            }
            if (!playIn.button2) {
                b2release = true;
            }
            if (!playIn.button3) {
                b3release = true;
            }
            if (!playIn.button4) {
                b4release = true;
            }

            if (playIn.movement && style.status.canCast() && style.status.castLock <= 0) {
                //check to see if the current spellCode matches that of any of the player's known spells.
                for (int i = 0; i < spellsKnown.Count; i++) {
                    if (currentSpellCode == spellsKnown[i].GetComponent<Spell>().spellCode) {
                        repeatSpell = false;
                        for (int i2 = 0; i2 < runningSpells.Count; i2++) {
                            if (currentSpellCode == runningSpells[i2].GetComponent<Spell>().spellCode) {
                                repeatSpell = true;
                            }
                        }
                            if (!repeatSpell) {
                            currentSpell = Instantiate(spellsKnown[i]).GetComponent<Spell>();
                            currentSpell.debug = true; //testing mode
                            currentSpell.status = style.status;
                            currentSpell.charStat = style.charStat;
                            currentSpell.movement = style.movement;
                            currentSpell.transform.position = transform.position;
                            currentSpell.transform.rotation = transform.rotation;
                            currentSpell.active = true;
                            currentSpell.ready = 0;
                            style.status.castLock = currentSpell.castTime;
                            currentSpellCode = "";
                            runningSpells.Add(currentSpell.gameObject);
                            style.forceSpellcast(currentSpell.castTime + 5); //set the spellcast state for a while
                        }
                    }
                }
            }

            if (playIn.spellcast && style.status.canCast() && reclickCounter <= 0) {
                style.status.casting = false;
                reclickCounter = 15; //can't re-enter spellcasting for 15 frames
                if (currentSpellCode == "") {
                    style.status.castLock = 2;
                } else {
                    //need time to dispel stuff if a spellcode is queued
                    style.status.castLock = 30;
                }
            }

        }
	}

    private void FixedUpdate() {
        if (style.status.spellFlinchTrigger) {
            stopCasting();
            style.status.spellFlinchTrigger = false;
        }
        if (reclickCounter > 0) {
            reclickCounter--;
        }
        for (int i = 0; i < runningSpells.Count; i++) {
            runningSpells[i].GetComponent<Spell>().castTime--;
            if (runningSpells[i].GetComponent<Spell>().castTime <= 0 && runningSpells[i].GetComponent<Spell>().ready == 0) {
                runningSpells[i].GetComponent<Spell>().ready = 1;
            }
            if (runningSpells[i].GetComponent<Spell>().castTime <= 0 && runningSpells[i].GetComponent<Spell>().channelTime > 0) {
                runningSpells[i].GetComponent<Spell>().channelTime--;
            }
            if (runningSpells[i].GetComponent<Spell>().castTime <= 0 && runningSpells[i].GetComponent<Spell>().channelTime <= 0) {
                runningSpells[i].GetComponent<Spell>().duration--;
            }
            if (runningSpells[i].GetComponent<Spell>().duration <= 0) {
                toDestroy = runningSpells[i];
                runningSpells.Remove(runningSpells[i]);
                toDestroy.GetComponent<Spell>().destroySpell();
            }
            if (runningSpells[i].GetComponent<Spell>().ready == 1) {
                runningSpells[i].GetComponent<Spell>().CastSpell();
            }
        }
    }
}
