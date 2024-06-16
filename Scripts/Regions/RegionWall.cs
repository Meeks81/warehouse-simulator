using UnityEngine;

public class RegionWall : MonoBehaviour
{

    [SerializeField] private ObjectPool<Transform> _wallsPool;
    [Space]
    [SerializeField] private RegionsSystem _regionsSystem;

    public void SpawnWallsAroundRegion()
    {
        _wallsPool.HideEverything();
        foreach (var item in _regionsSystem.GetCurrentAreas())
        {
            SpawnArea(item);
        }
    }

    public void SpawnArea(Vector2Int area)
    {
        if (_regionsSystem.CheckSide(area, Vector2Int.up) == false)
            SpawnWall((area.x - 1) * _regionsSystem.areaSize.x, area.x * _regionsSystem.areaSize.x, area.y * _regionsSystem.areaSize.y, true);
        if (_regionsSystem.CheckSide(area, Vector2Int.down) == false)
            SpawnWall((area.x - 1) * _regionsSystem.areaSize.x, area.x * _regionsSystem.areaSize.x, (area.y - 1) * _regionsSystem.areaSize.y - 1, true);

        if (_regionsSystem.CheckSide(area, Vector2Int.right) == false)
            SpawnWall((area.y - 1) * _regionsSystem.areaSize.y, area.y * _regionsSystem.areaSize.y, area.x * _regionsSystem.areaSize.x, false);
        if (_regionsSystem.CheckSide(area, Vector2Int.left) == false)
            SpawnWall((area.y - 1) * _regionsSystem.areaSize.y, area.y * _regionsSystem.areaSize.y, (area.x - 1) * _regionsSystem.areaSize.x - 1, false);
    }

    private void SpawnWall(int start, int end, int anotherAxis, bool axisX)
    {
        for (int i = start; i < end; i = (int)Mathf.MoveTowards(i, end, 1))
        {
            Transform wall = _wallsPool.Get();
            wall.position = MapGrid.current.grid.GetCellCenterWorld(new Vector3Int(axisX ? i : anotherAxis, axisX ? anotherAxis : i));
            wall.gameObject.SetActive(true);
        }
    }

}
