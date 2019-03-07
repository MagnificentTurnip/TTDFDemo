using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISliderManager : MonoBehaviour {

    public GameObject sliderObj;
    public StatusManager status;
    public StatSheet stat;

    public Canvas conditionCanvas;

    public bool beginInitialise;

    public Slider HPSlider;
    public Slider SPSlider;
    public Slider MPSlider;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI SPText;
    public TextMeshProUGUI MPText;

    public Slider currentSlider;
    public setConditionSlider currentSliderSettings;
    public List<GameObject> conditionSliders;
    public GameObject toDestroy;

    public bool invulnerableRunning;
    public bool flooredRunning;
    public bool vulnerableRunning;
    public bool silencedRunning;
    public bool airborneRunning;
    public bool stunnedRunning;
    public bool paralyzedRunning;
    public bool guardStunnedRunning;
    public bool parryStunnedRunning;
    public bool grappledRunning;

    // Use this for initialization
    void Start () {
        beginInitialise = false;
        invulnerableRunning = false;
        flooredRunning = false;
        vulnerableRunning = false;
        silencedRunning = false;
        airborneRunning = false;
        stunnedRunning = false;
        paralyzedRunning = false;
        guardStunnedRunning = false;
        parryStunnedRunning = false;
        grappledRunning = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate() {
        if (!beginInitialise) {
            HPSlider.minValue = 0;
            SPSlider.minValue = 0;
            MPSlider.minValue = 0;
            HPSlider.maxValue = Mathf.CeilToInt(stat.MaxHP);
            SPSlider.maxValue = Mathf.CeilToInt(stat.MaxSP);
            MPSlider.maxValue = Mathf.CeilToInt(stat.MaxMP);
            beginInitialise = true;
        }

        HPSlider.value = Mathf.CeilToInt(stat.HP);
        SPSlider.value = Mathf.CeilToInt(stat.SP);
        MPSlider.value = Mathf.CeilToInt(stat.MP);

        HPText.text = Mathf.CeilToInt(stat.HP).ToString() + " / " + Mathf.CeilToInt(stat.MaxHP).ToString();
        SPText.text = Mathf.CeilToInt(stat.SP).ToString() + " / " + Mathf.CeilToInt(stat.MaxSP).ToString();
        MPText.text = Mathf.CeilToInt(stat.MP).ToString() + " / " + Mathf.CeilToInt(stat.MaxMP).ToString();


        //condition sliders

        //invulnerable
        if (status.invulnerable > 0 && !invulnerableRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "invulnerableSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.invulnerable;
            currentSliderSettings.value = status.invulnerable;
            currentSliderSettings.titleTxt.text = "Invulnerable";
            conditionSliders.Add(currentSlider.gameObject);
            invulnerableRunning = true;
        }

        //floored
        if (status.floored > 0 && !flooredRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "flooredSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.floored;
            currentSliderSettings.value = status.floored;
            currentSliderSettings.titleTxt.text = "Floored";
            conditionSliders.Add(currentSlider.gameObject);
            flooredRunning = true;
        }

        //vulnerable
        if (status.vulnerable > 0 && !vulnerableRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "vulnerableSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.vulnerable;
            currentSliderSettings.value = status.vulnerable;
            currentSliderSettings.titleTxt.text = "Vulnerable";
            conditionSliders.Add(currentSlider.gameObject);
            vulnerableRunning = true;
        }

        //silenced
        if (status.silenced > 0 && !silencedRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "silencedSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.silenced;
            currentSliderSettings.value = status.silenced;
            currentSliderSettings.titleTxt.text = "Silenced";
            conditionSliders.Add(currentSlider.gameObject);
            silencedRunning = true;
        }

        //airborne
        if (status.airborne > 0 && !airborneRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "airborneSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.airborne;
            currentSliderSettings.value = status.airborne;
            currentSliderSettings.titleTxt.text = "Airborne";
            conditionSliders.Add(currentSlider.gameObject);
            airborneRunning = true;
        }

        //stunned
        if (status.stunned > 0 && !stunnedRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "stunnedSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.stunned;
            currentSliderSettings.value = status.stunned;
            currentSliderSettings.titleTxt.text = "Stunned";
            conditionSliders.Add(currentSlider.gameObject);
            stunnedRunning = true;
        }

        //paralyzed
        if (status.paralyzed > 0 && !paralyzedRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "paralyzedSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.paralyzed;
            currentSliderSettings.value = status.paralyzed;
            currentSliderSettings.titleTxt.text = "Paralyzed";
            conditionSliders.Add(currentSlider.gameObject);
            paralyzedRunning = true;
        }

        //guardstunned
        if (status.guardStunned > 0 && !guardStunnedRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "guardStunnedSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.guardStunned;
            currentSliderSettings.value = status.guardStunned;
            currentSliderSettings.titleTxt.text = "Guard-Stunned";
            conditionSliders.Add(currentSlider.gameObject);
            guardStunnedRunning = true;
        }

        //parrystunned
        if (status.parryStunned > 0 && !parryStunnedRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "parryStunnedSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.parryStunned;
            currentSliderSettings.value = status.parryStunned;
            currentSliderSettings.titleTxt.text = "Parry-Stunned";
            conditionSliders.Add(currentSlider.gameObject);
            parryStunnedRunning = true;
        }

        //grappled
        if (status.grappled > 0 && !grappledRunning) {
            currentSlider = Instantiate(sliderObj).GetComponent<Slider>();
            currentSlider.transform.SetParent(conditionCanvas.transform);
            currentSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -120 - (20 * conditionSliders.Count));
            currentSliderSettings = currentSlider.gameObject.GetComponent<setConditionSlider>();
            currentSlider.gameObject.tag = "grappledSlider";
            currentSlider.minValue = 0;
            currentSlider.maxValue = status.grappled;
            currentSliderSettings.value = status.grappled;
            currentSliderSettings.titleTxt.text = "Grappled";
            conditionSliders.Add(currentSlider.gameObject);
            grappledRunning = true;
        }


        for (int i = 0; i < conditionSliders.Count; i++) {

            //checking if a condition is over
            switch (conditionSliders[i].gameObject.tag) {
                case "invulnerableSlider":
                    if (status.invulnerable <= 0) {
                        invulnerableRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "flooredSlider":
                    if (status.floored <= 0) {
                        flooredRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "vulnerableSlider":
                    if (status.vulnerable <= 0) {
                        vulnerableRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "silencedSlider":
                    if (status.silenced <= 0) {
                        silencedRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "airborneSlider":
                    if (status.airborne <= 0) {
                        airborneRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "stunnedSlider":
                    if (status.stunned <= 0) {
                        stunnedRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "paralyzedSlider":
                    if (status.paralyzed <= 0) {
                        paralyzedRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "guardStunnedSlider":
                    if (status.guardStunned <= 0) {
                        guardStunnedRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "parryStunnedSlider":
                    if (status.parryStunned <= 0) {
                        parryStunnedRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
                case "grappledSlider":
                    if (status.grappled <= 0) {
                        grappledRunning = false;
                        toDestroy = conditionSliders[i];
                        conditionSliders.Remove(conditionSliders[i]);
                        Destroy(toDestroy);
                    }
                    break;
            }
            


            //checking if each condition is refreshed (THIS CAN BE MAYBE MORE EFFICIENT? PUT INSIDE THE IF STATUS.EG > 0 AND LOOP THROUGH THEN FOR EACH ONE)

                //invulnerable
            if (conditionSliders[i].tag.Contains("invulnerableSlider") && status.invulnerable > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.invulnerable;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.invulnerable;
            }

            //floored
            if (conditionSliders[i].tag.Contains("flooredSlider") && status.floored > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.floored;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.floored;
            }

            //vulnerable
            if (conditionSliders[i].tag.Contains("invulnerableSlider") && status.invulnerable > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.invulnerable;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.invulnerable;
            }

            //silenced
            if (conditionSliders[i].tag.Contains("vulnerableSlider") && status.vulnerable > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.vulnerable;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.vulnerable;
            }

            //airborne
            if (conditionSliders[i].tag.Contains("airborneSlider") && status.airborne > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.airborne;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.airborne;
            }

            //stunned
            if (conditionSliders[i].tag.Contains("stunnedSlider") && status.stunned > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.stunned;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.stunned;
            }

            //paralyzed
            if (conditionSliders[i].tag.Contains("paralyzedSlider") && status.paralyzed > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.paralyzed;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.paralyzed;
            }

            //guardStunned
            if (conditionSliders[i].tag.Contains("guardStunnedSlider") && status.guardStunned > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.guardStunned;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.guardStunned;
            }

            //parryStunned
            if (conditionSliders[i].tag.Contains("parryStunnedSlider") && status.parryStunned > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.parryStunned;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.parryStunned;
            }

            //grappled
            if (conditionSliders[i].tag.Contains("grappledSlider") && status.grappled > conditionSliders[i].GetComponent<setConditionSlider>().value) {
                conditionSliders[i].GetComponent<Slider>().maxValue = status.grappled;
                conditionSliders[i].GetComponent<setConditionSlider>().value = status.grappled;
            }



        }

        }

}
