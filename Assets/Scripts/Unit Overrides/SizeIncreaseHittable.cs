using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeIncreaseHittable : CCResistHittable {

    public AIWolfShaman ai;

    public override void OnTriggerStay(Collider col) {

        if (col.gameObject.tag.Contains("attack") && col.gameObject.transform.parent != this.gameObject.transform.parent) {

            currentAttack = col.gameObject.GetComponent<Attack>();

            //testing knockback
            Vector3 awayForce = Vector3.Normalize(transform.position - currentAttack.data.attackOwnerStyle.gameObject.transform.position);
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
                        if (currentAttack.data.hitsStanding && ai.big) {
                            actuallyHits = true;
                        }
                    }
                    else if (status.isFloored()) {
                        if (currentAttack.data.hitsFloored) {
                            actuallyHits = true;
                        }
                        if (currentAttack.data.hitsStanding && ai.big) {
                            actuallyHits = true;
                        }
                    }
                    else if (currentAttack.data.hitsStanding || (currentAttack.data.hitsAirborne && ai.big)) {
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

                        currentLight = Instantiate(parryLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        if (currentAttack.data.contact == true && status.parryStunFrames > 0) {
                            currentAttack.data.attackOwnerStyle.status.flinch();
                            if (status.parryFrames + status.parryStunFrames > currentAttack.data.attackOwnerStyle.status.parryStunned) {
                                currentAttack.data.attackOwnerStyle.status.parryStunned = status.parryFrames + status.parryStunFrames;
                            }
                        }

                        if (status.magicGuard) {
                            ApplyHitProperties(currentAttack.onGuard);
                        }
                    }


                    else if ((status.guarding || status.isGuardStunned()) && currentAttack.data.unblockable != 1 && currentAttack.data.unblockable != 3 && (towardAttackerAngle >= -90 && towardAttackerAngle <= 90 || Vector3.Distance(currentAttack.transform.position, transform.position) < 0.3f)) {
                        //guard the attack if this unit is guarding, the attack can be guarded,  this unit doesn't have its back facing the attacker, and the attack isn't basically directly on top of the unit

                        print("Guard"); //testing
                        ApplyHitProperties(currentAttack.onGuard);

                        if (currentAttack.data.attackOwnerStyle.gameObject.GetComponent<AtkStyle>()) {
                            currentAttack.data.attackOwnerStyle.gameObject.GetComponent<AtkStyle>().stat.SP -= currentAttack.onGuard.SPcost;
                        }

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.1f, 0.1f)); //guard hitlag
                        }

                        currentLight = Instantiate(guardLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        //awayForce = -awayForce * currentAttack.onGuard.onHitForwardBackward; //testing knockback, change this to onHitAwayToward when implemented
                        //motor.rb.AddForce(awayForce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.forward * -currentAttack.onGuard.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.right * -currentAttack.onGuard.onHitRightLeft);

                    }
                    else if (status.isFloored()) {
                        //apply attack

                        print("FlooredHit"); //testing

                        ApplyHitProperties(currentAttack.onFlooredHit);

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.4f, 0.15f)); //floored hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = currentAttack.transform.position;

                        //testforce = -testforce * currentAttack.onFlooredHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.forward * -currentAttack.onFlooredHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.right * -currentAttack.onFlooredHit.onHitRightLeft);

                    }
                    else if (status.isAirborne()) {
                        //apply attack

                        print("AirborneHit"); //testing

                        ApplyHitProperties(currentAttack.onAirborneHit);

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.4f, 0.3f)); //airborne hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        //testforce = -testforce * currentAttack.onAirborneHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.forward * -currentAttack.onAirborneHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.right * -currentAttack.onAirborneHit.onHitRightLeft);

                    }
                    else if (status.isVulnerable()) {
                        //apply attack

                        print("VulnerableHit"); //testing

                        //consume vulnerable status
                        status.vulnerable = 0;

                        ApplyHitProperties(currentAttack.onVulnerableHit);

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.2f, 0.3f)); //Vulnerable Hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);
                        currentLight.range = 1.5f;
                        currentLight.intensity = 6;

                        //testforce = -testforce * currentAttack.onVulnerableHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.forward * -currentAttack.onVulnerableHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.right * -currentAttack.onVulnerableHit.onHitRightLeft);

                    }
                    else {
                        //apply attack
                        print("Hit"); //testing

                        ApplyHitProperties(currentAttack.onHit);

                        if (currentAttack.data.attackOwnerStyle.hitlag) {
                            StartCoroutine(HitLag(currentAttack.data.attackOwnerStyle.animator, 0.2f, 0.15f)); //normal hitlag
                        }

                        currentLight = Instantiate(hitLight).GetComponent<Light>();
                        currentLight.transform.position = col.ClosestPointOnBounds(transform.position);

                        //testforce = -testforce * currentAttack.onHit.onHitForwardBackward; //testing knockback
                        //motor.rb.AddForce(testforce);

                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.forward * -currentAttack.onHit.onHitForwardBackward);
                        motor.rb.AddForce(currentAttack.data.attackOwnerStyle.gameObject.transform.right * -currentAttack.onHit.onHitRightLeft);

                    }
                } //else do nothing
            }

        }

    }
}
