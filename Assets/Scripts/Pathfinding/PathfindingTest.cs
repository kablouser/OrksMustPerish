using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour
{
    public int mapX;
    public int mapY;
    private Node[,] myMap;

    public Vector3 start;
    public Vector3 end;
    public Vector3 mapOrigin;
    public Vector3 mapNodeSize = Vector3.one;

    [ContextMenu("Start")]
    // Start is called before the first frame update
    void Start()
    {
        myMap = new Node[mapX, mapY];
        for (int i = 0; i < mapX; ++i)
            for (int j = 0; j < mapY; ++j)
                myMap[j, i].isWalkable = true;
    }

    void OnGUI()
    {
        for (int j = 0; j < myMap.GetLength(0); ++j)
            for (int i = 0; i < myMap.GetLength(1); ++i)
            {
                bool newVal = GUI.Toggle(new Rect(i * 20, (myMap.GetLength(0) - 1 - j) * 20, 20, 20), myMap[j, i].isWalkable, GUIContent.none);
                if(newVal != myMap[j, i].isWalkable)
                {
                    print(i + ", " + j);
                }
                myMap[j, i].isWalkable = newVal;
            }

        if (GUI.Button(new Rect(0, myMap.GetLength(0) * 20, 100, 50), "Solve"))
            Solve();
    }
    
    [ContextMenu("Solve")]
    void Solve()
    {
        bool success = Pathfinding.Solve(myMap, mapOrigin, mapNodeSize,
            start, 
            end, 
            out List<Vector3> path,
            out List<Vector3> breakablePath);

        Debug.Log("success="+success);
        if (path != null)
        {
            path.Insert(0, start);
            GetComponent<LineRenderer>().positionCount = path.Count;
            GetComponent<LineRenderer>().SetPositions(path.ToArray());
        }
        else if (breakablePath != null)
        {
            breakablePath.Insert(0, start);
            GetComponent<LineRenderer>().positionCount = breakablePath.Count;
            GetComponent<LineRenderer>().SetPositions(breakablePath.ToArray());
        }
    }
}
