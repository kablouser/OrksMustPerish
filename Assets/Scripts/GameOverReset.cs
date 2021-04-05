using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverReset : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.R))
        {
            CampaignManager.GetCampaignManager.RestartLevel();
        }
    }
}
