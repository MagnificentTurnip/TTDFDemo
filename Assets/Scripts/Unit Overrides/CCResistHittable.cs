using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CCResistHittable : Hittable {

    public int flinchLimit; //the number the flinchCounter can reach before it causes flinch
    public int flinchCounter; //the current stored number of flinches

    public override void ApplyHitProperties(Attack.hitProperties properties) {

        if (properties.damageInstances != null) {
            for (int i = 0; i < properties.damageInstances.Count; i++) { //loop through damage instances to apply them
                currentDamageNumber = Instantiate(damageNumber);
                RectTransform cdnrt = currentDamageNumber.GetComponent<RectTransform>();
                cdnrt.position = cam.WorldToScreenPoint(transform.position);
                cdnrt.SetParent(damageNumberCanvas.transform);
                cdnrt.Translate(Random.Range(-40, 40), Random.Range(-40, 40), 0);

                switch (properties.damageInstances[i].damageType) {
                    case Attack.typeOfDamage.Slashing:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * slashingTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * slashingTaken;
                        source.PlayOneShot(slashingSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.Impact:
                        if (stat.HP - properties.damageInstances[i].damageAmount * impactTaken > 0) { //Impact damage leaves the recipient at 0HP when it would otherwise kill, ensuring a knockout
                            currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * impactTaken)).ToString();
                            stat.HP -= properties.damageInstances[i].damageAmount * impactTaken;
                        }
                        else {
                            currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * impactTaken)).ToString() + " K.O.";
                            stat.HP = 0;
                            status.unconscious = true;
                        }
                        source.PlayOneShot(impactSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.Piercing:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * piercingTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * piercingTaken;
                        source.PlayOneShot(piercingSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.Fire:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * fireTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * fireTaken;
                        source.PlayOneShot(fireSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.Cold:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * coldTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * coldTaken;
                        source.PlayOneShot(coldSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.Caustic:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * causticTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * causticTaken;
                        source.PlayOneShot(causticSound, Random.Range(0.4f, 0.6f));
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
                        }
                        else {
                            currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * astralTaken)).ToString() + " K.O.";
                            stat.HP = 0;
                            status.unconscious = true;
                        }
                        source.PlayOneShot(astralSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.Ruinous:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * ruinousTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * ruinousTaken;
                        source.PlayOneShot(ruinousSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.Magic:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.HP) - Mathf.CeilToInt(stat.HP - properties.damageInstances[i].damageAmount * magicTaken)).ToString();
                        stat.HP -= properties.damageInstances[i].damageAmount * magicTaken;
                        source.PlayOneShot(magicSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.SPdamage:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.SP) - Mathf.CeilToInt(stat.SP - properties.damageInstances[i].damageAmount * SPdamageTaken)).ToString();
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().color = new Color(170, 255, 70, 150);
                        stat.SP -= properties.damageInstances[i].damageAmount * SPdamageTaken;
                        source.PlayOneShot(SPdamageSound, Random.Range(0.4f, 0.6f));
                        break;
                    case Attack.typeOfDamage.MPdamage:
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().text = (Mathf.CeilToInt(stat.MP) - Mathf.CeilToInt(stat.MP - properties.damageInstances[i].damageAmount * MPdamageTaken)).ToString();
                        stat.MP -= properties.damageInstances[i].damageAmount * MPdamageTaken;
                        currentDamageNumber.GetComponent<TextMeshProUGUI>().color = new Color(70, 70, 255, 150);
                        source.PlayOneShot(MPdamageSound, Random.Range(0.4f, 0.6f));
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

        if (properties.causesFlinch && flinchCounter < flinchLimit) {
            flinchCounter += 1;
        }

        if (flinchCounter >= flinchLimit || status.isVulnerable()) {
            if (properties.causesFlinch) {
                status.flinch();
                transform.LookAt(currentAttack.transform.parent, Vector3.up); //face the attacker that caused flinch;
                transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //lock rotation on x and z;
                StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.2f, 0.15f));
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

            flinchCounter = 0;
        }
    }

    public void FixedUpdate() {
        
    }
}
