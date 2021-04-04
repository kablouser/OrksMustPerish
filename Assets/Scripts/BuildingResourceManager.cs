using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingResourceManager : MonoBehaviour
{
    private int buildingResource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
