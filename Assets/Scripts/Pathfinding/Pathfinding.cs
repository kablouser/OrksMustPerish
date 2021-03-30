using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    private struct InternalNode
    {
        public Vector2Int cameFrom;
        public int fromStartCost;
        public float heuristicCost;
        public float totalCost;
    }

    /// <summary>
    /// USE THIS! Using continuous floating grid, attempts to solve the pathfinding problem.
    /// </summary>
    /// <param name="map">Indices are defined as: map[y,x]</param>
    /// <param name="mapOrigin">Map's (0,0) origin in world-space</param>
    /// <param name="mapNodeSize">How big is each node? y value is ignored.</param>
    /// <param name="start">Start position</param>
    /// <param name="end">End position</param>
    /// <param name="path">Found path from start to end. null if not found</param>
    /// <param name="breakablePath">Path to the closest breakable node. null if path is found</param>
    /// <returns>true if path is found, otherwise false</returns>
    /// <returns></returns>
    public static bool Solve(
        Node[,] map, Vector3 mapOrigin, Vector3 mapNodeSize,
        Vector3 start, Vector3 end,
        out List<Vector3> path, out List<Vector3> breakablePath)
    {
        Vector2Int integerStart = new Vector2Int(
            Mathf.FloorToInt((start.x - mapOrigin.x) / mapNodeSize.x),
            Mathf.FloorToInt((start.z - mapOrigin.z) / mapNodeSize.z));
        Vector2Int integerEnd = new Vector2Int(
            Mathf.FloorToInt((end.x - mapOrigin.x) / mapNodeSize.x),
            Mathf.FloorToInt((end.z - mapOrigin.z) / mapNodeSize.z));

        bool returnValue = Solve(map, integerStart, integerEnd, out List<Vector2Int> integerPath, out List<Vector2Int> integerBreakablePath);

        List<Vector3> selectPath;
        if(returnValue)
        {
            path = selectPath = new List<Vector3>(integerPath.Count);
            breakablePath = null;
        }
        else
        {
            path = null;
            breakablePath = selectPath = new List<Vector3>(integerPath.Count);
        }

        for(int i = 0; i < integerPath.Count; ++i)
            selectPath.Add(new Vector3(
                (integerPath[i].x + 0.5f) * mapNodeSize.x + mapOrigin.x,
                start.y,
                (integerPath[i].y + 0.5f) * mapNodeSize.z + mapOrigin.z));

        return returnValue;
    }

    /// <summary>
    /// Using discrete integer grid, attempts to solve the pathfinding problem.
    /// </summary>
    /// <param name="map">Indices are defined as: map[y,x]</param>
    /// <param name="start">Start position</param>
    /// <param name="end">End position</param>
    /// <param name="path">Found path from start to end. null if not found</param>
    /// <param name="breakablePath">Path to the closest breakable node. null if path is found</param>
    /// <returns>true if path is found, otherwise false</returns>
    public static bool Solve(
        Node[,] map, 
        Vector2Int start, Vector2Int end,
        out List<Vector2Int> path, out List<Vector2Int> breakablePath)
    {
        if( start.y < 0 || map.GetLength(0) <= start.y ||
            start.x < 0 || map.GetLength(1) <= start.x)
            throw new System.ArgumentException("argument out of range", "start");
        else if(
            end.y < 0 || map.GetLength(0) <= end.y ||
            end.x < 0 || map.GetLength(1) <= end.x )
            throw new System.ArgumentException("argument out of range", "end");

        InternalNode[,] internalMap = new InternalNode[map.GetLength(0), map.GetLength(1)];        
        internalMap[start.y, start.x].fromStartCost = 0;

        List<Vector2Int> frontier = new List<Vector2Int> { start };
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>() { start };

        bool foundPath = false;

        // closest position to the end.
        Vector2Int bestPosition = start;
        float bestHeuristic = Heuristic(start, end);

        while (0 < frontier.Count)
        {
            // find the node in frontier that has the lowest heuristic!
            Vector2Int current = frontier[0];
            float currentTotalCost = internalMap[current.y, current.x].totalCost;
            for (int i = 1; i < frontier.Count; ++i)
            {
                Vector2Int position = frontier[i];
                if (internalMap[position.y, position.x].totalCost < currentTotalCost)
                {
                    current = position;
                    currentTotalCost = internalMap[current.y, current.x].totalCost;
                }
            }
            frontier.Remove(current);
            int currentFromStart = internalMap[current.y, current.x].fromStartCost;            

            foreach (Vector2Int neighbor in GetNeighbors(map, current))
            {
                int neighborFromStart = currentFromStart + 1;
                if (visited.Contains(neighbor) == false ||
                    neighborFromStart < internalMap[neighbor.y, neighbor.x].fromStartCost)
                {
                    internalMap[neighbor.y, neighbor.x].cameFrom = current;
                    internalMap[neighbor.y, neighbor.x].fromStartCost = neighborFromStart;
                    internalMap[neighbor.y, neighbor.x].heuristicCost = Heuristic(neighbor, end);
                    internalMap[neighbor.y, neighbor.x].totalCost =
                        internalMap[neighbor.y, neighbor.x].fromStartCost +
                        internalMap[neighbor.y, neighbor.x].heuristicCost;

                    frontier.Add(neighbor);
                    visited.Add(neighbor);

                    if (internalMap[neighbor.y, neighbor.x].heuristicCost < bestHeuristic)
                    {
                        bestPosition = neighbor;
                        bestHeuristic = internalMap[neighbor.y, neighbor.x].heuristicCost;
                    }

                    if (neighbor == end)
                    {
                        foundPath = true;
                        break;
                    }
                }
            }

            if (foundPath)
                break;
        }

        List<Vector2Int> selectOutput;
        Vector2Int selectEnd;
        if(foundPath)
        {
            path = selectOutput = new List<Vector2Int>();
            breakablePath = null;
            selectEnd = end;
        }
        else
        {
            path = null;
            breakablePath = selectOutput = new List<Vector2Int>();
            selectEnd = bestPosition;
        }

        // reconstruct path
        Vector2Int appendPosition = selectEnd;
        while (appendPosition != start)
        {
            selectOutput.Add(appendPosition);
            appendPosition = internalMap[appendPosition.y, appendPosition.x].cameFrom;
        }
        selectOutput.Reverse();

        if(foundPath == false)
        {
            float bestHeuristicBreakable = Mathf.Infinity;
            Vector2Int bestBreakable = new Vector2Int(-1, -1);
            foreach(Vector2Int endNeighbor in GetNeighbors(map, bestPosition))
            {
                if (map[endNeighbor.y, endNeighbor.x].isBreakable &&
                    internalMap[endNeighbor.y, endNeighbor.x].heuristicCost < bestHeuristicBreakable)
                {
                    bestHeuristicBreakable = internalMap[endNeighbor.y, endNeighbor.x].heuristicCost;
                    bestBreakable = endNeighbor;
                }
            }

            // could not find a breakable near the end (shouldn't happen)
            if (bestBreakable.x == -1)
                Debug.LogError("Path is blocked, but couldn't find a breakable! Is your path blocked regardless of breakables?");
            // else found a breakable near the end ...
            else
                path.Add(bestBreakable);
        }

        return foundPath;
    }

    private static IEnumerable<Vector2Int> GetNeighbors(Node[,] map, Vector2Int position)
    {
        // assume position is in range
        int x, y;
        
        x = position.x + 1;
        y = position.y;
        if (x < map.GetLength(1) && map[y, x].isWalkable)
            yield return new Vector2Int(x, y);

        x = position.x - 1;
        if (0 <= x && map[y, x].isWalkable)
            yield return new Vector2Int(x, y);

        x = position.x;
        y = position.y + 1;
        if (y < map.GetLength(0) && map[y, x].isWalkable)
            yield return new Vector2Int(x, y);

        y = position.y - 1;
        if (0 <= y && map[y, x].isWalkable)
            yield return new Vector2Int(x, y);
    }

    private static float Heuristic(Vector2Int position, Vector2Int end)
    {
        return (position - end).magnitude;
    }
}
