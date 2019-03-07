using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class setConditionSlider : MonoBehaviour {
    
    public int value;
    public TextMeshProUGUI titleTxt;
    public TextMeshProUGUI valTxt;
    public Slider slider;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        valTxt.text = value.ToString();
        slider.value = value;
        value--;
	}
}
