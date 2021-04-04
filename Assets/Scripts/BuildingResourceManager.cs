using UnityEngine;

public class BuildingResourceManager : MonoBehaviour
{
    [SerializeField]
    private int buildingResource;

    public int GetBuildingResource()
    {
        return buildingResource;
    }

    public void AddBuildingResource(int amount)
    {
        buildingResource += amount;
    }

    //Will return true and take the amount if there is enough building resource.
    //Returns false if the amount is to high.
    public bool TakeBuildingResource(int amount)
    {
        if(buildingResource - amount >= 0)
        {
            buildingResource -= amount;
            return true;
        }
        return false;
    }
}
