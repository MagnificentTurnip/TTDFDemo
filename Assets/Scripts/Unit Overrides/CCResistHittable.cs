﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CCResistHittable : Hittable {

    public int flinchLimit; //the number the flinchCounter can reach before it causes flinch
    public int flinchCounter; //the current stored number of flinches

    public override void applyHitProperties(Attack.hitProperties properties) {
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
        
        if (properties.causesFlinch && flinchCounter < flinchLimit) {
            flinchCounter += 1;
        }

        if (flinchCounter >= flinchLimit || status.isVulnerable()) {
            if (properties.causesFlinch) {
                status.flinch();
                transform.LookAt(currentAttack.transform.parent, Vector3.up); //face the attacker that caused flinch;
                transform.localEulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //lock rotation on x and z;
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
