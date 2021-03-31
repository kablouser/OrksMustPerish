using UnityEngine;

[System.Serializable]
/// <summary>
/// Pathfinding data structure.
/// </summary>
public struct PathingNode
{    
    [Tooltip("To allow pathing to find nearest barricade, set the barricade's isWalkable to true (yes that's right) and its penalty to fairly high.")]
    public bool isWalkable;
    [Tooltip("+ value for slow-down, - value for speed-up (don't use less than -2!). 1 equivalent to walking across 1 node. 2 equivalent to walking across 2 nodes. etc...")]
    public float penalty;

    [HideInInspector]
    public float totalCost;
    [HideInInspector]
    public Vector2Int cameFrom;

    public PathingNode(bool isWalkable, float penalty, float totalCost, Vector2Int cameFrom)
    {
        this.isWalkable = isWalkable;
        this.penalty = penalty;
        this.totalCost = totalCost;
        this.cameFrom = cameFrom;
    }
}
