using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Map : MonoBehaviour
{

    [SerializeField] private List<BuildingController> _buildObjects = new List<BuildingController>();

    public List<BuildingController> BuildObjects => new List<BuildingController>(_buildObjects);

    public UnityEvent<BuildingController> OnAddBuilding;
    public UnityEvent<BuildingController> OnDeleteBuilding;

    public void AddBuilding(BuildingController building)
    {
        if (_buildObjects.Contains(building))
            return;

        _buildObjects.Add(building);

        OnAddBuilding.Invoke(building);
    }

    public void DeleteBuilding(BuildingController building, bool deleteObject = true)
    {
        if (_buildObjects.Contains(building) == false)
            return;

        _buildObjects.Remove(building);

        if (deleteObject)
            Destroy(building.gameObject);

        OnDeleteBuilding.Invoke(building);
    }

}
