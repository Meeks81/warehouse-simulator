using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegionsSystem : MonoBehaviour
{

    [SerializeField] private Vector2Int _areaSize;
    [SerializeField] private Vector2Int _startAreas;
    [SerializeField] private Vector2Int _maxAreas;
    [Space]
    [SerializeField] private RegionWall _regionWall;

    public Vector2Int areaSize => _areaSize;
    public Vector2Int maxAreas => _maxAreas;

    private Dictionary<Vector2Int, Vector2Int> _currentAreas = new Dictionary<Vector2Int, Vector2Int>();

    private void Start()
    {
        SpawnStartAreas();
        _regionWall.SpawnWallsAroundRegion();
    }

    public List<Vector2Int> GetCurrentAreas() => _currentAreas.Keys.ToList();

    public List<Vector2Int> GetAvailableAreasForOpen()
    {
        List<Vector2Int> availableAreas = new List<Vector2Int>();
        for (int x = 0; x < _maxAreas.x; x++)
        {
            for (int y = 0; y < _maxAreas.y; y++)
            {
                Vector2Int area = new Vector2Int(x, y);
                if (_currentAreas.ContainsKey(area))
                    continue;

                if (CheckSide(area, Vector2Int.up) ||
                   CheckSide(area, Vector2Int.down) ||
                   CheckSide(area, Vector2Int.left) ||
                   CheckSide(area, Vector2Int.right))
                {
                    availableAreas.Add(area);
                }
            }
        }

        return availableAreas;
    }

    public void OpenArea(Vector2Int area)
    {
        if (_currentAreas.ContainsKey(area) ||
            area.x >= _maxAreas.x ||
            area.y >= _maxAreas.y)
            return;

        _currentAreas.Add(area, area);
        _regionWall.SpawnWallsAroundRegion();
    }

    public bool CheckSide(Vector2Int area, Vector2Int offset) => _currentAreas.ContainsKey(area + offset);

    private void SpawnStartAreas()
    {
        _currentAreas = new Dictionary<Vector2Int, Vector2Int>();
        for (int x = 0; x < _startAreas.x; x++)
        {
            for (int y = 0; y < _startAreas.y; y++)
            {
                Vector2Int area = new Vector2Int(x, y);
                _currentAreas.Add(area, area);
            }
        }
    }

}
