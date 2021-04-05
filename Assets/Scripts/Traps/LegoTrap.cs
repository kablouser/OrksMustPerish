using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoTrap : MonoBehaviour
{
    public int damage = 1;
    public float damageTime = 1;

    private float timeToDamage;

    private LayerMask mask;

    private void Start()
    {
        mask = LayerMask.GetMask("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= timeToDamage)
        {
            //Debug.Log("DOING DMAGE");
            timeToDamage = Time.time + damageTime;
            DoDamage();
        }
    }

    private void DoDamage()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 2, Quaternion.identity, mask);
        int i = 0;

        while(i < hitColliders.Length)
        {
            Debug.Log("Hit : " + hitColliders[i].name + i);
            hitColliders[i].GetComponentInParent<EntityHealth>().DamageMe(damage);
            i++;
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireCube(transform.position, transform.localScale * 4);
    //}
}
