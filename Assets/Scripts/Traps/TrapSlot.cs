using UnityEngine;

/// <summary>
/// Place on special trap slot prefabs, the trap grid will take care of the rest
/// </summary>
public class TrapSlot : MonoBehaviour
{
    public TrapGrid trapGrid;
    GenericTrap placedTrap;

    private void Start()
    {
        if (trapGrid == null)
        {
            trapGrid = FindObjectOfType<TrapGrid>();
            Debug.LogWarning("Trap slot doesn't have reference to trap grid. Finding now ...");
        }
    }

    public GenericTrap GetPlacedTrap()
    {
        return placedTrap;
    }

    public bool IsTrapPlaced()
    {
        return placedTrap != null;
    }

    /// <summary>
    /// Doesn't check cost. Only checks if a trap exists.
    /// </summary>
    public GenericTrap CopyAndPlace(GenericTrap originalTrap)
    {
        if (IsTrapPlaced())
        {
            Debug.LogError("A trap already exists on this slot!");
            return null;
        }
        placedTrap = Instantiate(originalTrap, transform.position, Quaternion.identity, transform);
        trapGrid.UpdateGrid(this);
        return placedTrap;
    }

    public void StartDeleteTrap()
    {
        if(IsTrapPlaced())
            placedTrap.StartDelete();
    }

    public void EndDeleteTrap(bool confirm,
        BuildingResourceManager buildingResourceManager,
        float refundAmount)
    {
        if (IsTrapPlaced())
        {          
            placedTrap.EndDelete(confirm, buildingResourceManager, refundAmount);

            if (confirm)
            {
                placedTrap = null;
                trapGrid.UpdateGrid(this);
            }
        }
    }
}
