using UnityEngine;

public class ChildAnimator : GenericAnimator
{
    protected static readonly int walkX = Animator.StringToHash("walkX");
    protected static readonly int walkY = Animator.StringToHash("walkY");

    public override void SetWalk(float speed)
    {
        Debug.LogError("Child cannot use SetWalk. Use SetWalkDirection instead.");
    }

    public override void SetWalkDirection(Vector2 direction)
    {
        animator.SetFloat(walkX, direction.x);
        animator.SetFloat(walkY, direction.y);
    }
}
