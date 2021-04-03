public class LittleEnemyAnimator : EnemyAnimator
{
    public float maxAnimationSpeed = 4;

    public override void SetWalk(float speed)
    {
        animator.SetFloat(walkBlend, speed / maxAnimationSpeed);
    }
}
