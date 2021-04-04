using UnityEngine;
using System.Collections;

public class BigEnemyAnimator : GenericAnimator
{
    public MeshRenderer tracksRenderer;
    public float scrollSpeed = 4.0f;
    public ParticleSystem exhaustFX;
    public float fireStartTime;
    public ParticleSystem fireFX;
        
    private Material tracksMaterial;
    private float currentScroll;
    private float speed;

    private void Start()
    {
        enabled = false;
        // not shared material, so each instance gets 1 copy
        tracksMaterial = tracksRenderer.material;
        currentScroll = speed = 0;
    }

    private void FixedUpdate()
    {
        currentScroll += Time.fixedDeltaTime * scrollSpeed * speed;
        tracksMaterial.SetTextureOffset(mainTexture, new Vector2(0, currentScroll));
    }

    public override void SetWalk(float speed)
    {
        enabled = 0 < speed;
        this.speed = speed;
        if (enabled)
        {
            if (exhaustFX.isStopped)
                exhaustFX.Play();
        }
        else
        {
            if (exhaustFX.isPlaying)
                exhaustFX.Stop();
        }
    }
    
    public override void TriggerAttack()
    {
        base.TriggerAttack();
        if (fireFX.isPlaying)
            fireFX.Stop();

        StopAllCoroutines();
        StartCoroutine(PlayFireFXLater());
    }

    private IEnumerator PlayFireFXLater()
    {
        yield return new WaitForSeconds(fireStartTime);
        fireFX.Play();
    }

    public override void SetWalkDirection(Vector2 direction)
    {
        Debug.LogError("Big enemy cannot use SetWalkDirection. Use SetWalk instead.");
    }
}
