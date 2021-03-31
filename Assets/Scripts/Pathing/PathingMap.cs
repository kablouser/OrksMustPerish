using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingMap : MonoBehaviour
{
    [System.Serializable]
    private struct PathingNodeArray
    {
        public PathingNode[] array;
        public PathingNodeArray(int length)
        {
            array = new PathingNode[length];
        }
    }

    [SerializeField]
    private Vector2Int mapDimensions;
    [SerializeField]
    private PathingJob settings;
    [SerializeField]
    private PathingNodeArray[] editorMap;

    private void Start()
    {
        settings.map = new PathingNode[mapDimensions.y, mapDimensions.x];
        for (int y = 0; y < mapDimensions.y; ++y)
            for (int x = 0; x < mapDimensions.x; ++x)
                settings.map[y, x] = editorMap[y].array[x];
        settings.WaitForStart();
        editorMap = null;
    }

    private void OnDrawGizmos()
    {
        if (mapDimensions[0] <= 0)
            mapDimensions[0] = 1;
        if (mapDimensions[1] <= 0)
            mapDimensions[1] = 1;

        if (editorMap.Length != mapDimensions[1] ||
            editorMap.Length == 0 ||
            editorMap[0].array.Length != mapDimensions[0])
        {
            editorMap = new PathingNodeArray[mapDimensions[1]];
            for (int j = 0; j < mapDimensions[1]; ++j)
            {
                editorMap[j] = new PathingNodeArray(mapDimensions[0]);
                for (int i = 0; i < mapDimensions[0]; ++i)
                    editorMap[j].array[i].isWalkable = true;
            }
        }

        Gizmos.color = Color.red;
        for (int y = 0; y < mapDimensions[1]; ++y)
            for (int x = 0; x < mapDimensions[0]; ++x)
                if (editorMap[y].array[x].isWalkable)
                    Gizmos.DrawWireCube(
                        settings.mapOrigin + new Vector3(0.5f, 0, 0.5f) +
                        new Vector3(x * settings.mapNodeSize.x, 0.25f, y * settings.mapNodeSize.z),
                        new Vector3(settings.mapNodeSize.x, 0.5f, settings.mapNodeSize.z));

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(settings.start, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(settings.end, 0.5f);
    }

    private bool UpdateNodeChecked(Vector3 worldPosition, PathingNode node)
    {
        int x = Mathf.FloorToInt((worldPosition.x - settings.mapOrigin.x) / settings.mapNodeSize.x);
        int y = Mathf.FloorToInt((worldPosition.z - settings.mapOrigin.z) / settings.mapNodeSize.z);
        if (0 <= y && y < settings.map.GetLength(0) &&
            0 <= x && x < settings.map.GetLength(1))
        {
            settings.map[y, x] = node;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Attempts to update a node in the map. Fails if outside of the map boundaries.
    /// </summary>
    /// <returns>true if updated, false if out of range</returns>
    public bool UpdateNode(Vector3 worldPosition, PathingNode node)
    {
        if (UpdateNodeChecked(worldPosition, node))
        {
            settings.WaitForStart();
            return true;
        }
        else
            return false;
    }

    public bool UpdateNodes(IEnumerable<Vector3> worldPositions, IEnumerable<PathingNode> nodes)
    {
        var positionEnumerator = worldPositions.GetEnumerator();
        var nodeEnumerator = nodes.GetEnumerator();
        int validUpdates = 0;
        while (positionEnumerator.MoveNext() && nodeEnumerator.MoveNext())
        {
            if (UpdateNodeChecked(positionEnumerator.Current, nodeEnumerator.Current))
                ++validUpdates;
        }

        if (0 < validUpdates)
        {
            settings.WaitForStart();
            return true;
        }
        else
            return false;
    }

    public bool GetPath(out List<Vector3> path)
    {
        return settings.GetResults(out path);
    }
}
