using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyCharacterController : MonoBehaviour
{
    public PathingMapManager mapManager;
    public GenericAnimator animator;
    public Rigidbody rb;
    public float moveSpeed;
    public float slowSpeed = 1f;
    [Tooltip("Degrees per second")]
    public float rotateSpeed;

    [Header("How much this pushes other enemies")]
    public float pushOtherEnemy;
    [Header("How much other enemies pushes this")]
    public float otherEnemyPush;

    public LayerMask playerDetectionMask;
    public float attackRange;
    public int attackDamage;
    [Tooltip("Warning. Goes in a straight line towards the player. Too much range means enemy can get stuck!")]
    public float chaseRange = 1.5f;
    
    [SerializeField]
    private List<EnemyCharacterController> insideTrigger;

    private Vector3 targetVelocity;
    private Collider[] sphereCastResults;

    private EntityHealth currentTarget;

    private static readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private void Start()
    {
        const int sphereCastResultsLength = 10;
        sphereCastResults = new Collider[sphereCastResultsLength];
        currentTarget = null;
        slowSpeed = 1;
    }

    private void FixedUpdate()
    {
        if (currentTarget == null)
        {
            // follow current path
            Vector2Int mapPosition = mapManager.pathingMap.WorldToMap(transform.position);
            if (mapManager.pathingMap.InRange(mapPosition))
            {
                Vector2Int nextMapPosition = mapManager.pathingMap.map[mapManager.pathingMap.GetMapIndex(mapPosition)].cameFrom;
                Vector3 nextWorldPosition = mapManager.pathingMap.MapToWorld(nextMapPosition);
                SetMove(nextWorldPosition - transform.position);
            }
            else
                SetMove(Vector3.zero);

            // check for a target, attack if in range
            EntityHealth target = GetChaseTarget();
            if (target != null)
                Chase(target);
        }

        Vector3 combinedVelocity = targetVelocity * (moveSpeed * slowSpeed);
        if (otherEnemyPush != 0.0f)
            for (int i = 0; i < insideTrigger.Count; ++i)
            {
                Vector3 awayVector = transform.position - insideTrigger[i].transform.position;
                awayVector.y = 0;
                if (awayVector.sqrMagnitude == 0.0f)
                {
                    awayVector = Random.insideUnitSphere;
                    awayVector.y = 0;
                }
                else
                    awayVector.Normalize();
                combinedVelocity += awayVector * insideTrigger[i].pushOtherEnemy * otherEnemyPush;
            }
        animator.SetWalk(combinedVelocity.magnitude);
        rb.velocity = new Vector3(combinedVelocity.x, rb.velocity.y, combinedVelocity.z);        
    }

    private void SetMove(Vector3 direction)
    {
        direction.y = 0;
        direction.Normalize();
        targetVelocity = direction;
        if (direction != Vector3.zero)
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(direction), rotateSpeed * Time.fixedDeltaTime);
    }

    private void SetRotation(Vector3 direction)
    {
        direction.y = 0;
        if (direction != Vector3.zero)
            rb.rotation = Quaternion.LookRotation(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.attachedRigidbody != null)
        {
            var otherEnemy = other.attachedRigidbody.GetComponent<EnemyCharacterController>();
            if (otherEnemy != null)
                insideTrigger.Add(otherEnemy);
        }
        else
        {
            var bed = other.GetComponentInParent<EntityHealth>();
            if (bed != null && bed.isBed)
            {
                bed.DamageMe(10);
                GetComponent<EntityHealth>().ReachedBed();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger && other.attachedRigidbody != null)
        {
            var otherEnemy = other.attachedRigidbody.GetComponent<EnemyCharacterController>();
            if (otherEnemy != null)
                insideTrigger.Remove(otherEnemy);
        }
    }

    private void OnDestroy()
    {
        for(int i = 0; i < insideTrigger.Count; ++i)
            insideTrigger[i].insideTrigger.Remove(this);
        insideTrigger.Clear();
    }

    void Chase(EntityHealth newTarget)
    {
        currentTarget = newTarget;
        StopAllCoroutines();
        StartCoroutine(ChaseRoutine());
    }

    EntityHealth GetChaseTarget()
    {
        int results = Physics.OverlapSphereNonAlloc(transform.position, chaseRange, sphereCastResults, playerDetectionMask);
        for (int i = 0; i < results; ++i)
        {
            Rigidbody rigidbody = sphereCastResults[i].attachedRigidbody;
            if (rigidbody != null)
            {
                EntityHealth player = rigidbody.GetComponent<EntityHealth>();
                if (player != null && player.isPlayer && IsTargetChasible(player))
                {
                    return player;
                }
            }
        }
        return null;
    }

    bool IsTargetInRange(EntityHealth target)
    {
        int results = Physics.OverlapSphereNonAlloc(transform.position, attackRange, sphereCastResults, playerDetectionMask);
        for (int i = 0; i < results; ++i)
        {
            Rigidbody rigidbody = sphereCastResults[i].attachedRigidbody;
            if (rigidbody != null)
            {
                EntityHealth compare = rigidbody.GetComponent<EntityHealth>();
                if (compare == target)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool IsTargetChasible(EntityHealth target)
    {
        return Vector3.Distance(target.transform.position, transform.position) < chaseRange;
    }

    IEnumerator ChaseRoutine()
    {
        while(true)
        {
            if (IsTargetInRange(currentTarget))
            {
                yield return AttackRoutine();
                continue;
            }
            else if (IsTargetChasible(currentTarget))
            {
                SetMove(currentTarget.transform.position - transform.position);
                yield return waitForFixedUpdate;
                continue;
            }
            else
            {
                break;
            }
        }
        currentTarget = null;
    }

    IEnumerator AttackRoutine()
    {
        SetMove(Vector3.zero);
        SetRotation(currentTarget.transform.position - transform.position);
        animator.TriggerAttack(out float attackDuration, out float damageTimestamp);

        yield return new WaitForSeconds(damageTimestamp);

        currentTarget.DamageMe(attackDamage);

        yield return new WaitForSeconds(attackDuration - damageTimestamp);
    }
}
