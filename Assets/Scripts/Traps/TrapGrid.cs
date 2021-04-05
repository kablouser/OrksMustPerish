using UnityEngine;

public class TrapGrid : MonoBehaviour
{
    [Header("Go to pathing map manager. Set its dimensions equal to the size of the trap slots!")]
    public GameObject trapSlotPrefab;
    public PathingMapManager pathingMapManager;

    [SerializeField]
    [HideInInspector]
    private TrapSlot[] grid;    

    public TrapSlot GetSlot(Vector3 worldPosition, out Vector2Int gridPosition)
    {
        gridPosition = pathingMapManager.pathingMap.WorldToMap(worldPosition);
        if(pathingMapManager.pathingMap.InRange(gridPosition))
        {
            int mapIndex = pathingMapManager.pathingMap.GetMapIndex(gridPosition);
            TrapSlot trapSlot = grid[pathingMapManager.pathingMap.GetMapIndex(gridPosition)];
            if (grid[mapIndex] != null)
                return trapSlot;
        }
        return null;
    }

    public void UpdateGrid(TrapSlot trapSlot)
    {
        GenericTrap trap = trapSlot.GetPlacedTrap();
        float newPenalty;
        if (trap == null)
            newPenalty = 0;
        else
            newPenalty = trap.pathingPenalty;

        int mapIndex = pathingMapManager.pathingMap.GetMapIndex(pathingMapManager.pathingMap.WorldToMap(trapSlot.transform.position));
        float currentPenalty = pathingMapManager.pathingMap.map[mapIndex].penalty;
        if(currentPenalty != newPenalty)
        {
            pathingMapManager.pathingMap.map[mapIndex].penalty = newPenalty;
            // this is slow
            pathingMapManager.pathingMap.UpdateInstructions();
        }
    }

    [ContextMenu("Recreate Grid")]
    private void RecreateGrid()
    {
        if (grid != null)
            foreach (var x in grid)
                if (x != null)
                    DestroyImmediate(x.gameObject);

        int dimensionX = pathingMapManager.pathingMap.dimensions.x;
        grid = new TrapSlot[pathingMapManager.pathingMap.dimensions.x * pathingMapManager.pathingMap.dimensions.y];
        for(int i = 0; i < grid.Length; ++i)
        {
            Vector2Int gridPosition = new Vector2Int(i % dimensionX, i / dimensionX);
            grid[i] = Instantiate(trapSlotPrefab, transform).GetComponent<TrapSlot>();
            grid[i].transform.position = pathingMapManager.pathingMap.MapToWorld(gridPosition);
            grid[i].trapGrid = this;
        }
    }

    [ContextMenu("Change Pathing to Trap Grid's Layout")]
    private void UpdatePathingMap()
    {
        for (int i = 0; i < grid.Length; ++i)
            pathingMapManager.pathingMap.map[i].isWalkable = grid[i] != null;
    }
}
