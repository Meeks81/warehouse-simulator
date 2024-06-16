using System.Collections;
using UnityEngine;

public class BuildingPackSystem : MonoBehaviour
{

    [SerializeField] private float _buildTime;
    [Space]
    [SerializeField] private BuildingPack _buildingPackPrefab;
    [SerializeField] private Builder _builder;
    [SerializeField] private Map _map;

    private void Start()
    {
        _builder.OnBuildingPlaced.AddListener(OnBuildingPlaced);
    }

    private void OnBuildingPlaced(BuildingController building, Vector3 position, Vector3 rotate)
    {
        BuildingPack pack = Instantiate(_buildingPackPrefab);
        pack.SetBuilding(_map, building, position, rotate);

        StartCoroutine(SetBuildTimer(pack));
    }

    private IEnumerator SetBuildTimer(BuildingPack pack)
    {
        yield return new WaitForSeconds(_buildTime);
        pack.MoveBuilding();
    }

}
