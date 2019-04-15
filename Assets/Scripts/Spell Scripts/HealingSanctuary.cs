using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSanctuary : Spell {

    public Camera cam;
    Ray playInRay;
    RaycastHit playInHit;

    public bool init;

    public float healPerSec = 5;
    
	// Use this for initialization
	public override void Start () {
        base.Start();
        cam = Camera.main;
        spellName = "Healing Sanctuary";
        spellCode = "44141";
        castTime = 120;
        postCastTime = 60;
        channelTime = 0;
        //duration = 1800;
        cost = 80; //COST MUST BE PRE-SET IN THE PREFAB TO ALLOW MISFIRING
    }

    public override void CastSpell() {
        base.CastSpell();

        
    }

    // Update is called once per rendered frame
    public override void Update () {

    }

    // FixedUpdate is called once per physics frame
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (!init) {
            //place the thundercloud over the current target
            if (mouseTarget) {
                playInRay = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(playInRay, out playInHit, 100, 1 << LayerMask.NameToLayer("Terrain"))) {
                    playInHit.point = new Vector3(playInHit.point.x, 0, playInHit.point.z);
                    transform.position = playInHit.point;
                }
            }
            else {
                transform.position = new Vector3(target.transform.position.x, 0.21f, target.transform.position.z);
            }
            healPerSec = 5;
            init = true;
        }
    }


    public void OnTriggerStay(Collider other) {
        if (ready == 2) {
            if (other.gameObject.GetComponent<StatSheet>()) {
                if (other.gameObject.GetComponent<StatSheet>().HP < other.gameObject.GetComponent<StatSheet>().MaxHP) {
                    other.gameObject.GetComponent<StatSheet>().HP += healPerSec / 60;
                }
            }
            if (other.gameObject.GetComponent<CharStatSheet>()) {
                if (other.gameObject.GetComponent<CharStatSheet>().HP < other.gameObject.GetComponent<CharStatSheet>().MaxHP) {
                    other.gameObject.GetComponent<CharStatSheet>().HP += healPerSec / 60;
                }
            }
        }
    }


}
