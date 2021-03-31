using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PathingMap
{
    public struct Neighbor
    {
        public Vector2Int position;
        public float cost;
        public Neighbor(Vector2Int position, float cost)
        {
            this.position = position;
            this.cost = cost;
        }
    }

    [HideInInspector]
    public PathingNode[] map;
    public Vector2Int dimensions;
    public Vector3 mapOrigin;
    public Vector3 mapNodeSize;
    public Vector3 sink;

    public PathingMap(Vector2Int dimensions, Vector3 mapOrigin, Vector3 mapNodeSize, Vector3 sink)
    {
        map = new PathingNode[dimensions.x * dimensions.y];
        for (int i = 0; i < map.Length; ++i)
            map[i].isWalkable = true;
        this.dimensions = dimensions;
        this.mapOrigin = mapOrigin;
        this.mapNodeSize = mapNodeSize;
        this.sink = sink;
    }

    public void CheckDimensions()
    {
        if (map.Length != dimensions.x * dimensions.y)
        {
            if (dimensions.x < 1)
                dimensions.x = 1;
            if (dimensions.y < 1)
                dimensions.y = 1;
            map = new PathingNode[dimensions.x * dimensions.y];
            for (int i = 0; i < map.Length; ++i)
                map[i].isWalkable = true;
        }
    }

    public int GetMapIndex(Vector2Int position)
    {
        return position.x + position.y * dimensions.x;
    }

    public bool InRange(Vector2Int position)
    {
        return 0 <= position.x && position.x < dimensions.x &&
            0 <= position.y && position.y < dimensions.y;
    }

    public void UpdateInstructions()
    {
        Pathing.Solve(this, WorldToMap(sink), null);
    }

    /// <summary>
    /// Converts world position to map position.
    /// </summary>
    /// <returns>Map position in integer form.</returns>
    public Vector2Int WorldToMap(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt((worldPosition.x - mapOrigin.x) / mapNodeSize.x),
            Mathf.FloorToInt((worldPosition.z - mapOrigin.z) / mapNodeSize.z));
    }

    /// <summary>
    /// Converts map position to world position.
    /// </summary>
    /// <returns>World position in float form.</returns>
    public Vector3 MapToWorld(Vector2Int mapPosition)
    {
        return mapOrigin + new Vector3(
            0.5f + mapPosition.x * mapNodeSize.x,
            mapOrigin.y,
            0.5f + mapPosition.y * mapNodeSize.z);
    }

    public IEnumerable<Neighbor> GetNeighbors(Vector2Int position)
    {
        const float squareRoot2 = 1.41421356237f;
        // how much diagonal neighbors affect the diagonal penalties
        const float diagonalNeighborWeight = 0.2f;
        // how much of self's penalty affect the diagonal penalties
        const float diagonalSelfWeight = 1 - 2 * diagonalNeighborWeight;

        float diagonalCost, straightCost;
        {
            int index = GetMapIndex(position);
            diagonalCost = diagonalSelfWeight * squareRoot2 * (1.0f + map[index].penalty);
            straightCost = 1.0f + map[index].penalty;
        }

        // assume position is in range
        Vector2Int neighbor = new Vector2Int();

        neighbor.x = position.x + 1;
        if (neighbor.x < dimensions.x)
        {
            neighbor.y = position.y + 1;
            int neighborIndex1 = GetMapIndex(new Vector2Int(neighbor.x, position.y));
            int neighborIndex2 = GetMapIndex(new Vector2Int(position.x, neighbor.y));
            float neighborCost1 = diagonalNeighborWeight * squareRoot2 * (1.0f + map[neighborIndex1].penalty);
            if (neighbor.y < dimensions.y &&
                map[neighborIndex1].isWalkable &&
                map[neighborIndex2].isWalkable)
                yield return new Neighbor(neighbor,
                    diagonalCost +
                    neighborCost1 + 
                    diagonalNeighborWeight * squareRoot2 * (1.0f + map[neighborIndex2].penalty));

            neighbor.y = position.y - 1;
            neighborIndex2 = GetMapIndex(new Vector2Int(position.x, neighbor.y));
            if (0 <= neighbor.y &&
                map[neighborIndex1].isWalkable &&
                map[neighborIndex2].isWalkable)
                yield return new Neighbor(neighbor,
                    diagonalCost +
                    neighborCost1 +
                    diagonalNeighborWeight * squareRoot2 * (1.0f + map[neighborIndex2].penalty));

            neighbor.y = position.y;
            yield return new Neighbor(neighbor, straightCost);
        }

        neighbor.x = position.x - 1;
        if (0 <= neighbor.x)
        {
            neighbor.y = position.y + 1;
            int neighborIndex1 = GetMapIndex(new Vector2Int(neighbor.x, position.y));
            int neighborIndex2 = GetMapIndex(new Vector2Int(position.x, neighbor.y));
            float neighborCost1 = diagonalNeighborWeight * squareRoot2 * (1.0f + map[neighborIndex1].penalty);
            if (neighbor.y < dimensions.y &&
                map[neighborIndex1].isWalkable &&
                map[neighborIndex2].isWalkable)
                yield return new Neighbor(neighbor,
                    diagonalCost +
                    neighborCost1 +
                    diagonalNeighborWeight * squareRoot2 * (1.0f + map[neighborIndex2].penalty));

            neighbor.y = position.y - 1;
            neighborIndex2 = GetMapIndex(new Vector2Int(position.x, neighbor.y));
            if (0 <= neighbor.y &&
                map[neighborIndex1].isWalkable &&
                map[neighborIndex2].isWalkable)
                yield return new Neighbor(neighbor,
                    diagonalCost +
                    neighborCost1 +
                    diagonalNeighborWeight * squareRoot2 * (1.0f + map[neighborIndex2].penalty));

            neighbor.y = position.y;
            yield return new Neighbor(neighbor, straightCost);
        }

        neighbor.x = position.x;
        neighbor.y = position.y + 1;
        if (neighbor.y < dimensions.y)
            yield return new Neighbor(neighbor, straightCost);

        neighbor.y = position.y - 1;
        if (0 <= neighbor.y)
            yield return new Neighbor(neighbor, straightCost);
    }
}
