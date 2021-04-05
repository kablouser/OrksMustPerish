using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyCharacterController : MonoBehaviour
{
    public PathingMapManager mapManager;
    public TrapGrid trapGrid;
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
    public LayerMask entityHealthDetectionMask;
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

                // check if its a barricade
                if (CheckBarricade(nextWorldPosition) == false)
                {
                    bool isDiagonal = 1 < (nextMapPosition - mapPosition).sqrMagnitude;
                    if (isDiagonal)
                    {
                        Vector3 position1 = mapManager.pathingMap.MapToWorld(new Vector2Int(nextMapPosition.x, mapPosition.y));
                        Vector3 position2 = mapManager.pathingMap.MapToWorld(new Vector2Int(mapPosition.x, nextMapPosition.y));

                        Vector3 closest, furthest;
                        if ((position1 - transform.position).sqrMagnitude < (position2 - transform.position).sqrMagnitude)
                        {
                            closest = position1;
                            furthest = position2;
                        }
                        else
                        {
                            closest = position2;
                            furthest = position1;
                        }

                        if( CheckBarricade(closest) ||
                            CheckBarricade(furthest))
                        { }
                    }
                }
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
        int results = Physics.OverlapSphereNonAlloc(transform.position, attackRange, sphereCastResults, entityHealthDetectionMask);
        for (int i = 0; i < results; ++i)
        {
            EntityHealth compare = null;
            Rigidbody rigidbody = sphereCastResults[i].attachedRigidbody;
            if (rigidbody != null)
                compare = rigidbody.GetComponent<EntityHealth>();
            else if (sphereCastResults[i].GetComponent<EntityHealth>())
                compare = sphereCastResults[i].GetComponent<EntityHealth>();

            if (compare == target)
                return true;
        }
        return false;
    }

    bool IsTargetChasible(EntityHealth target)
    {
        return Vector3.Distance(target.transform.position, transform.position) < chaseRange;
    }

    bool CheckBarricade(Vector3 atPosition)
    {
        TrapSlot trapSlot = trapGrid.GetSlot(atPosition, out _);
        if (trapSlot != null)
        {
            GenericTrap trap = trapSlot.GetPlacedTrap();
            if (trap != null)
            {
                EntityHealth health = trap.GetComponent<EntityHealth>();
                if (health != null)
                {
                    Chase(health);
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator ChaseRoutine()
    {
        while(currentTarget != null)
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
