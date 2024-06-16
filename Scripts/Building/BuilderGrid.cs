using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuilderGrid : MonoBehaviour
{

    [SerializeField] private Tilemap _placedBuildingsTilemap;
    [SerializeField] private Tilemap _flyingBuildingTilemap;
    [SerializeField] private TileBase _placedTile;
    [SerializeField] private TileBase _avaialbleTile;
    [SerializeField] private TileBase _unavailableTile;
    [Space]
    [SerializeField] private Builder _builder;
    [SerializeField] private Map _map;

    private void Start()
    {
        _builder.OnFlyingBuildingChanged.AddListener(OnFlyingBuildingChanged);
        _builder.OnBuildingEdited.AddListener(OnBuildingEdited);
        _builder.OnFlyingBuildingMoved.AddListener(OnFlyingBuildingMoved);
        _map.OnAddBuilding.AddListener((building) => UpdatePlacedTilemap());
        _map.OnDeleteBuilding.AddListener((building) => UpdatePlacedTilemap());
    }

    private void OnFlyingBuildingChanged(BuildingController building)
    {
        UpdatePlacedTilemap();
    }

    private void OnBuildingEdited(BuildingController building, Vector3 position, Vector3 rotate)
    {
        _flyingBuildingTilemap.ClearAllTiles();
        UpdatePlacedTilemap();
    }

    private void OnFlyingBuildingMoved(BuildingController building)
    {
        Vector3Int[] area = building.GetCurrentAreaCells();
        _flyingBuildingTilemap.ClearAllTiles();
        FillArea(area, _flyingBuildingTilemap, (x, y) => _placedBuildingsTilemap.GetTile(new Vector3Int(x, y)) == null ? _avaialbleTile : _unavailableTile);
    }

    private void UpdatePlacedTilemap()
    {
        _placedBuildingsTilemap.ClearAllTiles();
        foreach (var item in _map.BuildObjects)
        {
            if (item == _builder.FlyingBuilding)
                continue;

            FillArea(item.GetCurrentAreaCells(), _placedBuildingsTilemap, (x, y) => _placedTile);
        }
    }

    private void FillArea(Vector3Int[] area, Tilemap tilemap, Func<int, int, TileBase> tile)
    {
        foreach (var item in area)
        {
            tilemap.SetTile(item, tile(item.x, item.y));
        }
    }

}
