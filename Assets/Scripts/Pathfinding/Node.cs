[System.Serializable]
/// <summary>
/// Pathfinding data structure.
/// </summary>
public struct Node
{
    public bool isWalkable;
    public bool isBreakable;
    /// <summary>
    /// Positive value for slow-down, negative value for speed-up.
    /// </summary>
    public int penalty;

    public Node(bool isWalkable, bool isBreakable = false, int penalty = 0)
    {
        this.isWalkable = isWalkable;
        this.isBreakable = isBreakable;
        this.penalty = penalty;
    }
}
