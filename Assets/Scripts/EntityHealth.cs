using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    public bool isPlayer;
    public bool isBed;

    public int buildingResourceWorth;

    private WaveManager waveManager;
    private BuildingResourceManager buildingResourceManager;

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
            else if(isBed)
            {
                //Some game over thing here.
                Debug.Log("GAME OVER");
            }
            else
            {
                waveManager.AddEnemyDeath();
                buildingResourceManager.AddBuildingResource(buildingResourceWorth);
                Destroy(gameObject);
            }
        }
    }

    //Enemy reached the bed.
    public void ReachedBed()
    {
        waveManager.AddEnemyDeath();
        Destroy(gameObject);
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

    //Set the building resource manager.
    public void SetBuildingResourceManager(BuildingResourceManager buildingResource)
    {
        this.buildingResourceManager = buildingResource;
    }
}
