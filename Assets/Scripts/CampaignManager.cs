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

    public void NextLevel()
    {
        ++currentLevel;
        if (currentLevel < allLevels.Length)
            SceneManager.LoadScene(allLevels[currentLevel]);
        
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(allLevels[currentLevel], LoadSceneMode.Single);
    }
}
