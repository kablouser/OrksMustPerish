using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float impactFXLife = 2;
    public GameObject impactFX;
    public float fizzleFXLife = 2;
    [Tooltip("This effect it should be visually distinct from impacting something.")]
    public GameObject fizzleFX;
    public float decoupleFXLife = .5f;
    public GameObject decoupleFX;

    private int damage;
    private float range;

    private float distanceTraveled;
    private Vector3 lastDistance;

    private void Awake()
    {
        lastDistance = transform.position;
    }

    private void Update()
    {
        distanceTraveled += Vector3.Distance(transform.position, lastDistance);
        if (distanceTraveled >= range)            
            Fizzle();

         lastDistance = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Projectile"))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponentInParent<EntityHealth>().DamageMe(damage);
            }
            // get close to impact point
            const float surfaceOffset = 0.1f;
            ContactPoint contactPoint = collision.GetContact(0);
            transform.position = contactPoint.point + contactPoint.normal * surfaceOffset;
            Impact();
        }
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetRange(float range)
    {
        this.range = range;
    }

    private void Impact()
    {
        if (impactFX != null)
        {
            GameObject impact = Instantiate(impactFX, transform.position, transform.rotation);
            Destroy(impact, impactFXLife);
        }
        if (decoupleFX != null)
        {
            decoupleFX.transform.parent = null;
            Destroy(decoupleFX.gameObject, decoupleFXLife);
        }
        Destroy(gameObject);
    }

    private void Fizzle()
    {
        if (fizzleFX != null)
        {
            GameObject fizzle = Instantiate(fizzleFX, transform.position, transform.rotation);
            Destroy(fizzle, fizzleFXLife);
        }
        if(decoupleFX != null)
        {
            decoupleFX.transform.parent = null;
            Destroy(decoupleFX.gameObject, decoupleFXLife);
        }
        Destroy(gameObject);
    }
}
