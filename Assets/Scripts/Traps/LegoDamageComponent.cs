using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoDamageComponent : MonoBehaviour
{
    public int damage = 1;
    public float damageTimer = 0.5f;

    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<EntityHealth>().DamageMe(damage);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= damageTimer)
        {
            timer = 0;
            gameObject.GetComponent<EntityHealth>().DamageMe(damage);
        }
    }
}
