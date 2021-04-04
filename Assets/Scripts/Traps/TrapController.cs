using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to player, place traps in the appropriate public game objects, and that's about it
/// </summary>
public class TrapController : MonoBehaviour
{
    public LayerMask trapLayer;
    public GameObject[] traps;
    public int[] trapCost;

    GameObject trapDisplay;
    GameObject display;
    bool isPlacing = false;
    bool isRemoving = false;
    
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
        else if (isRemoving)
        {
            RemoveTrap();
        }
    }

    private void HotKeyTrap()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //if (trapCost[0] <= GameObject.Find("LevelManager").GetComponent<BuildingResourceManager>().GetBuildingResource())
            {
                isPlacing = true;
                trapDisplay = traps[0];
                isRemoving = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //if (trapCost[1] <= GameObject.Find("LevelManager").GetComponent<BuildingResourceManager>().GetBuildingResource())
            {
                isPlacing = true;
                trapDisplay = traps[1];
                isRemoving = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
           //if (trapCost[2] <= GameObject.Find("LevelManager").GetComponent<BuildingResourceManager>().GetBuildingResource())
            {
                isPlacing = true;
                trapDisplay = traps[2];
                isRemoving = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isPlacing = false;
            isRemoving = true;
            if (display != null)
            {
                display.GetComponent<TrapSlot>().DestroyTrapDisplay();
                display = null;
            }
        }
    }

    private void ShowTrap()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, ~trapLayer))
        {
            if (hit.transform.gameObject.GetComponent<TrapSlot>())
            {
                if (!hit.transform.gameObject.GetComponent<TrapSlot>().trapPlaced)
                {
                    if (display != hit.transform.gameObject)
                    {
                        if (display != null && display.GetComponent<TrapSlot>())
                            display.GetComponent<TrapSlot>().DestroyTrapDisplay();
                        display = hit.transform.gameObject;

                    }
                    hit.transform.gameObject.GetComponent<TrapSlot>().DestroyTrapDisplay();
                    hit.transform.gameObject.GetComponent<TrapSlot>().ShowTrapDisplay(trapDisplay);
                }
                else if (display != null)
                {
                    display.GetComponent<TrapSlot>().DestroyTrapDisplay();
                    display = null;
                }
            }
            else if (display != null && display.GetComponent<TrapSlot>())
            {
                display.GetComponent<TrapSlot>().DestroyTrapDisplay();
            }
        }
        
    }

    private void RemoveTrap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, ~trapLayer))
            {
                if (hit.transform.gameObject.GetComponent<TrapSlot>())
                {
                    hit.transform.gameObject.GetComponent<TrapSlot>().DestroyTrap();
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            isRemoving = false;
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
