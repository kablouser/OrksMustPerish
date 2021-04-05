using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignManager : MonoBehaviour
{
    public static CampaignManager GetCampaignManager { get; private set; }

    [SerializeField]
    [Header("Include title screen scene")]
    private int[] allLevels;
    private int currentLevel;

    private void Awake()
    {
        if(GetCampaignManager == null)
        {
            DontDestroyOnLoad(gameObject);
            GetCampaignManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ContextMenu("Next Level")]
    public void NextLevel()
    {
        ++currentLevel;
        if (allLevels.Length <= currentLevel)
            currentLevel = 0;
        if (currentLevel < allLevels.Length)
            SceneManager.LoadScene(allLevels[currentLevel]);        
    }

    [ContextMenu("Restart Level")]
    public void RestartLevel()
    {
        SceneManager.LoadScene(allLevels[currentLevel], LoadSceneMode.Single);
    }
}
