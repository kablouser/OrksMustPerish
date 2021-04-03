using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Place on special trap slot prefabs, the trap grid will take care of the rest
/// </summary>
public class TrapSlot : MonoBehaviour
{
    GameObject trapDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowTrapDisplay(GameObject trap)
    {
        trapDisplay = Instantiate(trap, transform.position, transform.rotation, transform);
    }
    public void DestroyTrapDisplay()
    {
        Destroy(trapDisplay);
    }

    public void SpawnTrap(GameObject trap)
    {
        Instantiate(trap, transform.position, transform.rotation, transform);
    }
}
