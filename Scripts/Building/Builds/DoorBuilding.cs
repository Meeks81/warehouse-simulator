using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorBuilding : BuildingController
{

    public const int MIN_SIZE = 4;
    public const int MAX_SIZE = 8;

    [SerializeField] private Transform _columnStart;
    [SerializeField] private Transform _columnEnd;
    [SerializeField] private ObjectPool<Transform> _topColumnPool;

    public override bool IsEdit => false;
    public override Vector3 LastPosition => transform.position;
    public override Quaternion LastRotation => transform.rotation;

    private List<Vector3Int> _areaCells = new List<Vector3Int>();

    private bool _isExpand = false;
    private Vector3Int _startExpandCell;
    private int _size;

    private UnityAction<BuildingController[]> _onPlace;
    private UnityAction _onCancel;

    public override void Init(UnityAction<BuildingController[]> onPlace, UnityAction onCancel)
    {
        _onPlace = onPlace;
        _onCancel = onCancel;

        _columnStart.gameObject.SetActive(false);
        _columnEnd.gameObject.SetActive(false);
    }

    public override Vector3 GetSize() => MapGrid.current.grid.cellSize;
    public override Vector3Int[] GetCurrentAreaCells() => _areaCells.ToArray();
    public override List<Vector3Int> GetAreaCells(Vector3 position, Quaternion rotation)
    {
        return new List<Vector3Int>();
    }

    public override void MouseDown(Vector3 position)
    {
        if (_isExpand)
        {
            if (_size >= MIN_SIZE)
                _onPlace(new BuildingController[] { this });
        }
        else
        {
            _isExpand = true;
            _startExpandCell = MapGrid.current.grid.WorldToCell(position);

            _columnStart.gameObject.SetActive(true);
            _columnEnd.gameObject.SetActive(true);
        }
    }

    public override void MouseDrag(Vector3 position)
    {

    }

    public override void MouseMove(Vector3 position)
    {
        Vector3Int cell = MapGrid.current.grid.WorldToCell(position);

        if (_isExpand)
        {
            transform.position = MapGrid.current.grid.GetCellCenterWorld(_startExpandCell);

            int startX = _startExpandCell.x;
            int startY = _startExpandCell.y;
            int endX = cell.x;
            int endY = cell.y;

            Vector2Int size = new Vector2Int(Mathf.Abs(endX - startX), Mathf.Abs(endY - startY));

            if (size.x > size.y)
            {
                if (size.x > MAX_SIZE)
                {
                    endX = (int)Mathf.MoveTowards(startX, endX, MAX_SIZE);
                    _size = MAX_SIZE;
                }
                else
                    _size = size.x;

                _columnEnd.position = MapGrid.current.grid.GetCellCenterWorld(new Vector3Int(endX, startY));
                SpawnTopColumns(startX, startY, endX, startY);
                _areaCells.Add(new Vector3Int(endX, startY));

                _areaCells = new List<Vector3Int>();
                FillArea(new Vector3Int(startX, startY - 2), new Vector3Int(endX, startY + 2));
            }
            else
            {
                if (size.y > MAX_SIZE)
                {
                    endY = (int)Mathf.MoveTowards(startY, endY, MAX_SIZE);
                    _size = MAX_SIZE;
                }
                else
                    _size = size.y;

                _columnEnd.position = MapGrid.current.grid.GetCellCenterWorld(new Vector3Int(startX, endY));
                SpawnTopColumns(startX, startY, startX, endY);
                _areaCells.Add(new Vector3Int(startX, endY));

                _areaCells = new List<Vector3Int>();
                FillArea(new Vector3Int(startX + 2, startY), new Vector3Int(startX - 2, endY));
            }
        }
        else
        {
            transform.position = MapGrid.current.grid.GetCellCenterWorld(cell);
            _areaCells = new List<Vector3Int>() { cell };
        }
    }

    public override void MouseUp(Vector3 position)
    {

    }

    public override void Rotate()
    {

    }

    public override void Cancel()
    {
        _onCancel();
    }

    public override void Place()
    {
        _isExpand = false;
    }

    private void SpawnTopColumns(int startX, int startY, int endX, int endY)
    {
        int x = startX;
        int y = startY;

        _topColumnPool.HideEverything();
        while (x != endX || y != endY)
        {
            if (x != endX)
                x = (int)Mathf.MoveTowards(x, endX, 1);
            else
                y = (int)Mathf.MoveTowards(y, endY, 1);

            Transform column = _topColumnPool.Get();
            column.position = MapGrid.current.grid.GetCellCenterWorld(new Vector3Int(x, y));

            column.gameObject.SetActive(true);
        }
    }

    private void FillArea(Vector3Int startCell, Vector3Int endCell)
    {
        int x = startCell.x;
        int y = startCell.y;
        for (x = startCell.x; true; x = (int)Mathf.MoveTowards(x, endCell.x, 1))
        {
            for (y = startCell.y; true; y = (int)Mathf.MoveTowards(y, endCell.y, 1))
            {
                _areaCells.Add(new Vector3Int(x, y));

                if (y == endCell.y)
                    break;
            }
            if (x == endCell.x)
                break;
        }
    }
}
