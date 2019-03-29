using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hittable : MonoBehaviour {

    //Damage resistances (or vulnerabilities)
    public float slashingTaken;
    public float impactTaken;
    public float piercingTaken;
    public float fireTaken;
    public float coldTaken;
    public float causticTaken;
    public float shockTaken;
    public float astralTaken;
    public float ruinousTaken;
    public float magicTaken;
    public float SPdamageTaken;
    public float MPdamageTaken;

    public GameObject damageNumber;
    public GameObject currentDamageNumber;

    public Light currentLight;
    public GameObject hitLight;
    public GameObject guardLight;
    public GameObject parryLight;

    public Camera cam;
    public Canvas damageNumberCanvas;

    public bool actuallyHits;

    public Attack currentAttack;
    public Motor motor;
    public StatusManager status;
    public StatSheet stat;

    public HitLag hitlag;

    public StatusManager hitLagStatus;
    public float hitLagTime;

    public float towardAttackerAngle;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //Mathf.DeltaAngle(transform.localEulerAngles.y, toMouseAngle);
        //print(Vector3.Angle(transform.position, uhme.transform.position));
    }

    public IEnumerator HitLag(Animator animator, float newSpeed, float lagTime) {
        animator.speed = newSpeed;
        yield return new WaitForSeconds(lagTime);
        animator.speed = 2f - newSpeed;
        yield return new WaitForSeconds(lagTime);
        animator.speed = 1f;
    }


    public virtual void applyHitProperties(Attack.hitProperties properties) {


        if (properties.damageInstances != null) {
            for (int i = 0; i < properties.damageInstances.Count; i++) { //loop through damage instances to apply them
                currentDamageNumber = Instantiate(damageNumber);
                /*
                currentDamageNumber.GetComponent<RectTransform>().position = cam.WorldToScreenPoint(transform.position);
                currentDamageNumber.GetComponent<RectTransform>().SetParent(damageNumberCanvas.transform);
                currentDamageNumber.GetComponent<RectTransform>().Translate(Random.Range(-40, 40), Random.Range(-40, 40), 0);
                */
                currentDamageNumber.transform.position = cam.WorldToScreenPoint(transform.position);
                currentDamageNumber.transform.SetParent(damageNumberCanvas.transform);
                currentDamageNumber.transform.Translate(Random.Range(-40, 40), Random.Range(-40, 40), 0);


                switch (properties.damageInstances[i].damageType) {
                    case Attack.typeOfDamage.Slashing:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * slashingTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * slashingTaken;
                        break;
                    case Attack.typeOfDamage.Impact:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * impactTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * impactTaken;
                        break;
                    case Attack.typeOfDamage.Piercing:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * piercingTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * piercingTaken;
                        break;
                    case Attack.typeOfDamage.Fire:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * fireTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * fireTaken;
                        break;
                    case Attack.typeOfDamage.Cold:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * coldTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * coldTaken;
                        break;
                    case Attack.typeOfDamage.Caustic:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * causticTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * causticTaken;
                        break;
                    case Attack.typeOfDamage.Shock:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * shockTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * shockTaken;
                        break;
                    case Attack.typeOfDamage.Astral:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * astralTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * astralTaken;
                        break;
                    case Attack.typeOfDamage.Ruinous:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * ruinousTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * ruinousTaken;
                        break;
                    case Attack.typeOfDamage.Magic:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * magicTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * magicTaken;
                        break;
                    case Attack.typeOfDamage.SPdamage:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.SP) - Mathf.CeilToInt(stat.SP - properties.damageInstances[i].damageAmount * SPdamageTaken)).ToString();
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().color = new Color(170, 255, 70);
                        stat.SP -= properties.damageInstances[i].damageAmount * SPdamageTaken;
                        break;
                    case Attack.typeOfDamage.MPdamage:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.MP) - Mathf.CeilToInt(stat.MP - properties.damageInstances[i].damageAmount * MPdamageTaken)).ToString();
                        stat.MP -= properties.damageInstances[i].damageAmount * MPdamageTaken;
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().color = new Color(70, 70, 255);
                        break;
                }
                

            }
        }
        
        if (properties.causesFlinch) {
            status.flinch();
            transform.LookAt(currentAttack.transform.parent, Vector3.up); //face the attack that caused flinch;
            transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //lock rotation on x and z;
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

                actuallyHits = false;
                if (status.invulnerable <= 0) {
                    if (status.isAirborne()) {
                        if (currentAttack.data.hitsAirborne) {
                            actuallyHits = true;
                        }
                    } else if (status.isFloored()) {
                        if (currentAttack.data.hitsFloored) {
                            actuallyHits = true;
                        }
                    } else if (currentAttack.data.hitsStanding) {
                        actuallyHits = true;
                    }
                }

                if (currentAttack.same <= 0 && actuallyHits) { //if something has been hit and it wasn't something that was hit before
                    currentAttack.thingsHit.Add(this.gameObject);

                    //calculate the angle this unit needs to turn to be facing the attacker
                    towardAttackerAngle = Mathf.DeltaAngle(transform.localEulerAngles.y, Mathf.Atan2(currentAttack.data.attackOwnerStatus.transform.position.x - transform.position.x, currentAttack.data.attackOwnerStatus.transform.position.z - transform.position.z) * Mathf.Rad2Deg);

                    if (status.parryFrames > 0 && currentAttack.data.unblockable != 2 && currentAttack.data.unblockable != 3 && towardAttackerAngle >= -90 && towardAttackerAngle <= 90 && Vector3.Distance(currentAttack.transform.position, transform.position) > 0.2f) {
                        //parry the attack if this unit is parrying, the attack can be parried, this unit doesn't have its back facing the attacker, and the attack isn't basically directly on top of the unit

                        status.parryLock = 0; //undo the parryLock
                        //and the attack has no effect

                        currentLight = Instantiate(parryLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        if (currentAttack.data.contact == true) {
                            currentAttack.data.attackOwnerStatus.flinch();
                            if (status.parryFrames + 60 > currentAttack.data.attackOwnerStatus.parryStunned) {
                                currentAttack.data.attackOwnerStatus.parryStunned = status.parryFrames + 60;
                            }
                        }
                    }


                    else if ((status.guarding || status.isGuardStunned()) && currentAttack.data.unblockable != 1 && currentAttack.data.unblockable != 3 && towardAttackerAngle >= -90 && towardAttackerAngle <= 90 && Vector3.Distance(currentAttack.transform.position, transform.position) > 0.2f) {
                        //guard the attack if this unit is guarding, the attack can be guarded,  this unit doesn't have its back facing the attacker, and the attack isn't basically directly on top of the unit

                        print("Guard"); //testing
                        applyHitProperties(currentAttack.onGuard);

                        StartCoroutine(HitLag(currentAttack.data.attackOwnerStatus.animator, 0.1f, 0.1f)); //guard hitlag

                        currentLight = Instantiate(guardLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        //awayForce = -awayForce * currentAttack.onGuard.onHitForwardBackward; //testing knockback, change this to onHitAwayToward when implemented
                        //motor.rb.AddForce(awayForce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onGuard.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onGuard.onHitRightLeft);

                    }
                    else if (status.isFloored()) {
                        //apply attack

                        print("FlooredHit"); //testing

                        applyHitProperties(currentAttack.onFlooredHit);

                        StartCoroutine(HitLag(currentAttack.data.attackOwnerStatus.animator, 0.4f, 0.15f)); //floored hitlag

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = currentAttack.transform.position;

                        //testforce = -testforce * currentAttack.onFlooredHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onFlooredHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onFlooredHit.onHitRightLeft);

                    }
                    else if (status.isAirborne()) {
                        //apply attack

                        print("AirborneHit"); //testing

                        applyHitProperties(currentAttack.onAirborneHit);

                        StartCoroutine(HitLag(currentAttack.data.attackOwnerStatus.animator, 0.4f, 0.3f)); //airborne hitlag

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        //testforce = -testforce * currentAttack.onAirborneHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onAirborneHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onAirborneHit.onHitRightLeft);

                    }
                    else if (status.isVulnerable()) {
                        //apply attack

                        print("VulnerableHit"); //testing

                        //consume vulnerable status
                        status.vulnerable = 0;

                        applyHitProperties(currentAttack.onVulnerableHit);

                        StartCoroutine(HitLag(currentAttack.data.attackOwnerStatus.animator, 0.2f, 0.3f)); //Vulnerable Hitlag

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);
                        currentLight.range = 1.5f;
                        currentLight.intensity = 6;

                        //testforce = -testforce * currentAttack.onVulnerableHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onVulnerableHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onVulnerableHit.onHitRightLeft);

                    }
                    else {
                        //apply attack
                            print("Hit"); //testing

                            applyHitProperties(currentAttack.onHit);

                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStatus.animator, 0.2f, 0.15f)); //normal hitlag

                            currentLight = Instantiate(hitLight).GetComponent<Light>();
                            currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                            //testforce = -testforce * currentAttack.onHit.onHitForwardBackward; //testing knockback
                            //motor.rb.AddForce(testforce);

                            motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.forward * -currentAttack.onHit.onHitForwardBackward);
                            motor.rb.AddForce(currentAttack.data.attackOwnerStatus.gameObject.transform.right * -currentAttack.onHit.onHitRightLeft);

                    }
                } //else do nothing
            }
            
        }

    }

}
