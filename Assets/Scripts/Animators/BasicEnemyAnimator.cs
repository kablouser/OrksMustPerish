using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAnimator : GenericAnimator
{
    public float maxAnimationSpeed = 4;

    public override void SetWalk(float speed)
    {
        animator.SetFloat(walkBlend, speed / maxAnimationSpeed);
    }

    public override void SetWalkDirection(Vector2 direction)
    {
        Debug.LogError("Basic enemy cannot use SetWalkDirection. Use SetWalk instead.");
    }
}
