using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    public bool isPlayer;

    private WaveManager waveManager;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Dead();
    }

    //Check to see if dead.
    void Dead()
    {
        if(currentHealth <= 0)
        {
            if(isPlayer)
            {
                //Some sort of respawn thing here please.!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //Temp.
                Debug.Log("Player has died");
                currentHealth = maxHealth;
            }
            else
            {
                waveManager.AddEnemyDeath();
                Destroy(gameObject);
            }
        }
    }

    //Used to damage entity.
    public void DamageMe(int damageTaken)
    {
        currentHealth -= damageTaken;
    }

    //Used to heal entity.
    public void HealMe(int amountHealed)
    {
        if (currentHealth + amountHealed <= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amountHealed;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    //Set the wave manager
    public void SetWaveManager(WaveManager waveManager)
    {
        this.waveManager = waveManager;
    }
}
