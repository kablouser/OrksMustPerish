using UnityEngine;
using System.Collections.Generic;

public class EnemyCharacterController : MonoBehaviour
{
    public PathingMapManager mapManager;
    public EnemyAnimator animator;
    public Rigidbody rb;
    public float moveSpeed;
    [Tooltip("Degrees per second")]
    public float rotateSpeed;

    [Header("How much this pushes other enemies")]
    public float pushOtherEnemy;
    [Header("How much other enemies pushes this")]
    public float otherEnemyPush;

    [SerializeField]
    private List<EnemyCharacterController> insideTrigger;

    private Vector3 targetVelocity;

    private void FixedUpdate()
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

        Vector3 combinedVelocity = targetVelocity * moveSpeed;
        if (otherEnemyPush != 0.0f)
            for (int i = 0; i < insideTrigger.Count; ++i)
            {
                Vector3 awayVector = transform.position - insideTrigger[i].transform.position;
                awayVector.y = 0;
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
}
