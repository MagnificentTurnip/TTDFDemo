using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour {

    public GameObject tooltipElementPrefab;

    public List<TooltipElement> tooltipElements;
    public TooltipElement currentElement;

    public GameObject toDestroy;
    public Transform playerTransform;

	// Use this for initialization
	void Start () {
        tooltipElements = new List<TooltipElement>();
    }
	
	public void NewElement(string inText, string actText, TooltipElement.InputDirection direction) {
        currentElement = Instantiate(tooltipElementPrefab).GetComponent<TooltipElement>();
        currentElement.playerTransform = playerTransform;
        currentElement.transform.SetParent(transform, false);
        currentElement.transform.Translate(0, tooltipElements.Count * -50, 0);
        currentElement.inputText.text = inText;
        currentElement.actionText.text = actText;
        currentElement.directionType = direction;
        tooltipElements.Add(currentElement);
    }

    public void ClearElements() {
        for (int i = 0; i < tooltipElements.Count; i++) {
            toDestroy = tooltipElements[i].gameObject;
            Destroy(toDestroy.gameObject);
        }
        tooltipElements.Clear();
    }
    
}
