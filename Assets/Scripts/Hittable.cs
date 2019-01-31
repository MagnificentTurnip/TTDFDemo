using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour {

    public Attack currentAttack;
    public Motor motor;
    public StatusManager status;
    public StatSheet stat;

    //resistances?
    //public float slashingTaken = 0.9 for 10% slashing resistance or something?

    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    
    private void OnTriggerEnter(Collider col) {

        if (col.gameObject.tag.Contains("attack") && col.gameObject.transform.parent != this.gameObject.transform.parent) {
            currentAttack = col.gameObject.GetComponent<Attack>();
            currentAttack.same = 0;
            for (int i = 0; i < currentAttack.thingsHit.Count; i++) {
                if (this.gameObject == currentAttack.thingsHit[i]) {
                    currentAttack.same += 1;
                }
            }
            if (currentAttack.same <= 0) { //if something has been hit and it wasn't something that was hit before
                currentAttack.thingsHit.Add(this.gameObject);
                if (status.parryFrames > 0) {
                    //parry the attack
                }
                else if (status.guarding) {
                    //guard the attack
                }
                else {
                    //apply attack

                    print("Hit"); //testing

                    for (int i = 0; i < currentAttack.onHit.damageInstances.Count; i++) { //loop through damage instances to apply them
                        //if (currentAttack.onHit.damageInstances[i].damageType == Attack.typeOfDamage.Slashing) {
                        stat.HP -= currentAttack.onHit.damageInstances[i].damageAmount/* *= slashingTaken*/;
                        //}
                        //stun equal to currentAttack.onHit.causesStun + currentAttack.data.attackDuration?
                    }

                }
            } //else do nothing
        }

    }

}
