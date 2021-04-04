using UnityEngine;

/// <summary>
/// Place on special trap slot prefabs, the trap grid will take care of the rest
/// </summary>
public class TrapSlot : MonoBehaviour
{
    GenericTrap placedTrap;

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
        return (placedTrap = Instantiate(originalTrap, transform.position, Quaternion.identity, transform));
    }

    public void StartDeleteTrap()
    {
        if(IsTrapPlaced())
            placedTrap.StartDelete();
    }

    public void EndDeleteTrap(bool confirm,
        BuildingResourceManager buildingResourceManager,
        float refundPrice)
    {
        if (IsTrapPlaced())
        {
            placedTrap.EndDelete(confirm, buildingResourceManager, refundPrice);
            if (confirm)
                placedTrap = null;
        }
    }
}
