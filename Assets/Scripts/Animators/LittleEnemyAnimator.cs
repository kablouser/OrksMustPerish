using UnityEngine;

public class LittleEnemyAnimator : GenericAnimator
{
    public float maxAnimationSpeed = 4;

    public override void SetWalk(float speed)
    {
        animator.SetFloat(walkBlend, speed / maxAnimationSpeed);
    }

    public override void SetWalkDirection(Vector2 direction)
    {
        Debug.LogError("Little enemy cannot use SetWalkDirection. Use SetWalk instead.");
    }
}
