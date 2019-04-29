using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipElement : MonoBehaviour {

    public TextMeshProUGUI inputText;
    public TextMeshProUGUI actionText;
    public GameObject directionInput;

    public Transform playerTransform;

    public enum InputDirection {none, forward, back, left, right}

    public InputDirection directionType;

    private void Start() {
        
        if (directionType == InputDirection.none) {
            directionInput.gameObject.SetActive(false);
        }
        else {
            if (playerTransform != null) {
                directionInput.gameObject.SetActive(true);
                switch (directionType) {
                    case InputDirection.forward:
                        directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y));
                        break;

                    case InputDirection.back:
                        directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y + 180f));
                        break;

                    case InputDirection.left:
                        directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y - 90f));
                        break;

                    case InputDirection.right:
                        directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y + 90f));
                        break;
                }
            }
        } 
    }

    // Update is called once per frame
    void Update() {
        if (directionType == InputDirection.none) {
            directionInput.gameObject.SetActive(false);
        }
        else {
            directionInput.gameObject.SetActive(true);
            switch (directionType) {
                case InputDirection.forward:
                    directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y));
                    break;

                case InputDirection.back:
                    directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y + 180f));
                    break;

                case InputDirection.left:
                    directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y - 90f));
                    break;

                case InputDirection.right:
                    directionInput.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -playerTransform.eulerAngles.y + 90f));
                    break;
            }
        }
    }

}
