using UnityEngine;

public abstract class GenericAnimator : MonoBehaviour
{
    protected static readonly int attackTrigger = Animator.StringToHash("attack");
    protected static readonly int walkBlend = Animator.StringToHash("walkBlend");
    protected static readonly int mainTexture = Shader.PropertyToID("_MainTex");

    public Animator animator;
    public float attackDuration;
    public float damageTimestamp;

    /// <returns>duration for the attack animation</returns>
    public virtual void TriggerAttack(out float attackDuration, out float damageTimestamp)
    {
        animator.SetTrigger(attackTrigger);
        attackDuration = this.attackDuration;
        damageTimestamp = this.damageTimestamp;
    }

    public abstract void SetWalk(float speed);

    public abstract void SetWalkDirection(Vector2 direction);
}
