using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int maxHealth;
    [SerializeField]
    private int currentHealth;

    public bool isPlayer;
    public bool isBed;
    public bool isBarricade;

    public int buildingResourceWorth;

    public bool isGameOver = false;

    public GameObject player;

    private WaveManager waveManager;
    private BuildingResourceManager buildingResourceManager;

    public GameOverReset gameOverReset;

    private List<Color> originalMaterialsColor = new List<Color>();
    private Renderer objectRenderer;
    private bool takeingDamage = false;
    private float flashTime = 0.2f;
    private float timeToDamageOver;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        objectRenderer = GetComponentInChildren<Renderer>();
        for(int i = 0; i < objectRenderer.materials.Length; i++)
        {
            originalMaterialsColor.Add(objectRenderer.materials[i].color);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(takeingDamage && Time.time >= timeToDamageOver)
        {
            //Debug.Log("Reseting color");
            takeingDamage = false;
            for(int i = 0; i < objectRenderer.materials.Length; i++)
            {
                objectRenderer.materials[i].color = originalMaterialsColor[i];
            }
        }

        Dead();
    }

    //Check to see if dead.
    void Dead()
    {
        if(currentHealth <= 0 && !isGameOver)
        {
            if(isPlayer)
            {
                //TODO Some sort of respawn thing here please.!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //Temp.
                //Debug.Log("Player has died");
                isGameOver = true;
                GetComponent<ThirdPersonCharacterController>().enabled = false;
                GetComponent<WandWeapon>().enabled = false;
                gameOverReset.enabled = true;
            }
            else if(isBed)
            {
                //Some game over thing here.
                //Debug.Log("GAME OVER");
                isGameOver = true;
                player.GetComponent<ThirdPersonCharacterController>().enabled = false;
                player.GetComponent<WandWeapon>().enabled = false;
                gameOverReset.enabled = true;
            }
            else if(isBarricade)
            {
                GetComponent<GenericTrap>().transform.parent.GetComponent<TrapSlot>().EndDeleteTrap(true, null, 0);
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
        //Debug.Log("I TOOK DAMAGEEEE");
        currentHealth -= damageTaken;

        takeingDamage = true;
        for(int i = 0; i < objectRenderer.materials.Length; i++)
        {
            objectRenderer.materials[i].color = Color.red;
        }
        timeToDamageOver = Time.time + flashTime;
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
