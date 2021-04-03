using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        healthSlider = healthSliderObject.GetComponent<Slider>();
        manaSlider = manaSliderObject.GetComponent<Slider>();
        playerHealth = player.GetComponent<EntityHealth>();
        bedHealth = bed.GetComponent<EntityHealth>();
        bedHealthSlider = bedHealthSliderObject.GetComponent<Slider>();
        waveTextNumber = waveNumberUI.GetComponent<TextMeshProUGUI>();
        buildingResourceNumber = buildingResourceNumberUI.GetComponent<TextMeshProUGUI>();
        waveManager = waveManagerObject.GetComponent<WaveManager>();
    }

    void FixedUpdate()
    {
        SetMaxHealth(playerHealth.maxHealth);
        SetCurrentHealth(playerHealth.GetCurrentHealth());

        SetMaxBedHealth(bedHealth.maxHealth);
        SetCurrentBedHealth(bedHealth.GetCurrentHealth());

        SetWaveNumber(waveManager.GetWaveNumber(), waveManager.GetMaxWaveNumber());
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
        buildingResourceNumber.SetText("{0}", amountOfBuildingResource);
    }
}
