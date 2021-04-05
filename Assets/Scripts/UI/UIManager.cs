using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject mainUI;
    public GameObject gameOverUI;

    public GameObject player;
    private EntityHealth playerHealth;

    public GameObject bed;
    private EntityHealth bedHealth;

    public GameObject waveManagerObject;
    private WaveManager waveManager;

    public GameObject bedHealthSliderObject;
    private Slider bedHealthSlider;

    public GameObject healthSliderObject;
    private Slider healthSlider;

    public GameObject manaSliderObject;
    private Slider manaSlider;

    public GameObject waveNumberUI;
    private TextMeshProUGUI waveTextNumber;

    public GameObject buildingResourceNumberUI;
    private TextMeshProUGUI buildingResourceNumber;

    public GameObject levelManager;
    private BuildingResourceManager buildingResource;

    private TrapController trapController;
    public Color slotDefaultColor;
    public Color slotSelectedColor;
    public Image[] hotbarSlots;
        
    public TextMeshProUGUI trapControllerMessage;

    public float levelOverWaitTime = 4.0f;
    public TextMeshProUGUI levelOverMessage;

    public TextMeshProUGUI nextWaveMessage;

    private const string levelOverString = "Level victory! Continuing in {0}s...";

    // Start is called before the first frame update
    void Start()
    {
        // Lol what is this, Ben?
        healthSlider = healthSliderObject.GetComponent<Slider>();
        manaSlider = manaSliderObject.GetComponent<Slider>();
        playerHealth = player.GetComponent<EntityHealth>();
        bedHealth = bed.GetComponent<EntityHealth>();
        bedHealthSlider = bedHealthSliderObject.GetComponent<Slider>();
        waveTextNumber = waveNumberUI.GetComponent<TextMeshProUGUI>();
        buildingResourceNumber = buildingResourceNumberUI.GetComponent<TextMeshProUGUI>();
        waveManager = waveManagerObject.GetComponent<WaveManager>();
        buildingResource = levelManager.GetComponent<BuildingResourceManager>();

        trapController = player.GetComponent<TrapController>();
    }

    void FixedUpdate()
    {
        gameOver();

        SetMaxHealth(playerHealth.maxHealth);
        SetCurrentHealth(playerHealth.GetCurrentHealth());

        SetMaxBedHealth(bedHealth.maxHealth);
        SetCurrentBedHealth(bedHealth.GetCurrentHealth());

        SetWaveNumber(waveManager.GetWaveNumber(), waveManager.GetMaxWaveNumber());

        SetBuildingResourceNumber(buildingResource.GetBuildingResource());

        int placingTrapIndex = trapController.PlacingTrapIndex;
        for (int i = 0; i < hotbarSlots.Length; ++i)
            hotbarSlots[i].color = placingTrapIndex == i ?
                slotSelectedColor : slotDefaultColor;
        string getMessage = trapController.UIMessage;
        if (getMessage.Length == 0)
            trapControllerMessage.enabled = false;
        else
        {
            trapControllerMessage.enabled = true;
            trapControllerMessage.SetText(getMessage);
        }

        if(waveManager.LevelOver && levelOverMessage.enabled == false)
        {
            levelOverMessage.enabled = true;
            StopAllCoroutines();
            StartCoroutine(NextLevelRoutine());
        }
                
        nextWaveMessage.enabled = waveManager.IsWaveDone() && waveManager.LevelOver == false;
    }

    //Health bar setting functions.
    private void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
    }

    private void SetCurrentHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    //Mana bar setting functions.
    private void SetMaxMana(int maxMana)
    {
        manaSlider.maxValue = maxMana;
    }

    private void SetCurrentMana(int currentMana)
    {
        manaSlider.value = currentMana;
    }

    //Bed health bar setting functions.
    private void SetMaxBedHealth(int maxBedHealth)
    {
        bedHealthSlider.maxValue = maxBedHealth;
    }

    private void SetCurrentBedHealth(int currentBedHealth)
    {
        bedHealthSlider.value = currentBedHealth;
    }

    //Wave counter function.
    private void SetWaveNumber(int currentWave, int maxWaveCount)
    {
        waveTextNumber.SetText("{0} / {1}", currentWave, maxWaveCount);
    }

    //Building resource function.
    private void SetBuildingResourceNumber(int amountOfBuildingResource)
    {
        buildingResourceNumber.SetText("${0}", amountOfBuildingResource);
    }

    private IEnumerator NextLevelRoutine()
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        float waitUntil = Time.time + levelOverWaitTime;
        int previousSecondsLeft = -1;
        do
        {
            int secondsLeft = Mathf.CeilToInt(waitUntil - Time.time);
            if(previousSecondsLeft != secondsLeft)
            {
                previousSecondsLeft = secondsLeft;
                levelOverMessage.SetText(string.Format(levelOverString, secondsLeft));
            }
            yield return wait;
        }
        while (Time.time < waitUntil);

        // campaign manager
        CampaignManager.GetCampaignManager.NextLevel();
    }

    private void gameOver()
    {
        if(playerHealth.isGameOver || bedHealth.isGameOver)
        {
            mainUI.SetActive(false);
            gameOverUI.SetActive(true);
        }
    }
}
