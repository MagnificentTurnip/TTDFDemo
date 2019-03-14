using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hittable : MonoBehaviour {

    public GameObject damageNumber;
    public GameObject currentDamageNumber;

    public Camera cam;
    public Canvas damageNumberCanvas;

    public Attack currentAttack;
    public Motor motor;
    public StatusManager status;
    public StatSheet stat;

    public float towardAttackerAngle;

    //resistances?
    //public float slashingTaken = 0.9 for 10% slashing resistance or something?

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //Mathf.DeltaAngle(transform.localEulerAngles.y, toMouseAngle);
        //print(Vector3.Angle(transform.position, uhme.transform.position));
    }

    public virtual void applyHitProperties(Attack.hitProperties properties) {
        if (properties.damageInstances != null) {
            for (int i = 0; i < properties.damageInstances.Count; i++) { //loop through damage instances to apply them

                //if (currentAttack.onHit.damageInstances[i].damageType == Attack.typeOfDamage.Slashing) {
                currentDamageNumber = Instantiate(damageNumber);
                currentDamageNumber.GetComponent<RectTransform>().position = cam.WorldToScreenPoint(transform.position);
                currentDamageNumber.GetComponent<RectTransform>().SetParent(damageNumberCanvas.transform);
                currentDamageNumber.GetComponent<RectTransform>().Translate(Random.Range(-40, 40), Random.Range(-40, 40), 0);
                currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount)).ToString();

                stat.HP -= properties.damageInstances[i].damageAmount/* *= slashingTaken*/;
                //}

            }
        }
        

        if (properties.causesFlinch) {
            status.flinch();
            transform.LookAt(currentAttack.transform.parent, Vector3.up); //face the attacker that caused flinch;
            transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //lock rotation on x and z;
            //transform.LookAt(currentAttack.data.attackOwnerStatus.gameObject.transform.position, Vector3.up); //face the attacker that caused flinch;
        }

        if (properties.causesVulnerable + currentAttack.data.attackDuration > status.vulnerable && properties.causesVulnerable != 0) {
            status.vulnerable = properties.causesVulnerable + currentAttack.data.attackDuration;
        }

        if (properties.causesSilenced + currentAttack.data.attackDuration > status.silenced && properties.causesSilenced != 0) {
            status.silenced = properties.causesSilenced + currentAttack.data.attackDuration;
        }

        if (properties.causesFloored + currentAttack.data.attackDuration > status.floored && properties.causesFloored != 0) {
            status.floored = properties.causesFloored + currentAttack.data.attackDuration;
        }

        if (properties.causesAirborne + currentAttack.data.attackDuration > status.airborne && properties.causesAirborne != 0) {
            status.airborne = properties.causesAirborne + currentAttack.data.attackDuration;
        }

        if (properties.causesStun + currentAttack.data.attackDuration > status.stunned && properties.causesStun != 0) {
            status.stunned = properties.causesStun + currentAttack.data.attackDuration;
        }

        if (properties.causesParalyze + currentAttack.data.attackDuration > status.paralyzed && properties.causesParalyze != 0) {
            status.paralyzed = properties.causesParalyze + currentAttack.data.attackDuration;
        }

        if (properties.causesGrapple + currentAttack.data.attackDuration > status.grappled && properties.causesGrapple != 0) {
            status.grappled = properties.causesGrapple + currentAttack.data.attackDuration;
        }

        if (properties.causesGuardStun + currentAttack.data.attackDuration > status.guardStunned && properties.causesGuardStun != 0) {
            status.guardStunned = properties.causesGuardStun + currentAttack.data.attackDuration;
        }
    }

    private void OnTriggerStay(Collider col) {

        if (col.gameObject.tag.Contains("attack") && col.gameObject.transform.parent != this.gameObject.transform.parent) {

            currentAttack = col.gameObject.GetComponent<Attack>();

            //testing knockback
            Vector3 awayForce = Vector3.Normalize(transform.position - currentAttack.data.attackOwnerStatus.gameObject.transform.position);
            awayForce.y = 0f;

            if (currentAttack.data.attackDelay <= 0 && currentAttack.data.attackDuration > 0) {
                currentAttack.same = 0;
                for (int i = 0; i < currentAttack.thingsHit.Count; i++) {
                    if (this.gameObject == currentAttack.thingsHit[i]) {
                        currentAttack.same += 1;
                    }
                }

                if (currentAttack.same <= 0) { //if something has been hit and it wasn't something that was hit before
                    currentAttack.thingsHit.Add(this.gameObject);

                    //calculate the angle this unit needs to turn to be facing the attacker
                    towardAttackerAngle = Mathf.DeltaAngle(transform.localEulerAngles.y, Mathf.Atan2(currentAttack.data.attackOwnerStatus.transform.position.x - transform.position.x, currentAttack.data.attackOwnerStatus.transform.position.z - transform.position.z) * Mathf.Rad2Deg);

                    if (status.parryFrames > 0 && currentAttack.data.unblockable != 2 && currentAttack.data.unblockable != 3 && towardAttackerAngle >= -90 && towardAttackerAngle <= 90) {
                        //parry the attack if this unit is parrying, the attack can be parried, and this unit doesn't have its back facing the attacker

                        status.parryLock = 0; //undo the parryLock
                        //and the attack has no effect

                        if (currentAttack.data.contact == true) {
                            currentAttack.data.attackOwnerStatus.flinch();
                            if (status.parryFrames + 60 > currentAttack.data.attackOwnerStatus.parryStunned) {
                                currentAttack.data.attackOwnerStatus.parryStunned = status.parryFrames + 60;
                            }
                        }
                    }


                    else if ((status.guarding || status.isGuardStunned()) && status.invulnerable <= 0 && currentAttack.data.unblockable != 1 && currentAttack.data.unblockable != 3 && towardAttackerAngle >= -90 && towardAttackerAngle <= 90) {
                        //guard the attack if this unit is guarding and not invulnerable, the attack can be guarded, and this unit doesn't have its back facing the attacker

                        print("Guard"); //testing
                        applyHitProperties(currentAttack.onGuard);

                        //awayForce = -awayForce * currentAttack.onGuard.onHitForwardBackward; //testing knockback, change this to onHitAwayToward when implemented
                        //motor.rb.AddForce(awayForce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onGuard.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onGuard.onHitRightLeft);

                    }
                    else if (status.isVulnerable() && status.invulnerable <= 0) {
                        //apply attack

                        print("VulnerableHit"); //testing

                        //consume vulnerable status
                        status.vulnerable = 0;

                        applyHitProperties(currentAttack.onVulnerableHit);

                        //testforce = -testforce * currentAttack.onVulnerableHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onVulnerableHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onVulnerableHit.onHitRightLeft);

                    }
                    else if (status.isFloored() && status.invulnerable <= 0) {
                        //apply attack

                        print("FlooredHit"); //testing

                        applyHitProperties(currentAttack.onFlooredHit);

                        //testforce = -testforce * currentAttack.onFlooredHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onFlooredHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onFlooredHit.onHitRightLeft);

                    }
                    else if (status.isAirborne() && status.invulnerable <= 0) {
                        //apply attack

                        print("AirborneHit"); //testing

                        applyHitProperties(currentAttack.onAirborneHit);
                        
                        //testforce = -testforce * currentAttack.onAirborneHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onAirborneHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onAirborneHit.onHitRightLeft);

                    }
                    else {
                        //apply attack
                        if (status.invulnerable <= 0) {
                            print("Hit"); //testing

                            applyHitProperties(currentAttack.onHit);

                            //testforce = -testforce * currentAttack.onHit.onHitForwardBackward; //testing knockback
                            //motor.rb.AddForce(testforce);

                            motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onHit.onHitForwardBackward);
                            motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onHit.onHitRightLeft);
                        }

                    }
                } //else do nothing
            }
            
        }

    }

}
