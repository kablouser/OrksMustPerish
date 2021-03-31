using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth;
    private int health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Dead();
    }

    //Check to see if dead.
    void Dead()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    //Used to damage entity.
    public void DamageMe(int damageTaken)
    {
        health -= damageTaken;
    }

    //Used to heal entity.
    public void HealMe(int amountHealed)
    {
        if (health + amountHealed <= maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amountHealed;
        }
    }
}
