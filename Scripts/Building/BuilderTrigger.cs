using System.Collections.Generic;
using UnityEngine;

public class BuilderTrigger : MonoBehaviour
{

    [SerializeField] private Map _map;
    [SerializeField] private Builder _builder;
    [SerializeField] private BuildingPermit _buildingPermit;

    private Dictionary<Vector3Int, BuildingController> _cells;

    private List<BuildingController> _tranparencyBuildings;

    private void Start()
    {
        UpdateCells();

        _map.OnAddBuilding.AddListener(AddObject);
        _map.OnDeleteBuilding.AddListener(RemoveObject);
        _builder.OnFlyingBuildingMoved.AddListener(OnFlyingBuildingMoved);
    }

    public BuildingController GetBuilding(int x, int y, int z = 0) => GetBuilding(new Vector3Int(x, y, z));
    public BuildingController GetBuilding(Vector3Int cell)
    {
        if (_cells.ContainsKey(cell))
            return _cells[cell];
        else
            return null;
    }

    private void OnFlyingBuildingMoved(BuildingController building)
    {
        ShowIntersectingObjects();
    }

    private void ShowIntersectingObjects()
    {
        List<Vector3Int> unavailableCells = new List<Vector3Int>();
        foreach (var item in _builder.FlyingBuilding.GetCurrentAreaCells())
        {
            if (_buildingPermit.IsCellAvailble(item) == false)
                unavailableCells.Add(item);
        }

        ClearTransparencyBuildings();

        _tranparencyBuildings = new List<BuildingController>();
        foreach (var item in unavailableCells)
        {
            BuildingController building = GetBuilding(item);
            if (building == null || _tranparencyBuildings.Contains(building))
                continue;

            _tranparencyBuildings.Add(building);
        }

        foreach (var item in _tranparencyBuildings)
        {
            item.transparentBuilding.SetTransparent(Color.red, 0.4f);
        }
    }

    private void ClearTransparencyBuildings()
    {
        if (_tranparencyBuildings == null)
            return;

        foreach (var item in _tranparencyBuildings)
        {
            item.transparentBuilding.SetNormal();
        }
    }

    private void UpdateCells()
    {
        _cells = new Dictionary<Vector3Int, BuildingController>();

        foreach (var buildingObject in _map.BuildObjects)
        {
            foreach (var cell in buildingObject.GetCurrentAreaCells())
            {
                _cells.Add(cell, buildingObject);
            }
        }
    }

    private void AddObject(BuildingController buildingObject)
    {
        foreach (var cell in buildingObject.GetCurrentAreaCells())
        {
            if (_cells.ContainsKey(cell))
                continue;

            _cells.Add(cell, buildingObject);
        }
        ClearTransparencyBuildings();
    }

    private void RemoveObject(BuildingController buildingObject)
    {
        foreach (var cell in buildingObject.GetAreaCells(buildingObject.LastPosition, buildingObject.LastRotation))
        {
            if (_cells.ContainsKey(cell) == false || _cells[cell] != buildingObject)
                continue;

            _cells.Remove(cell);
        }
        ClearTransparencyBuildings();
    }

}
