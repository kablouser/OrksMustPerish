using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathingMapManager : MonoBehaviour
{
    public PathingMap pathingMap;

    [Header("This controls the origin of the strip")]
    public Vector2Int stripOrigin;
    [Range(0, 20)]
    public int stripLength = 10;
    [Header("true to change map, false to read")]
    public bool editStrip = false;
    [Header("A small \"strip\" of the full map")]
    public PathingNode[] strip;

    [ContextMenu("Start")]
    private void Start()
    {
        pathingMap.UpdateInstructions();
    }

    private void OnValidate()
    {
        pathingMap.CheckDimensions();

        if (stripLength != strip.Length)
            strip = new PathingNode[stripLength];

        if (pathingMap.InRange(stripOrigin) == false)
            stripOrigin = new Vector2Int(
                Mathf.Clamp(stripOrigin.x, 0, pathingMap.dimensions.x - 1),
                Mathf.Clamp(stripOrigin.y, 0, pathingMap.dimensions.y - 1));

        for (int stripI = 0, mapI = pathingMap.GetMapIndex(stripOrigin);
            stripI < stripLength && mapI < pathingMap.map.Length;
            ++stripI, ++mapI)
        {
            if (editStrip)
            {
                if (strip[stripI].penalty < -2)
                {
                    Debug.LogError("Stop being naughty! Penalty cannot be less than -2! (causes infinite loop)");
                    strip[stripI].penalty = 0;
                }
                pathingMap.map[mapI] = strip[stripI];
            }
            else
                strip[stripI] = pathingMap.map[mapI];
        }
    }

    private void OnDrawGizmos()
    {
        if (pathingMap.dimensions.x < 1 || pathingMap.dimensions.y < 1)
            return;

        Gizmos.color = Color.red;

        int stripBegin = pathingMap.GetMapIndex(stripOrigin);
        int stripEnd = stripBegin + stripLength;
        int i = 0;
        for (int y = 0; y < pathingMap.dimensions.y; ++y)
            for (int x = 0; x < pathingMap.dimensions.x; ++x)
                if (pathingMap.map[pathingMap.GetMapIndex(new Vector2Int(x, y))].isWalkable)
                {
                    Vector3 position = pathingMap.MapToWorld(new Vector2Int(x, y));
                    position.y += 0.25f;

                    if (stripBegin <= i && i < stripEnd)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(
                            position,
                            new Vector3(
                                pathingMap.mapNodeSize.x * 0.9f,
                                0.5f,
                                pathingMap.mapNodeSize.z * 0.9f));
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.DrawWireCube(
                            position,
                            new Vector3(
                                pathingMap.mapNodeSize.x * 0.9f,
                                0.5f,
                                pathingMap.mapNodeSize.z * 0.9f));
                    }
                                        
                    Vector3 towards = pathingMap.MapToWorld(pathingMap.map[pathingMap.GetMapIndex(new Vector2Int(x, y))].cameFrom);                    
                    Gizmos.DrawRay(position, (towards - position) * 0.5f);
                    ++i;
                }
    }
}
