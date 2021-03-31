using System.Collections.Generic;
using UnityEngine;

public static class Pathing
{
    public class StopControl
    {
        public bool stop;
    }

    private struct InternalNode
    {
        public Vector2Int cameFrom;
        public float fromStartCost;
        public float totalCost;
    }

    private struct Neighbor
    {
        public Vector2Int position;
        public float moveCost;
        public Neighbor(Vector2Int position, float moveCost)
        {
            this.position = position;
            this.moveCost = moveCost;
        }
    }

    private static readonly StopControl noStop = new StopControl();

    /// <summary>
    /// USE THIS! Using continuous floating grid, attempts to solve the pathfinding problem.
    /// </summary>
    /// <param name="map">Indices are defined as: map[y,x]</param>
    /// <param name="mapOrigin">Map's (0,0) origin in world-space</param>
    /// <param name="mapNodeSize">How big is each node? y value is ignored.</param>
    /// <param name="start">Start position</param>
    /// <param name="end">End position</param>
    /// <param name="path">Found path from start to end. Or path to the closest breakable node. Otherwise null</param>
    /// <param name="quitControl">Optional parameter that can quit the computation prematurely.</param>
    /// <returns>true if path is found, otherwise false</returns>
    /// <returns></returns>
    public static bool Solve(
        PathingNode[,] map, Vector3 mapOrigin, Vector3 mapNodeSize,
        Vector3 start, Vector3 end,
        out List<Vector3> path,
        StopControl quitControl = null)
    {
        Vector2Int integerStart = new Vector2Int(
            Mathf.FloorToInt((start.x - mapOrigin.x) / mapNodeSize.x),
            Mathf.FloorToInt((start.z - mapOrigin.z) / mapNodeSize.z));
        Vector2Int integerEnd = new Vector2Int(
            Mathf.FloorToInt((end.x - mapOrigin.x) / mapNodeSize.x),
            Mathf.FloorToInt((end.z - mapOrigin.z) / mapNodeSize.z));

        bool returnValue = Solve(map,
            integerStart, integerEnd,
            out List<Vector2Int> integerPath,
            quitControl ?? noStop);

        if (integerPath != null)
        {
            path = new List<Vector3>(integerPath.Count);
            for (int i = 0; i < integerPath.Count; ++i)
                path.Add(new Vector3(
                    (integerPath[i].x + 0.5f) * mapNodeSize.x + mapOrigin.x,
                    start.y,
                    (integerPath[i].y + 0.5f) * mapNodeSize.z + mapOrigin.z));

            return returnValue;
        }
        else
        {
            path = null;
            return returnValue;
        }        
    }

    /// <summary>
    /// Using discrete integer grid, attempts to solve the pathfinding problem.
    /// </summary>
    /// <param name="map">Indices are defined as: map[y,x]</param>
    /// <param name="start">Start position</param>
    /// <param name="end">End position</param>
    /// <param name="path">Found path from start to end. Or path to the closest breakable node. Otherwise null</param>
    /// <returns>true if path from start to end is found, otherwise false</returns>
    public static bool Solve(
        PathingNode[,] map, 
        Vector2Int start, Vector2Int end,
        out List<Vector2Int> path,
        StopControl quitControl)
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
        internalMap[start.y, start.x].totalCost = Heuristic(start, end);

        BinaryTree<float, Vector2Int> frontier = new BinaryTree<float, Vector2Int>();
        frontier.Initialise();
        frontier.Add(0, start);
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>() { start };        

        bool foundPath = false;

        // best breakable position closest to the end.
        Vector2Int bestBreakable = start;

        // find the node in frontier that has the lowest heuristic!
        while (frontier.Pop(out _, out Vector2Int current))
        {
            float currentFromStart = internalMap[current.y, current.x].fromStartCost;

            foreach (Neighbor neighbor in GetNeighbors(map, current))
            {
                float neighborFromStart = currentFromStart + neighbor.moveCost;

                if (map[neighbor.position.y, neighbor.position.x].isBreakable)
                {
                    float breakableCost = neighborFromStart + Heuristic(neighbor.position, end);
                    if(breakableCost < internalMap[bestBreakable.y, bestBreakable.x].totalCost)
                    {
                        bestBreakable = neighbor.position;
                        internalMap[neighbor.position.y, neighbor.position.x].cameFrom = current;
                        internalMap[neighbor.position.y, neighbor.position.x].totalCost = breakableCost;
                    }
                }

                if (map[neighbor.position.y, neighbor.position.x].isWalkable == false)
                    continue;

                if (visited.Contains(neighbor.position) == false ||
                    neighborFromStart < internalMap[neighbor.position.y, neighbor.position.x].fromStartCost)
                {
                    internalMap[neighbor.position.y, neighbor.position.x].cameFrom = current;
                    internalMap[neighbor.position.y, neighbor.position.x].fromStartCost = neighborFromStart;
                    internalMap[neighbor.position.y, neighbor.position.x].totalCost =
                        internalMap[neighbor.position.y, neighbor.position.x].fromStartCost +
                        Heuristic(neighbor.position, end);

                    frontier.Add(internalMap[neighbor.position.y, neighbor.position.x].totalCost, neighbor.position);
                    visited.Add(neighbor.position);

                    if (neighbor.position == end)
                    {
                        foundPath = true;
                        break;
                    }
                }
            }

            if (quitControl.stop)
            {
                path = null;
                return false;
            }

            if (foundPath)
                break;
        }

        Vector2Int selectEnd;
        if (foundPath)
            selectEnd = end;
        else if(map[bestBreakable.y, bestBreakable.x].isBreakable)
            selectEnd = bestBreakable;
        else
        {
            path = null;
            return false;
        }

        path = new List<Vector2Int>();
        // reconstruct path
        Vector2Int appendPosition = selectEnd;
        while (appendPosition != start)
        {
            path.Add(appendPosition);
            appendPosition = internalMap[appendPosition.y, appendPosition.x].cameFrom;
        }
        path.Reverse();

        return foundPath;
    }

    private static IEnumerable<Neighbor> GetNeighbors(PathingNode[,] map, Vector2Int position)
    {
        const float squareRoot2 = 1.41421356237f;

        float diagonalCost = squareRoot2 * (1.0f - map[position.y, position.x].penalty);
        float straightCost = 1.0f - map[position.y, position.x].penalty;        

        // assume position is in range
        int x, y;

        x = position.x + 1;        
        if (x < map.GetLength(1))
        {
            y = position.y + 1;
            if (y < map.GetLength(0) && 
                map[y, position.x].isWalkable && 
                map[position.y, x].isWalkable)
                yield return new Neighbor(new Vector2Int(x, y), diagonalCost);

            y = position.y - 1;
            if (0 <= y &&
                map[y, position.x].isWalkable &&
                map[position.y, x].isWalkable)
                yield return new Neighbor(new Vector2Int(x, y), diagonalCost);

            y = position.y;
            yield return new Neighbor(new Vector2Int(x, y), straightCost);
        }

        x = position.x - 1;
        if (0 <= x)
        {
            y = position.y + 1;
            if (y < map.GetLength(0) &&
                map[y, position.x].isWalkable &&
                map[position.y, x].isWalkable)
                yield return new Neighbor(new Vector2Int(x, y), diagonalCost);

            y = position.y - 1;
            if (0 <= y &&
                map[y, position.x].isWalkable &&
                map[position.y, x].isWalkable)
                yield return new Neighbor(new Vector2Int(x, y), diagonalCost);

            y = position.y;
            yield return new Neighbor(new Vector2Int(x, y), straightCost);
        }

        x = position.x;
        y = position.y + 1;
        if (y < map.GetLength(0))
            yield return new Neighbor(new Vector2Int(x, y), straightCost);

        y = position.y - 1;
        if (0 <= y)
            yield return new Neighbor(new Vector2Int(x, y), straightCost);
    }

    private static float Heuristic(Vector2Int position, Vector2Int end)
    {
        return (position - end).magnitude;
    }
}
