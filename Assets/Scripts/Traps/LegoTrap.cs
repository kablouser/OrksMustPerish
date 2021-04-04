using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoTrap : MonoBehaviour
{
    public int damage = 1;
    public float damageTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EntityHealth>() && !other.GetComponent<LegoDamageComponent>())
        {
            if (other.GetComponent<EntityHealth>().isPlayer)
            {
                other.gameObject.AddComponent(typeof(LegoDamageComponent));
                other.GetComponent<LegoDamageComponent>().damage = damage;
                other.GetComponent<LegoDamageComponent>().damageTimer = damageTime;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<EntityHealth>() && !other.GetComponent<LegoDamageComponent>())
        {
            if (other.GetComponent<EntityHealth>().isPlayer)
            {
                other.gameObject.AddComponent(typeof(LegoDamageComponent));
                other.GetComponent<LegoDamageComponent>().damage = damage;
                other.GetComponent<LegoDamageComponent>().damageTimer = damageTime;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EntityHealth>())
        {
            if (other.GetComponent<EntityHealth>().isPlayer)
            {
                Destroy(other.GetComponent<LegoDamageComponent>());
                //other.GetComponent<EntityHealth>().DamageMe(damage);
            }
        }
    }
}
