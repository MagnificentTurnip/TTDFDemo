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
    public TMPro.TextMeshProUGUI codeDisplay;

    public string currentSpellCode;
    public string displaySpellCode;
    public int reclickCounter;
    public bool repeatSpell; //bool that checks for a repeated spell - no copies of the same spell existing at a time, thanks
    public int misfireFrames; //when you attempt to cast a spell without MP for it, it initially misfires

    public bool b1release;
    public bool b2release;
    public bool b3release;
    public bool b4release;

    public void StopCasting() {
        for (int i = 0; i < runningSpells.Count; i++) {
            if (runningSpells[i] != null) {
                if (runningSpells[i].GetComponent<Spell>().castTime > 0) {
                    toDestroy = runningSpells[i];
                    runningSpells.Remove(runningSpells[i]);
                    toDestroy.GetComponent<Spell>().destroySpell();
                }
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
        if (playIn.spellcast && style.status.canCast() && style.status.castLock <= 0 && !style.status.casting && reclickCounter <= 0) { 
            currentSpellCode = "";
            displaySpellCode = "";
            reclickCounter = 15; //can't exit spellcasting for 15 frames 
            style.status.casting = true;
        }

        if (style.status.casting) {
            
            style.forceSpellcast(1); //set the spellcast state while casting

            //Spell input management - needs to be only on press to prevent large strings of the same command on a single tap of the button/key
            if (playIn.button1 && b1release) {
                currentSpellCode = currentSpellCode + "1"; //add the button's code to the spellCode
                displaySpellCode = displaySpellCode + playIn.button1Control;
                b1release = false;
            }
            if (playIn.button2 && b2release) {
                currentSpellCode = currentSpellCode + "2"; //add the button's code to the spellCode
                displaySpellCode = displaySpellCode + playIn.button2Control;
                b2release = false;
            }
            if (playIn.button3 && b3release) {
                currentSpellCode = currentSpellCode + "3"; //add the button's code to the spellCode
                displaySpellCode = displaySpellCode + playIn.button3Control;
                b3release = false;
            }
            if (playIn.button4 && b4release) {
                currentSpellCode = currentSpellCode + "4"; //add the button's code to the spellCode
                displaySpellCode = displaySpellCode + playIn.button4Control;
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

            //check to see if it's one of the funky spells that aren't cast with lmb and have the honour of being hardcoded into the spellbook
            if (playIn.evade && style.status.canCast() && style.status.castLock <= 0 && currentSpellCode == "") {
                //perform the magical evade
                style.movement.evade(style.movement.rollSpeed, 0f, style.movement.rollTime);
                style.animator.Play("fRoll");
                style.status.invulnerable = 30;
                style.status.Incorporealise(30);
                style.status.castLock = 30;
                style.stat.MP -= 30;
            }

            if (playIn.movement && style.status.canCast() && style.status.castLock <= 0) {
                //check to see if the current spellCode matches that of any of the player's known spells.
                for (int i = 0; i < spellsKnown.Count; i++) {
                    if (currentSpellCode == spellsKnown[i].GetComponent<Spell>().spellCode) {
                        if (style.stat.MP - spellsKnown[i].GetComponent<Spell>().cost < 0) {
                            misfireFrames = 30;
                        }
                        else {
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
                                currentSpell.Start();
                                style.status.castLock = currentSpell.castTime;
                                style.stat.MP -= currentSpell.cost;
                                currentSpellCode = "";
                                displaySpellCode = "";
                                runningSpells.Add(currentSpell.gameObject);
                                style.forceSpellcast(currentSpell.castTime + 5); //set the spellcast state for a while
                            }
                        }
                    }
                }
            }

            if (playIn.movement && style.status.canCast() && misfireFrames > 0 && misfireFrames < 21) {
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
                            currentSpell.Start();
                            style.status.castLock = currentSpell.castTime;
                            style.stat.MP -= currentSpell.cost;
                            currentSpellCode = "";
                            displaySpellCode = "";
                            runningSpells.Add(currentSpell.gameObject);
                            style.forceSpellcast(currentSpell.castTime + 5); //set the spellcast state for a while
                        }
                    }
                }
            }

            if (!playIn.spellcast && style.status.castLock <= 0 && reclickCounter <= 0) {
                style.status.casting = false;
                reclickCounter = 15; //can't re-enter spellcasting for 15 frames
                if (currentSpellCode == "") {
                    style.status.castLock = 2;
                } else {
                    //need time to dispel stuff if a spellcode is queued
                    style.status.castLock = 30;
                }
                currentSpellCode = "";
                displaySpellCode = "";
            }

        }
	}

    private void FixedUpdate() {
        codeDisplay.text = displaySpellCode;
        if (style.status.casting && style.status.castLock <= 0) {
            style.movement.pointToTarget(); //point to the mouse cursor while casting (unless castlocked) (in fixedUpdate because it looks weird to point places while paused)
        }
        if (style.status.spellFlinchTrigger) {
            currentSpellCode = "";
            displaySpellCode = "";
            StopCasting();
            style.status.spellFlinchTrigger = false;
        }
        if (reclickCounter > 0) {
            reclickCounter--;
        }
        if (misfireFrames > 0) {
            misfireFrames--;
            if (style.status.castLock < misfireFrames) {
                style.status.castLock = misfireFrames;
            }
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
             else if (runningSpells[i].GetComponent<Spell>().ready == 1) {
                runningSpells[i].GetComponent<Spell>().CastSpell();
            }
        }
    }
}
