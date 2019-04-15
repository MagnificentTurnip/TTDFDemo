using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : DeathManager {

    public GameObject failMenu;
    public GameObject deathText;
    public GameObject KOText;

    public override void StartDie() {
        failMenu.SetActive(true);
        deathText.SetActive(true);
    }

    public override void ContinueDie() {
        framesUntilDeleted = 5000; //ages until deletion refreshed on every frame, because the player still needs to do stuff so don't delete them.
        stat.status.invulnerable = 60; //refreshing invulnerability, incorporeality, non-regen, and deadness
        stat.status.Incorporealise(60);
        stat.status.HPRegenEnabled = false;
        stat.status.SPRegenEnabled = false;
        stat.status.MPRegenEnabled = false;
        stat.status.slain = true;
    }

    public override void KO() {
        stat.status.unconscious = true;
        failMenu.SetActive(true);
        KOText.SetActive(true);
    }

    // Use this for initialization
    public override void Start() {
        base.Start();
    }

    // Update is called once per frame
    void Update() {

    }

    // FixedUpdate is called once per logical step
    public override void FixedUpdate() {
        base.FixedUpdate();
    }
}
