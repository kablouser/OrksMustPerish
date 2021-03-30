using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
    [System.Serializable]
    public struct Dummy
    {
        public bool[] array;
    }

    public Dummy[] serialMap;
    public int mapX;
    public int mapY;

    public List<Vector2Int> outPath;
    public List<Vector2Int> outBreakable;

    [ContextMenu("Solve")]
    // Start is called before the first frame update
    void Start()
    {
        Node[,] map = new Node[mapX, mapY];
        for (int i = 0; i < mapX; ++i)
            for (int j = 0; j < mapY; ++j)
                map[j, i].isWalkable = serialMap[j].array[i];

        bool success = Pathfinding.Solve(map, new Vector2Int(0, 0), new Vector2Int(2, 2), out outPath, out outBreakable);
        Debug.Log(success);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
