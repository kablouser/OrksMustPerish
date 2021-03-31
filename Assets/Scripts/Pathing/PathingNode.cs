[System.Serializable]
/// <summary>
/// Pathfinding data structure.
/// </summary>
public struct PathingNode
{
    public bool isWalkable;
    public bool isBreakable;
    /// <summary>
    /// Positive value for slow-down, negative value for speed-up.
    /// </summary>
    public float penalty;

    public PathingNode(bool isWalkable, bool isBreakable = false, float penalty = 0)
    {
        this.isWalkable = isWalkable;
        this.isBreakable = isBreakable;
        this.penalty = penalty;
    }
}
