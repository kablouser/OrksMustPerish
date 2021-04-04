using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Place this on room tile object (i.e. 5x5 hall), put TrapSlot object in the public gridPiece object, set grid size appropriatly to fill room with trap grid
/// </summary>
public class TrapGrid : MonoBehaviour
{
    public GameObject gridPiece;
    public float gridSize;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 gPosition = Vector3.zero;
        float modSize = (gridSize / 2) * 4 - 2;
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {

                GameObject trapSlot = Instantiate(gridPiece, new Vector3((4 * x) + (transform.position.x - modSize), transform.position.y, (4 * z) + (transform.position.z - modSize)), transform.rotation, transform);
                trapSlot.GetComponent<BoxCollider>().enabled = false;
                trapSlot.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
