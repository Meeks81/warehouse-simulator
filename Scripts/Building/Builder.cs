using UnityEngine;
using UnityEngine.Events;

public class Builder : MonoBehaviour
{

    [SerializeField] private Map _map;
    [SerializeField] private BuildingPermit _buildingPermit;

    [HideInInspector] public UnityEvent<BuildingController, Vector3, Vector3> OnBuildingPlaced;
    [HideInInspector] public UnityEvent<BuildingController, Vector3, Vector3> OnBuildingEdited;
    [HideInInspector] public UnityEvent<BuildingController> OnBuildingDeleted;
    [HideInInspector] public UnityEvent<BuildingController> OnFlyingBuildingChanged;
    [HideInInspector] public UnityEvent<BuildingController> OnFlyingBuildingMoved;

    public BuildingController FlyingBuilding { get; private set; }

    private Vector3Int _lastCell;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (FlyingBuilding == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) &&
                    hit.transform != null &&
                    hit.transform.TryGetComponent(out BuildingController buildObject) &&
                    buildObject.IsEdit)
                {
                    SetBuilding(buildObject);
                }
            }
        }
        else
        {
            MoveFlyingObject();
        }
    }

    public void SetBuilding(BuildingController building)
    {
        if (FlyingBuilding != null)
        {
            CancelEditFlyingObject();
        }

        FlyingBuilding = building;
        FlyingBuilding.Init(PlaceFlyingObject, CancelEditFlyingObject);

        OnFlyingBuildingChanged.Invoke(building);
    }

    public void PlaceFlyingObject(BuildingController[] buildings)
    {
        if (FlyingBuilding == null || _buildingPermit.IsBuildingPlaceAvailable(FlyingBuilding) == false)
            return;

        FlyingBuilding = null;

        foreach (var item in buildings)
        {
            OnBuildingPlaced.Invoke(item, item.transform.position, item.transform.eulerAngles);
            OnBuildingEdited.Invoke(item, item.transform.position, item.transform.eulerAngles);
        }
    }

    private void CancelEditFlyingObject()
    {
        if (_map.BuildObjects.Contains(FlyingBuilding) == false)
        {
            Destroy(FlyingBuilding.gameObject);
        }

        OnBuildingEdited.Invoke(FlyingBuilding, FlyingBuilding.transform.position, FlyingBuilding.transform.eulerAngles);

        FlyingBuilding = null;
    }

    private void MoveFlyingObject()
    {
        if (FlyingBuilding == null)
            return;

        if (HotKey.GetKey(HotKeyType.BuildObjectRotate))
        {
            FlyingBuilding.Rotate();
        }

        Vector3 cursorPosition = GetRayPosition();

        FlyingBuilding.MouseMove(cursorPosition);

        Vector3Int[] area = FlyingBuilding.GetCurrentAreaCells();
        Vector3Int checkCell = area[0] + area[area.Length - 1];

        if (checkCell != _lastCell)
        {
            OnFlyingBuildingMoved.Invoke(FlyingBuilding);

            _lastCell = checkCell;
        }

        if (Input.GetMouseButtonDown(0))
        {
            FlyingBuilding.MouseDown(cursorPosition);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FlyingBuilding.Cancel();
        }
    }

    private Vector3 GetRayPosition()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        if (hPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            return new Vector3(point.x, point.y, point.z);
        }
        return Vector3.zero;
    }

}
