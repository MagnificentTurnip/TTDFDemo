﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public enum typeOfDamage { True, Slashing, Impact, Piercing, Fire, Cold, Caustic, Shock, Astral, Ruinous, Magic, SPdamage, MPdamage };
    //true damage is another word for HPdamage, just flat out. SPdamage and MPdamage could I guess also be considered true damage but whatever.

    public struct Damage {
        public float damageAmount;
        public typeOfDamage damageType;
    }

    public struct AtkData {
        //the status manager of whoever instantiated this attack
        public AtkStyle attackOwnerStyle;

        //the animator for the hitbox
        public Animator HitboxAnimator;

        //the attack's hitbox. If you want more than one hitbox... just make a new attack!
        public Collider atkHitBox;

        //ATTACK PROPERTIES
        public string GFXAnimation; //the visual animation that will play with the attack
        public string HitboxAnimation; //the animation that will play to move the attack's hitbox throughout the attack

        public float xScale; //Attacks come in all different shapes and sizes, these variables are used to change the shape of the used hitbox
        public float yScale;
        public float zScale;

        public float HPcost; //Some attacks cost a certain amount of HP, SP or MP to perform.
        public float SPcost;
        public float MPcost;

        public int attackDelay; //the number of frames before the attack actually begins
        public int attackCharge; //the number of frames, if any, that the attacker can spend charging up the attack by holding down the attacking button(s)
        public int attackDuration; //the number of frames during which the attack will take place
        public int attackEnd; //the number of frames after the attack before the user returns to idle

        //some attacks hit in different areas of verticality
        public bool hitsStanding; //true if can hit targets just minding their own business, neither floored nor airborne
        public bool hitsFloored; //true if can hit targets with the floored condition
        public bool hitsAirborne; //true if can hit targets with the airborne condition

        public bool contact; //does the attack actually make contact between attacker and target?
        public int unblockable; //certain attacks are some kind of unblockable. 0 is blockable, 1 cannot be guarded, 2 cannot be parried, and 3 cannot be guarded or parried.

        /*
        idk what the heck this is, if you have nothing else to do try to do something ask Liam it's an optimisation doesn't big matter
        int exampleAnimHash;
        string exampleAnimString { set { exampleAnimHash = Animator.StringToHash(value); } }
        */

        public AtkData( //constructor
            AtkStyle _attackOwnerStyle = null,
            Animator _HitboxAnimator = null,
            Collider _atkHitBox = null,
            string _GFXAnimation = "defaultAnimation", 
            string _HitboxAnimation = "defaultAnimation", 
            float _xScale = 1f, 
            float _yScale = 1f, 
            float _zScale = 1f,
            float _HPcost = 0,
            float _SPcost = 0,
            float _MPcost = 0,
            int _attackDelay = 0,
            int _attackCharge = 0,
            int _attackDuration = 0,
            int _attackEnd = 0,
            bool _hitsStanding = false,
            bool _hitsFloored = false, 
            bool _hitsAirborne = false,
            bool _contact = true,
            int _unblockable = 0) {
            attackOwnerStyle = _attackOwnerStyle;
            HitboxAnimator = _HitboxAnimator;
            atkHitBox = _atkHitBox;
            GFXAnimation = _GFXAnimation;
            HitboxAnimation = _HitboxAnimation;
            xScale = _xScale;
            yScale = _yScale;
            zScale = _zScale;
            HPcost = _HPcost;
            SPcost = _SPcost;
            MPcost = _MPcost;
            attackDelay = _attackDelay;
            attackCharge = _attackCharge;
            attackDuration = _attackDuration;
            attackEnd = _attackEnd;
            hitsStanding = _hitsStanding;
            hitsFloored = _hitsFloored;
            hitsAirborne = _hitsAirborne;
            contact = _contact;
            unblockable = _unblockable;
        }
    }

    public struct HitProperties {

        public List<Damage> damageInstances;

        public float HPcost; //Some attacks, although unusual, cost a certain amount of HP, SP or MP on guard, hit, etc.
        public float SPcost;
        public float MPcost;

        public bool causesFlinch; //Flinch is an instantaneous effect, so it can never be active for more than one frame from an attack. So, it's a bool.
        public int causesVulnerable; //the other ones are conditions, active for a number of frames equal to these variables plus whatever number remains of attackDuration unless it is zero.
        public int causesSilenced;
        public int causesFloored;
        public int causesAirborne;
        public int causesStun;
        public int causesParalyze;
        public int causesGrapple;
        public int causesGuardStun; //should only be applied on guard, as you might guess from the name

        //Forced movement - some attacks will forcibly move someone hit by it, calling the instantBurst method on the attacked unit's motor.
        public float onHitAwayTowards; //positive for away from the attack, negative for toward the attack
        public float onHitForwardBackward; //positive for forward, negative for backward (referring to the attacker's transform)
        public float onHitRightLeft; //positive for right, negative for left (referring to the attacker's transform)

        public HitProperties( //constructor
            List<Damage> _damageInstances = null,
            float _HPcost = 0,
            float _SPcost = 0,
            float _MPcost = 0,
            bool _causesFlinch = false,
            int _causesVulnerable = 0,
            int _causesSilenced = 0,
            int _causesFloored = 0,
            int _causesAirborne = 0,
            int _causesStun = 0,
            int _causesParalyze = 0,
            int _causesGrapple = 0,
            int _causesGuardStun = 0,
            float _onHitAwayTowards = 0,
            float _onHitForwardBackward = 0,
            float _onHitRightLeft = 0) {
            damageInstances = _damageInstances;
            HPcost = _HPcost;
            SPcost = _SPcost;
            MPcost = _MPcost;
            causesFlinch = _causesFlinch;
            causesVulnerable = _causesVulnerable;
            causesSilenced = _causesSilenced;
            causesFloored = _causesFloored;
            causesAirborne = _causesAirborne;
            causesStun = _causesStun;
            causesParalyze = _causesParalyze;
            causesGrapple = _causesGrapple;
            causesGuardStun = _causesGuardStun;
            onHitAwayTowards = _onHitAwayTowards;
            onHitForwardBackward = _onHitForwardBackward;
            onHitRightLeft = _onHitRightLeft;
        }
    }

    public List<GameObject> thingsHit;
    public int same;
    public TrailRenderer trail;

    //audio things
    public AudioSource source;
    public AudioClip clip;
    public bool clipPlayed;

    public AtkData data;
    public HitProperties onHit;
    public HitProperties onChargeHit;
    public HitProperties onGuard;
    public HitProperties onChargeGuard;
    public HitProperties onVulnerableHit;
    public HitProperties onVulnerableChargeHit;
    public HitProperties onFlooredHit;
    public HitProperties onFlooredChargeHit;
    public HitProperties onAirborneHit;
    public HitProperties onAirborneChargeHit;

    // Use this for initialization
    void Start () {
        trail = GetComponent<TrailRenderer>();
        source = GetComponent<AudioSource>();
        clipPlayed = false;
    }

    // Update is called once per frame
    void Update () {

        if (!clipPlayed && source != null && clip != null) {
            source.PlayOneShot(clip, Random.Range(0.1f, 0.2f));
            clipPlayed = true;
        }

        //set the colour of the trail
        if (data.unblockable == 3) { //unblockable 3 attacks are red
            //maybe also have different albedo maps set for each thing
            //trail.material.SetTexture("_MainTex", (public Texture u3Tex));
            if (data.attackDelay > 0) {
                trail.material.SetColor("_Color", new Color(1f, 0.3f, 0.3f, 0.2f));
            }

            else if (data.attackDuration > 0) {
                trail.material.SetColor("_Color", new Color(1f, 0.3f, 0.3f, 0.8f));
            }

            else if (data.attackEnd > 0) {
                trail.material.SetColor("_Color", new Color(1f, 0.3f, 0.3f, 0.2f));
            }
        }


        else if (data.unblockable == 2) { //unblockable 2 attacks are blue
            if (data.attackDelay > 0) {
                trail.material.SetColor("_Color", new Color(0.3f, 0.3f, 1f, 0.1f));
            }

            else if (data.attackDuration > 0) {
                trail.material.SetColor("_Color", new Color(0.3f, 0.3f, 1f, 0.7f));
            }

            else if (data.attackEnd > 0) {
                trail.material.SetColor("_Color", new Color(0.3f, 0.3f, 1f, 0.1f));
            }
        }

        else if (data.unblockable == 1) { //unblockable 1 attacks are green
            if (data.attackDelay > 0) {
                trail.material.SetColor("_Color", new Color(0.3f, 1f, 0.3f, 0.1f));
            }

            else if (data.attackDuration > 0) {
                trail.material.SetColor("_Color", new Color(0.3f, 1f, 0.3f, 0.7f));
            }

            else if (data.attackEnd > 0) {
                trail.material.SetColor("_Color", new Color(0.3f, 1f, 0.3f, 0.1f));
            }
        }

        else { //regular attacks are grey/white
            if (data.attackDelay > 0) {
                trail.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f, 0.1f));
            }

            else if (data.attackDuration > 0) {
                trail.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f, 0.7f));
            }

            else if (data.attackEnd > 0) {
                trail.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f, 0.1f));
            }
        }

        
	}
}
