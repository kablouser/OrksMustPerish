using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject explosionPrefab;

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
        if(distanceTraveled >= range)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(explosion, 2);

            Destroy(gameObject);
        }

        lastDistance = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player" && other.gameObject.tag != "Projectile")
        {
            if(other.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponentInParent<EntityHealth>().DamageMe(damage);
            }

            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(explosion, 2);

            Destroy(gameObject);
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
}
