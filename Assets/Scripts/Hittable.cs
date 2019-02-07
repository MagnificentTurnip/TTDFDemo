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
            Vector3 testforce = Vector3.Normalize(transform.position - currentAttack.transform.parent.transform.position); //testing knockback
            if (currentAttack.data.attackDelay <= 0 && currentAttack.data.attackDuration > 0) {
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


                    else if (status.guarding || status.isGuardStunned()) {
                        //guard the attack

                        print("Guard"); //testing

                        if (currentAttack.onGuard.damageInstances != null) {
                            for (int i = 0; i < currentAttack.onGuard.damageInstances.Count; i++) { //loop through damage instances to apply them

                                //if (currentAttack.onHit.damageInstances[i].damageType == Attack.typeOfDamage.Slashing) {
                                stat.HP -= currentAttack.onGuard.damageInstances[i].damageAmount/* *= slashingTaken*/;
                                //}

                            }
                        }

                        testforce = -testforce * currentAttack.onGuard.onHitForwardBackward; //testing knockback
                        motor.rb.AddForce(testforce);
                        //find some way to apply onHitRightLeft

                        if (currentAttack.onGuard.causesFlinch) {
                            status.flinch();
                        }

                        if (currentAttack.onGuard.causesVulnerable + currentAttack.data.attackDuration > status.vulnerable && currentAttack.onGuard.causesVulnerable != 0) {
                            status.vulnerable = currentAttack.onGuard.causesVulnerable + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onGuard.causesSilenced + currentAttack.data.attackDuration > status.silenced && currentAttack.onGuard.causesSilenced != 0) {
                            status.silenced = currentAttack.onGuard.causesSilenced + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onGuard.causesFloored + currentAttack.data.attackDuration > status.floored && currentAttack.onGuard.causesFloored != 0) {
                            status.floored = currentAttack.onGuard.causesFloored + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onGuard.causesAirborne + currentAttack.data.attackDuration > status.airborne && currentAttack.onGuard.causesAirborne != 0) {
                            status.airborne = currentAttack.onGuard.causesAirborne + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onGuard.causesStun + currentAttack.data.attackDuration > status.stunned && currentAttack.onGuard.causesStun != 0) {
                            status.stunned = currentAttack.onGuard.causesStun + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onGuard.causesParalyze + currentAttack.data.attackDuration > status.paralyzed && currentAttack.onGuard.causesParalyze != 0) {
                            status.paralyzed = currentAttack.onGuard.causesParalyze + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onGuard.causesGrapple + currentAttack.data.attackDuration > status.grappled && currentAttack.onGuard.causesGrapple != 0) {
                            status.grappled = currentAttack.onGuard.causesGrapple + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onGuard.causesGuardStun + currentAttack.data.attackDuration > status.guardStunned && currentAttack.onGuard.causesGuardStun != 0) {
                            status.guardStunned = currentAttack.onGuard.causesGuardStun + currentAttack.data.attackDuration;
                        }




                    } else {
                        //apply attack

                        print("Hit"); //testing

                        if (currentAttack.onHit.damageInstances != null) {
                            for (int i = 0; i < currentAttack.onHit.damageInstances.Count; i++) { //loop through damage instances to apply them

                                //if (currentAttack.onHit.damageInstances[i].damageType == Attack.typeOfDamage.Slashing) {
                                stat.HP -= currentAttack.onHit.damageInstances[i].damageAmount/* *= slashingTaken*/;
                                //}

                            }
                        }

                        //motor.instantBurst(currentAttack.onHit.onHitForwardBackward, currentAttack.onHit.onHitRightLeft);
                        testforce = -testforce * currentAttack.onHit.onHitForwardBackward;
                        motor.rb.AddForce(testforce);
                        //find some way to apply onHitRightLeft

                        if (currentAttack.onHit.causesFlinch) {
                            status.flinch();
                        }

                        if (currentAttack.onHit.causesVulnerable + currentAttack.data.attackDuration > status.vulnerable && currentAttack.onHit.causesVulnerable != 0) {
                            status.vulnerable = currentAttack.onHit.causesVulnerable + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onHit.causesSilenced + currentAttack.data.attackDuration > status.silenced && currentAttack.onHit.causesSilenced != 0) {
                            status.silenced = currentAttack.onHit.causesSilenced + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onHit.causesFloored + currentAttack.data.attackDuration > status.floored && currentAttack.onHit.causesFloored != 0) {
                            status.floored = currentAttack.onHit.causesFloored + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onHit.causesAirborne + currentAttack.data.attackDuration > status.airborne && currentAttack.onHit.causesAirborne != 0) {
                            status.airborne = currentAttack.onHit.causesAirborne + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onHit.causesStun + currentAttack.data.attackDuration > status.stunned && currentAttack.onHit.causesStun != 0) {
                            status.stunned = currentAttack.onHit.causesStun + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onHit.causesParalyze + currentAttack.data.attackDuration > status.paralyzed && currentAttack.onHit.causesParalyze != 0) {
                            status.paralyzed = currentAttack.onHit.causesParalyze + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onHit.causesGrapple + currentAttack.data.attackDuration > status.grappled && currentAttack.onHit.causesParalyze != 0) {
                            status.grappled = currentAttack.onHit.causesGrapple + currentAttack.data.attackDuration;
                        }

                        if (currentAttack.onHit.causesGuardStun + currentAttack.data.attackDuration > status.guardStunned && currentAttack.onHit.causesGuardStun != 0) {
                            print("why");
                            print(currentAttack.onHit.causesGuardStun);
                            status.guardStunned = currentAttack.onHit.causesGuardStun + currentAttack.data.attackDuration;
                        }
                        


                    }
                } //else do nothing
            }
            
        }

    }

}
