using System.Collections.Generic;
using UnityEngine;

public class PathingTest : MonoBehaviour
{
    public int mapX;
    public int mapY;
    private PathingNode[,] myMap;

    public Vector3 start;
    public Vector3 end;
    public Vector3 mapOrigin;
    public Vector3 mapNodeSize = Vector3.one;

    [ContextMenu("Start")]
    // Start is called before the first frame update
    void Start()
    {
        myMap = new PathingNode[mapX, mapY];
        for (int i = 0; i < mapX; ++i)
            for (int j = 0; j < mapY; ++j)
                myMap[j, i].isWalkable = true;

        // Example : queueing up a pathfinding job
        PathingJob job = new PathingJob(myMap, mapOrigin, mapNodeSize, start, end);
        job.WaitForStart();
        job.Stop();
        job.WaitForStart();
        // Debug.Log("tryStart" + tryStart);
    }

    void OnGUI()
    {
        for (int j = 0; j < myMap.GetLength(0) && j < 20; ++j)
            for (int i = 0; i < myMap.GetLength(1) && i < 20; ++i)
                myMap[j, i].isWalkable = GUI.Toggle(new Rect(i * 20, (myMap.GetLength(0) - 1 - j) * 20, 20, 20), myMap[j, i].isWalkable, GUIContent.none);
        if (GUI.Button(new Rect(0, myMap.GetLength(0) * 20, 100, 50), "Solve"))
            Solve();
    }
    
    [ContextMenu("Solve")]
    void Solve()
    {
        bool success = Pathing.Solve(
            myMap, mapOrigin, mapNodeSize,
            start,
            end,
            out List<Vector3> path);

        Debug.Log("success="+success);
        if (path != null)
        {
            path.Insert(0, start);
            GetComponent<LineRenderer>().positionCount = path.Count;
            GetComponent<LineRenderer>().SetPositions(path.ToArray());
        }
    }

    [ContextMenu("Performance")]
    void MeasurePerformance()
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        const int tests = 1;

        stopwatch.Start();

        for (int test = 0; test < tests; test++)
        {
            stopwatch.Stop();
            for (int i = 0; i < mapX; ++i)
                for (int j = 0; j < mapY; ++j)
                    myMap[j, i].isWalkable = Random.value < 0.7f;
            stopwatch.Start();

            bool success = Pathing.Solve(myMap, mapOrigin, mapNodeSize,
                start,
                end,
                out List<Vector3> path);
        }
        stopwatch.Stop();

        Debug.LogFormat("Average ms {0}", stopwatch.ElapsedMilliseconds / (double)tests);
    }

    [ContextMenu("Binary Tree Test")]
    void BinaryTreeTest()
    {
        BinaryTree<int, string> binaryTree = new BinaryTree<int, string>();
        binaryTree.Initialise();

        binaryTree.Add(3, "Three");        
        binaryTree.Add(2, "Two");        
        binaryTree.Add(4, "Four");
        binaryTree.Add(1, "One");

        while (binaryTree.Pop(out int key, out string value))
            Debug.LogFormat("1st {0}, {1}", key, value);

        binaryTree.Initialise();

        binaryTree.Add(2, "Two");
        binaryTree.Add(1, "One");
        binaryTree.Add(4, "Four");
        binaryTree.Add(3, "Three");        

        while (binaryTree.Pop(out int key, out string value))
            Debug.LogFormat("2nd {0}, {1}", key, value);
    }
}
