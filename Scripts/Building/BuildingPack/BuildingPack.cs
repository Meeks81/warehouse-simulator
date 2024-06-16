using System.Collections.Generic;
using UnityEngine;

public class BuildingPack : MonoBehaviour
{

    [SerializeField] private GhostBuilding _ghostBuildingPrefab;
    [SerializeField] private Transform _buildingParent;
    [SerializeField] private List<Animator> _animators;

    public Vector3 NewPosition { get; private set; }
    public Vector3 NewRotate { get; private set; }

    public BuildingController Building { get; private set; }

    private GhostBuilding _currentPosGhost;
    private GhostBuilding _newPosGhost;
    private Map _map;

    public void SetBuilding(Map map, BuildingController building, Vector3 newPosition, Vector3 newRotate)
    {
        _map = map;
        Building = building;
        NewPosition = newPosition;
        NewRotate = newRotate;

        if (map.BuildObjects.Contains(building))
        {
            _currentPosGhost = SpawnGhostBuilding(building.transform.position, building.transform.eulerAngles);
            transform.position = building.LastPosition;
            transform.rotation = building.LastRotation;
            building.transform.position = building.LastPosition;
            building.transform.rotation = building.LastRotation;
        }
        else
        {
            transform.position = building.transform.position;
            transform.rotation = building.transform.rotation;
        }
        _newPosGhost = SpawnGhostBuilding(newPosition, newRotate);

        _map.DeleteBuilding(Building, false);
        Building.transform.SetParent(_buildingParent);

        foreach (var item in _animators)
            item.SetTrigger("Pack");
    }

    public void MoveBuilding()
    {
        Building.transform.position = NewPosition;
        Building.transform.eulerAngles = NewRotate;

        transform.position = NewPosition;
        transform.eulerAngles = NewRotate;

        Building.Place();

        _map.AddBuilding(Building);

        _map.DeleteBuilding(_currentPosGhost);
        _map.DeleteBuilding(_newPosGhost);

        foreach (var item in _animators)
            item.SetTrigger("Place");
    }

    public void FinishBuildingAnimation()
    {
        Building.transform.SetParent(null);
        Building.transform.localScale = Vector3.one;

        Destroy(gameObject);
    }

    private GhostBuilding SpawnGhostBuilding(Vector3 position, Vector3 rotate)
    {
        GhostBuilding ghost = Instantiate(_ghostBuildingPrefab);
        ghost.Initilize(Building, Building.transform.position, Building.transform.rotation);
        _map.AddBuilding(ghost);

        return ghost;
    }

}
