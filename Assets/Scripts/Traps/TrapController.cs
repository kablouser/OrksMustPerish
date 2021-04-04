using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to player, place traps in the appropriate public game objects, and that's about it
/// </summary>
public class TrapController : MonoBehaviour
{

    public GameObject[] traps;

    GameObject trapDisplay;
    GameObject display;
    bool isPlacing = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HotKeyTrap();
        if (isPlacing)
        {
            ShowTrap();
            PlaceTrap();
        }
    }

    private void HotKeyTrap()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isPlacing = true;
            trapDisplay = traps[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isPlacing = true;
            trapDisplay = traps[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            isPlacing = true;
            trapDisplay = traps[2];
        }
    }

    private void ShowTrap()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform.gameObject.GetComponent<TrapSlot>())
            {
                if (display != hit.transform.gameObject)
                {
                    if (display != null && display.GetComponent<TrapSlot>())
                        display.GetComponent<TrapSlot>().DestroyTrapDisplay();
                    display = hit.transform.gameObject;
                    hit.transform.gameObject.GetComponent<TrapSlot>().ShowTrapDisplay(trapDisplay);
                }
            }
        }
    }

    private void PlaceTrap()
    {
        if (display != null && display.GetComponent<TrapSlot>())
        {
            if (Input.GetMouseButtonDown(0))
            {
                display.GetComponent<TrapSlot>().SpawnTrap(trapDisplay);
            }
            if (Input.GetMouseButtonDown(1))
            {
                display.GetComponent<TrapSlot>().DestroyTrapDisplay();
                isPlacing = false;
                display = null;
            }
        }
    }
}
