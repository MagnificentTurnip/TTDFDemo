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

    //damage numbers that appear on screen to show the amount of damage taken
    public GameObject damageNumber;
    public GameObject currentDamageNumber;
    public Camera cam;
    public Canvas damageNumberCanvas;

    //lights that appear on hit, guard and parry for clarity
    public Light currentLight;
    public GameObject hitLight;
    public GameObject guardLight;
    public GameObject parryLight;

    public bool actuallyHits;

    public Attack currentAttack;

    public Motor motor;
    public StatusManager status;
    public StatSheet stat;

    public float towardAttackerAngle; //variable to store the angle toward an attacker that strikes this entity
    public Vector3 awayDirection; //variable to store a normalised vector of the direction away from an attack that strikes this entity

    //audio things
    public AudioSource source;
    public AudioClip onHitSound;
    public AudioClip onVulnerableHitSound;
    public AudioClip onGuardSound;
    public AudioClip onParrySound;

    public AudioClip slashingSound;
    public AudioClip impactSound;
    public AudioClip piercingSound;
    public AudioClip fireSound;
    public AudioClip coldSound;
    public AudioClip causticSound;
    public AudioClip shockSound;
    public AudioClip astralSound;
    public AudioClip ruinousSound;
    public AudioClip magicSound;
    public AudioClip SPdamageSound;
    public AudioClip MPdamageSound;


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public IEnumerator HitLag(Animator animator, float newSpeed, float lagTime) {
        if (animator != null) {
            animator.speed = newSpeed;
            yield return new WaitForSeconds(lagTime);
            animator.speed = 2f - newSpeed;
            yield return new WaitForSeconds(lagTime);
            animator.speed = 1f;
        }
    }


    public virtual void ApplyHitProperties(Attack.HitProperties properties) {
        
        if (properties.damageInstances != null) {
            for (int i = 0; i < properties.damageInstances.Count; i++) { //loop through damage instances to apply them
                currentDamageNumber = Instantiate(damageNumber);
                currentDamageNumber.transform.position = cam.WorldToScreenPoint(transform.position);
                currentDamageNumber.transform.SetParent(damageNumberCanvas.transform);
                currentDamageNumber.transform.Translate(Random.Range(-40, 40), Random.Range(-40, 40), 0);
                
                switch (properties.damageInstances[i].damageType) {
                    case Attack.typeOfDamage.Slashing:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * slashingTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * slashingTaken;
                        source.PlayOneShot(slashingSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Impact:
                        if (stat.HP - properties.damageInstances[i].damageAmount * impactTaken > 0) { //Impact damage leaves the recipient at 0HP when it would otherwise kill, ensuring a knockout
                            currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * impactTaken)).ToString();
                            stat.HP -= properties.damageInstances[i].damageAmount * impactTaken;
                        } else {
                            currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * impactTaken)).ToString() + " K.O.";
                            stat.HP = 0;
                            status.unconscious = true;
                        }
                        source.PlayOneShot(impactSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Piercing:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * piercingTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * piercingTaken;
                        source.PlayOneShot(piercingSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Fire:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * fireTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * fireTaken;
                        source.PlayOneShot(fireSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Cold:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * coldTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * coldTaken;
                        source.PlayOneShot(coldSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Caustic:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * causticTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * causticTaken;
                        source.PlayOneShot(causticSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Shock:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * shockTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * shockTaken;
                        source.PlayOneShot(shockSound, Random.Range(0.8f, 1f));
                        break;
                    case Attack.typeOfDamage.Astral:
                        if (stat.HP - properties.damageInstances[i].damageAmount * astralTaken > 0) { //Astral damage leaves the recipient at 0HP when it would otherwise kill, ensuring a knockout
                            currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * astralTaken)).ToString();
                            stat.HP -= properties.damageInstances[i].damageAmount * astralTaken;
                        } else {
                            currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * astralTaken)).ToString() + " K.O.";
                            stat.HP = 0;
                            status.unconscious = true;
                        }
                        source.PlayOneShot(astralSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Ruinous:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * ruinousTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * ruinousTaken;
                        source.PlayOneShot(ruinousSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.Magic:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * magicTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * magicTaken;
                        source.PlayOneShot(magicSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.SPdamage:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.SP) - Mathf.CeilToInt(stat.SP - properties.damageInstances[i].damageAmount * SPdamageTaken)).ToString();
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().color = new Color(170, 255, 70, 150);
                        stat.SP -= properties.damageInstances[i].damageAmount * SPdamageTaken;
                        source.PlayOneShot(SPdamageSound, Random.Range(0.3f, 0.4f));
                        break;
                    case Attack.typeOfDamage.MPdamage:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.MP) - Mathf.CeilToInt(stat.MP - properties.damageInstances[i].damageAmount * MPdamageTaken)).ToString();
                        stat.MP -= properties.damageInstances[i].damageAmount * MPdamageTaken;
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().color = new Color(70, 70, 255, 150);
                        source.PlayOneShot(MPdamageSound, Random.Range(0.3f, 0.4f));
                        break;
                }

            }
        }

        if (status.unconscious && stat.HP > 0) { //if appropriate, wake up
            status.unconscious = false;
        }

        if (currentAttack.data.attackOwnerStyle.charStat != null) {
            currentAttack.data.attackOwnerStyle.charStat.HP -= properties.HPcost; //apply costs
            currentAttack.data.attackOwnerStyle.charStat.MP -= properties.MPcost;
            currentAttack.data.attackOwnerStyle.charStat.SP -= properties.SPcost;
        }
        else {
            currentAttack.data.attackOwnerStyle.stat.HP -= properties.HPcost; //apply costs
            currentAttack.data.attackOwnerStyle.stat.MP -= properties.MPcost;
            currentAttack.data.attackOwnerStyle.stat.SP -= properties.SPcost;
        }

        if (properties.causesFlinch) {
            status.Flinch();
            transform.LookAt(currentAttack.transform.parent, Vector3.up); //face the attack that caused flinch;
            transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //lock rotation on x and z;
        }

        //floored with absence of airborne cleanses airborne
        if (properties.causesFloored > 0 && properties.causesAirborne <= 0) {
            status.airborne = 0;
        }

        //regular status application
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


        //knockback
        awayDirection = Vector3.Normalize(transform.position - currentAttack.data.attackOwnerStyle.gameObject.transform.position);
        awayDirection.y = 0f;
        awayDirection = awayDirection * -properties.onHitAwayTowards;
        motor.rb.AddForce(awayDirection * -properties.onHitAwayTowards);
        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.forward * -properties.onHitForwardBackward);
        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.right * -properties.onHitRightLeft);
    }

    public virtual void OnTriggerStay(Collider col) {

        if (col.gameObject.tag.Contains("attack") && col.gameObject.transform.parent != this.gameObject.transform.parent) {

            currentAttack = col.gameObject.GetComponent<Attack>();
            
            if (currentAttack.data.attackDelay <= 0 && currentAttack.data.attackDuration > 0) {
                currentAttack.same = 0;
                for (int i = 0; i < currentAttack.thingsHit.Count; i++) {
                    if (this.gameObject == currentAttack.thingsHit[i]) {
                        currentAttack.same += 1;
                    }
                }

                actuallyHits = false;
                if (status.invulnerable <= 0) {
                    if (status.IsAirborne()) {
                        if (currentAttack.data.hitsAirborne) {
                            actuallyHits = true;
                        }
                    } else if (status.IsFloored()) {
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
                    towardAttackerAngle = Mathf.DeltaAngle(transform.localEulerAngles.y, Mathf.Atan2(currentAttack.data.attackOwnerStyle.transform.position.x - transform.position.x, currentAttack.data.attackOwnerStyle.transform.position.z - transform.position.z) * Mathf.Rad2Deg);

                    if (status.parryFrames > 0 && currentAttack.data.unblockable != 2 && currentAttack.data.unblockable != 3 && (towardAttackerAngle >= -90 && towardAttackerAngle <= 90 || Vector3.Distance(currentAttack.transform.position, transform.position) < 0.3f)) {
                        //parry the attack if this unit is parrying, the attack can be parried, this unit doesn't have its back facing the attacker, and the attack isn't basically directly on top of the unit

                        status.parryLock = 0; //undo the parryLock
                        //and the attack has no effect

                        //unless it's magically guarded
                        if (status.magicGuard) {
                            source.PlayOneShot(onGuardSound);
                            ApplyHitProperties(currentAttack.onGuard);
                            if (currentAttack.data.attackOwnerStyle.hitlag) {
                                StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.1f, 0.1f)); //guard hitlag
                            }
                        } else {
                            source.PlayOneShot(onParrySound, Random.Range(0.4f, 0.6f));
                        }
                        
                        currentLight = Instantiate(parryLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        //cause relevant parryStun to the target
                        if (currentAttack.data.contact == true && status.parryStunFrames > 0) {
                            currentAttack.data.attackOwnerStyle.status.Flinch();
                            if (status.parryFrames + status.parryStunFrames > currentAttack.data.attackOwnerStyle.status.parryStunned) {
                                currentAttack.data.attackOwnerStyle.status.parryStunned = status.parryFrames + status.parryStunFrames;
                            }
                        }
                        
                    }


                    else if ((status.guarding || status.IsGuardStunned()) && currentAttack.data.unblockable != 1 && currentAttack.data.unblockable != 3 && (towardAttackerAngle >= -90 && towardAttackerAngle <= 90 || Vector3.Distance(currentAttack.transform.position, transform.position) < 0.3f)) {
                        //guard the attack if this unit is guarding, the attack can be guarded,  this unit doesn't have its back facing the attacker, and the attack isn't basically directly on top of the unit

                        print("Guard"); //testing
                        ApplyHitProperties(currentAttack.onGuard);

                        source.PlayOneShot(onGuardSound, Random.Range(0.3f, 0.5f));

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.1f, 0.1f)); //guard hitlag
                        }

                        currentLight = Instantiate(guardLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);
                    }
                    else if (status.IsFloored()) {
                        //apply attack

                        print("FlooredHit"); //testing

                        ApplyHitProperties(currentAttack.onFlooredHit);

                        source.PlayOneShot(onHitSound, Random.Range(0.1f, 0.2f));

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.4f, 0.15f)); //floored hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = currentAttack.transform.position;
                    }
                    else if (status.IsAirborne()) {
                        //apply attack

                        print("AirborneHit"); //testing

                        ApplyHitProperties(currentAttack.onAirborneHit);

                        source.PlayOneShot(onHitSound, Random.Range(0.1f, 0.2f));

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.4f, 0.3f)); //airborne hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);
                    }
                    else if (status.IsVulnerable()) {
                        //apply attack

                        print("VulnerableHit"); //testing

                        //consume vulnerable status
                        status.vulnerable = 0;

                        source.PlayOneShot(onVulnerableHitSound, Random.Range(0.1f, 0.2f));

                        ApplyHitProperties(currentAttack.onVulnerableHit);

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.2f, 0.3f)); //Vulnerable Hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);
                        currentLight.range = 1.5f;
                        currentLight.intensity = 6;
                    }
                    else {
                        //apply attack
                        print("Hit"); //testing

                        source.PlayOneShot(onHitSound, Random.Range(0.1f, 0.2f));

                        ApplyHitProperties(currentAttack.onHit);

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.2f, 0.15f)); //normal hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);
                    }
                } //else do nothing
            }
            
        }

    }

}
