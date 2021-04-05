using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
