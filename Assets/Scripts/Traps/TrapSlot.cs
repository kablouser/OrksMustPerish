using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Place on special trap slot prefabs, the trap grid will take care of the rest
/// </summary>
public class TrapSlot : MonoBehaviour
{
    public bool isDisplaying = false;
    public bool trapPlaced = false;
    GameObject trapDisplay;
    GameObject spawnedTrap;
    // Start is called before the first frame update
    void Start()
    {
        //Physics.OverlapBox(transform.position, transform.localScale / 2);Physics.OverlapBox(transform.position, gameObject.GetComponent<BoxCollider>().size / 3);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var other in hitColliders)
        {
            if (other.tag == "Prop")
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowTrapDisplay(GameObject trap)
    {
        if (!trapPlaced)
        {
            isDisplaying = true;
            trapDisplay = Instantiate(trap, transform.position, transform.rotation, transform);
        }
    }
    public void DestroyTrapDisplay()
    {
        isDisplaying = false;
        if (trapDisplay != null)
        {
            Destroy(trapDisplay);
        }
    }

    public void SpawnTrap(GameObject trap)
    {
        if (!trapPlaced)
        {
            spawnedTrap = Instantiate(trap, transform.position, transform.rotation, transform);
            trapPlaced = true;
        }
    }

    public void DestroyTrap()
    {
        if (trapPlaced)
        {
            Destroy(spawnedTrap);
            DestroyTrapDisplay();
            trapPlaced = false;
        }
    }
}
