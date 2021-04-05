using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCatcher : MonoBehaviour
{
    public Transform respawnPlayerPosition;

    private void OnTriggerEnter(Collider other)
    {
        EntityHealth health = other.attachedRigidbody.GetComponent<EntityHealth>();
        if(health != null)
        {
            if(health.isPlayer)
            {
                health.transform.position = respawnPlayerPosition.position;
                other.attachedRigidbody.velocity = Vector3.zero;
            }
            else
                health.DamageMe(health.maxHealth);
        }
    }
}
