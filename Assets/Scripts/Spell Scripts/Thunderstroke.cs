using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderstroke : Spell {
    
    public Camera cam;
    Ray playInRay;
    RaycastHit playInHit;

    Vector3 savedPosition;

    public GameObject shadow;
    public GameObject explosion;
    public GameObject thunderBolt;

    public bool init;

    // Use this for initialization
    public override void Start () {
        base.Start();
        cam = Camera.main;
        spellName = "Thunderstroke";
        spellCode = "3113";
        castTime = 20;
        postCastTime = 0;
        channelTime = 140;
        duration = 120;
        cost = 100; //COST MUST BE PRE-SET IN THE PREFAB TO ALLOW MISFIRING
    }

    public void ThunderstrokeVFX() {
        explosion.SetActive(true);
        thunderBolt.SetActive(true);
    }

    public void ThunderstrokeAttacks() {
        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = transform.position;
        currentAttack.transform.rotation = transform.rotation;
        currentAttack.transform.parent = transform;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<CapsuleCollider>(), //this attack uses a capsule collider
            _GFXAnimation: "thunderstroke",
            _HitboxAnimation: "defaultCapsule",
            _xScale: 1.5f,
            _yScale: 20f,
            _zScale: 1.5f,
            _attackDelay: 140, //attack takes a while to strike
            _attackDuration: 8, //8 frames within which the attack is active
            _attackEnd: 40, //when the attack ends it sticks around for a bit to continue playing sound.
            _hitsAirborne: true, //hits all targets.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: false, //doesn't make contact.
            _unblockable: 3); //can't be blocked or parried.

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = capsule; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 50f + charStat.WIL;
        }
        else {
            tempDamage.damageAmount = 50f + 2.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Shock;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 120,
            _causesFloored: 240,
            _onHitForwardBackward: 0f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = currentAttack.onHit; //this move is unblockable 3 so no guard stuff.

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //this move doesn't have any additional effect against a vulnerable target

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't change against floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //this move doesn't change against airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations - or don't
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        //animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked


        //THERE IS A BURST OF FLAME THAT ACCOMPANIES THE LIGHTNING BOLT IN A SLIGHTLY WIDER RADIUS

        //create the new attack as a child of this object
        currentAttack = Instantiate(attack).GetComponent<Attack>();
        currentAttack.transform.position = transform.position;
        currentAttack.transform.rotation = transform.rotation;
        currentAttack.transform.parent = transform;

        //set the attack data
        currentAttack.data = new Attack.AtkData(
            _attackOwnerStyle: this, //here's the style
            _HitboxAnimator: currentAttack.gameObject.GetComponent<Animator>(), //get the attack's animator
            _atkHitBox: currentAttack.gameObject.AddComponent<SphereCollider>(), //this attack uses a sphere collider
            _GFXAnimation: "thunderstroke", //same animation; no need for a second one
            _HitboxAnimation: "defaultSphere", //not sure if this is needed rn
            _xScale: 8f,
            _yScale: 2f,
            _zScale: 8f,
            _attackDelay: 140, //attack takes a while to strike
            _attackDuration: 15, //15 frames within which the attack is active
            _attackEnd: 0, //when the attack ends it's done.
            _hitsAirborne: false, //doesn't hit airborne targets but hits standing and floored.
            _hitsStanding: true,
            _hitsFloored: true,
            _contact: false, //doesn't make contact.
            _unblockable: 1); //can only be parried or magically guarded.

        currentAttack.transform.localScale = new Vector3(currentAttack.data.xScale, currentAttack.data.yScale, currentAttack.data.zScale);

        if (debug == true) {
            currentAttack.gameObject.GetComponent<MeshFilter>().mesh = sphere; //for testing the hitbox
        }

        currentAttack.data.HitboxAnimator.runtimeAnimatorController = hitboxAnimatorController; //set the attack hitbox animator's animator controller to be the one for this attack style (spell)
        currentAttack.data.atkHitBox.isTrigger = true; //make the hitbox a trigger so that it doesn't have physics

        //set the attack's properties on hit (all unset properties are defaults)
        if (charStat != null) {
            tempDamage.damageAmount = 10f + (0.2f * charStat.WIL);
        }
        else {
            tempDamage.damageAmount = 10f + 0.5f * stat.Level;
        }
        tempDamage.damageType = Attack.typeOfDamage.Fire;
        currentAttack.onHit = new Attack.HitProperties(
            _damageInstances: new List<Attack.Damage>(1) { tempDamage },
            _causesFlinch: true,
            _causesStun: 3,
            _onHitForwardBackward: 0f,
            _onHitRightLeft: 0f);

        //set the attack's properties on charge hit
        currentAttack.onChargeHit = currentAttack.onHit; //this move doesn't charge so they're the same as on hit properties

        //set the attack's properties on guard
        currentAttack.onGuard = new Attack.HitProperties(
            _causesGuardStun: 10,
            _onHitForwardBackward: -300f);

        //set the attack's properties on charge guard
        currentAttack.onChargeGuard = currentAttack.onGuard; //this move doesn't charge so they're the same as on guard properties

        //set the attack's properties on vulnerable hit
        currentAttack.onVulnerableHit = currentAttack.onHit; //this move doesn't have any additional effect against a vulnerable target

        //set the attack's properties on vulnerable charge hit
        currentAttack.onVulnerableChargeHit = currentAttack.onVulnerableHit; //this move doesn't charge

        //set the attack's properties on floored hit
        currentAttack.onFlooredHit = currentAttack.onHit; //this move doesn't change against floored targets

        //set the attack's properties on charged floored hit
        currentAttack.onFlooredChargeHit = currentAttack.onFlooredHit; //this move doesn't charge

        //set the attack's properties on airborne hit
        currentAttack.onAirborneHit = currentAttack.onHit; //this move doesn't change against airborne targets

        //set the attack's properties on charged airborne hit
        currentAttack.onAirborneChargeHit = currentAttack.onAirborneHit; //this move doesn't charge

        //play the animations - or don't
        currentAttack.data.HitboxAnimator.Play(currentAttack.data.HitboxAnimation);
        //animator.Play(currentAttack.data.GFXAnimation);

        instantiatedAttacks.Add(currentAttack); //add the current attack to the list of instantiated attacks so that it can be tracked
    }

    public override void CastSpell() {
        base.CastSpell();

        ThunderstrokeAttacks();
    }

    // Update is called once per rendered frame
    public override void Update () {

    }

    // FixedUpdate is called once per physics frame
    public override void FixedUpdate() {
        if (!init) {
            //place the thundercloud over the current target
            if (mouseTarget) {
                playInRay = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(playInRay, out playInHit, 100, 1 << LayerMask.NameToLayer("Terrain"))) {
                    playInHit.point = new Vector3(playInHit.point.x, 0, playInHit.point.z);
                    transform.position = playInHit.point;
                }
            }
            else {
                transform.position = new Vector3(target.transform.position.x, 0.21f, target.transform.position.z);
            }
            init = true;
        }

        if (channelTime <= 1) {
            ThunderstrokeVFX();
            gameObject.GetComponent<Light>().range = 0f;
        }

        shadow.transform.localScale = new Vector3(shadow.transform.localScale.x * 0.99f, shadow.transform.localScale.y, shadow.transform.localScale.z * 0.99f);

        base.FixedUpdate();
        if (ready == 2) { 
            if (channelTime > 10 && status.channelLock > 0) {

                if (mouseTarget) {

                    playInRay = cam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(playInRay, out playInHit, 100, 1 << LayerMask.NameToLayer("Terrain"))) {
                        playInHit.point = new Vector3(playInHit.point.x, 0, playInHit.point.z);
                        transform.Translate(Vector3.Normalize(playInHit.point - transform.position) * 0.15f, Space.World);
                    }

                }
                else {
                    transform.Translate(Vector3.Normalize(target.transform.position - transform.position) * 0.15f, Space.World);
                    transform.position = new Vector3(transform.position.x, 0.21f, transform.position.z);
                }
            } else {

                if (mouseTarget) {

                    playInRay = cam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(playInRay, out playInHit, 100, 1 << LayerMask.NameToLayer("Terrain"))) {
                        playInHit.point = new Vector3(playInHit.point.x, 0, playInHit.point.z);
                        transform.Translate(Vector3.Normalize(playInHit.point - transform.position) * 0.005f, Space.World);
                    }
                    
                }

                else {
                    transform.Translate(Vector3.Normalize(target.transform.position - transform.position) * 0.005f, Space.World);
                    transform.position = new Vector3(transform.position.x, 0.21f, transform.position.z);
                }
            }
            
        }

        //potential helping of attacks being deleted
        if (instantiatedAttacks.Count > 0) {
            if (instantiatedAttacks[0] == null) {
                instantiatedAttacks.Clear();
                ThunderstrokeAttacks();
                for (int i = 0; i < instantiatedAttacks.Count; i++) {
                    if (channelTime > 0) {
                        instantiatedAttacks[i].data.attackDelay -= (140 - channelTime);
                    }
                }
            }
        }
    }
    
}
