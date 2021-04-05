using UnityEngine;

[System.Serializable]
public struct WaveStructure
{
    public GameObject[] enemiesToSpawn;
    public GameObject[] spawnPoint;
    public float[] enemySpawnIntival;
}

public class WaveManager : MonoBehaviour
{
    public WaveStructure[] wavesInLevel;

    private BuildingResourceManager buildingResourceManager;

    private int maxWaveNumber;
    private int waveNumber = 0;

    private int maxEnemiesInWave = 0;
    private int enemiesDead = 0;

    private int enemiesSpawned = 0;

    private bool spawningEnemies = false;

    public bool LevelOver { get; private set; }

    private float timeToSpawn;

    private PathingMapManager pathingMapManager;

    // Start is called before the first frame update
    void Start()
    {
        LevelOver = false;
        maxWaveNumber = wavesInLevel.Length;
        pathingMapManager = GetComponent<PathingMapManager>();
        buildingResourceManager = GetComponent<BuildingResourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if all enemies are dead and the level is not over.
        if(IsWaveDone() && !LevelOver)
        {
            //If the final wave has already passed then the level is over.
            if(waveNumber == maxWaveNumber)
            {
                LevelOver = true;
            }
            else if(Input.GetKeyDown(KeyCode.T))
            {
                //Debug.Log("Spawning new wave");

                spawningEnemies = true;
                maxEnemiesInWave = wavesInLevel[waveNumber].enemiesToSpawn.Length;
                enemiesDead = 0;
            }
        }
        
        //Sees if next enemy can be spawned and does so if it can.
        if(spawningEnemies)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        //Checks to see if all enemies have been spawned, if they have it stops spawning enemies and resets the enemies spawned to 0.
        if(enemiesSpawned == maxEnemiesInWave)
        {
            spawningEnemies = false;
            enemiesSpawned = 0;

            return;
        }

        //Spawns an enemy if the alloted time has passed since the last spawn.
        if(Time.time >= timeToSpawn)
        {
            timeToSpawn = Time.time + wavesInLevel[waveNumber].enemySpawnIntival[enemiesSpawned];

            GameObject enemy = Instantiate(wavesInLevel[waveNumber].enemiesToSpawn[enemiesSpawned], wavesInLevel[waveNumber].spawnPoint[enemiesSpawned].transform);
            enemy.GetComponent<EntityHealth>().SetWaveManager(this);
            enemy.GetComponent<EntityHealth>().SetBuildingResourceManager(buildingResourceManager);
            enemy.GetComponent<EnemyCharacterController>().mapManager = pathingMapManager;

            enemiesSpawned += 1;

            //Debug.Log("Enemies Spawned: " + enemiesSpawned);
        }
    }

    //Checks if all enemies are dead.
    private bool IsWaveDone()
    {
        if(enemiesDead == maxEnemiesInWave)
        {
            return true;
        }
        return false;
    }

    //Function to add to enemies dead counter.
    public void AddEnemyDeath()
    {
        enemiesDead += 1;

        //If all enemies are dead the wave counter will increase.
        if(IsWaveDone())
        {
            waveNumber += 1;
        }
        //Debug.Log("Enemies Dead: " + enemiesDead);
        //Debug.Log("Enemies max: " + maxEnemiesInWave);
    }

    //Get wave information functions.
    public int GetMaxWaveNumber()
    {
        return maxWaveNumber;
    }

    public int GetWaveNumber()
    {
        if(waveNumber == maxWaveNumber)
        {
            return waveNumber;
        }
        
        return waveNumber + 1;
    }
}
