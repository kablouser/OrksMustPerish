using UnityEngine;

public abstract class EnemyAnimator : MonoBehaviour
{
    protected static readonly int attackTrigger = Animator.StringToHash("attack");
    protected static readonly int walkBlend = Animator.StringToHash("walkBlend");
    protected static readonly int mainTexture = Shader.PropertyToID("_MainTex");

    public Animator animator;

    public virtual void TriggerAttack()
    {
        animator.SetTrigger(attackTrigger);
    }

    public abstract void SetWalk(float speed);
}
