using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://forum.unity.com/threads/access-rendering-mode-var-on-standard-shader-via-scripting.287002/

public class StandardDeath : DeathManager {

    public Animator animator;
    public List<MeshRenderer> meshRends; //a list of all relevant meshrenderers to be faded out
    public List<SkinnedMeshRenderer> meshRendsSkinned; //basically same but in case of skinned ones;
    public float maxDeleteFrames; //the original number of frames until the unit is deleted
    public float savedDeleteFrames; //the true delete frames, but as a float

    public override void StartDie() {
        animator.Play("death", -1, 0f); //play the death animation;
    }

    public override void ContinueDie() {
        stat.status.invulnerable = 60; //refreshing invulnerability, incorporeality, non-regen, and deadness
        stat.status.Incorporealise(60);
        stat.status.HPRegenEnabled = false;
        stat.status.SPRegenEnabled = false;
        stat.status.MPRegenEnabled = false;
        stat.status.slain = true;
        if (meshRends != null) { //fade them out if possible
            for (int i = 0; i < meshRends.Count; i++) {
                meshRends[i].material.SetFloat("_Mode", 2);
                meshRends[i].material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                meshRends[i].material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                meshRends[i].material.SetInt("_ZWrite", 0);
                meshRends[i].material.DisableKeyword("_ALPHATEST_ON");
                meshRends[i].material.EnableKeyword("_ALPHABLEND_ON");
                meshRends[i].material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                meshRends[i].material.renderQueue = 3000;
                meshRends[i].material.color = new Color(meshRends[i].material.color.r, meshRends[i].material.color.g, meshRends[i].material.color.b, meshRends[i].material.color.a * (savedDeleteFrames / maxDeleteFrames));
            }
        }
        if (meshRendsSkinned != null) { //fade them out if possible (skinned version
            for (int i = 0; i < meshRendsSkinned.Count; i++) {
                meshRendsSkinned[i].material.SetFloat("_Mode", 2);
                meshRendsSkinned[i].material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                meshRendsSkinned[i].material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                meshRendsSkinned[i].material.SetInt("_ZWrite", 0);
                meshRendsSkinned[i].material.DisableKeyword("_ALPHATEST_ON");
                meshRendsSkinned[i].material.EnableKeyword("_ALPHABLEND_ON");
                meshRendsSkinned[i].material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                meshRendsSkinned[i].material.renderQueue = 3000;
                meshRendsSkinned[i].material.color = new Color(meshRendsSkinned[i].material.color.r, meshRendsSkinned[i].material.color.g, meshRendsSkinned[i].material.color.b, meshRendsSkinned[i].material.color.a * (savedDeleteFrames / maxDeleteFrames));
            }
        }
    }

    public override void KO() {
        stat.status.unconscious = true;
    }

    // Use this for initialization
    public override void Start () {
        base.Start();
        framesUntilDeleted = Mathf.RoundToInt(maxDeleteFrames);
	}
	
	// Update is called once per frame
	void Update () {

	}

    // FixedUpdate is called once per logical step
    public override void FixedUpdate() {
        base.FixedUpdate();
        savedDeleteFrames = framesUntilDeleted;
    }


}
