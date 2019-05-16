using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FungusBtnExtend : MonoBehaviour {

    public UnityEngine.Events.UnityEvent onButtonPressed;
    public CommInputManager commIn;

    public UnityEngine.UI.Button button;
    RectTransform buttonRect;
    BoxCollider2D buttonBox;

    public bool dlgTRUEintFALSE;

    Rect rect;

    public bool MouseIntersects(RectTransform transformRect) {
        Rect rect = new Rect(new Vector2(transformRect.position.x - (transformRect.rect.width / 4), transformRect.position.y), new Vector2(transformRect.rect.width / 2, transformRect.rect.height / 2));
        if (rect.Contains(Input.mousePosition)) {
            return true;
        }
        else {
            return false;
        }
    }

    // Use this for initialization
    void Start () {
        buttonRect = button.gameObject.GetComponent<RectTransform>();
        buttonBox = button.gameObject.GetComponent<BoxCollider2D>();
        rect = new Rect(new Vector2(buttonRect.position.x, buttonRect.position.y - (buttonBox.size.y / 8)), new Vector2(buttonRect.rect.width * 0.75f, buttonRect.rect.height * 0.75f));
    }
	
	// Update is called once per frame
	void Update () {
        /* commented out helper text because activating menu options with buttons is disabled to prevent bugs
        rect = new Rect(new Vector2(buttonRect.position.x - ((buttonRect.rect.width / 8) * 3), buttonRect.position.y - (buttonRect.rect.height / 4)), new Vector2(buttonRect.rect.width * 0.75f, buttonRect.rect.height * 0.75f));

        if (dlgTRUEintFALSE) {
            if (rect.Contains(Input.mousePosition)) {
                commIn.helperText.text = commIn.playIn.button3Control[0];
                if (commIn.b3New) {
                    onButtonPressed.Invoke(); 
                }
            }
        } else {
            if (rect.Contains(Input.mousePosition)) {
                commIn.helperText.text = commIn.playIn.button4Control[0];
                if (commIn.b4New) {
                    onButtonPressed.Invoke();
                }
            }
        }
        */
	}
    

}
