using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    public Animator animator;
    public Motor motor;
    public StatusManager status;
    public PlayerMovement movement;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        animator.SetFloat("speedPercent", motor.rb.velocity.magnitude / (movement.sprintSpeed / 150), .1f, Time.deltaTime);
        animator.SetBool("sneak", status.sneaking);


    }
}
