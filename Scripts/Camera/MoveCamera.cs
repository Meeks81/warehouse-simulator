using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    [SerializeField] private float _speed;
    [SerializeField] private float _sensetivity;
    [SerializeField] private float _keysSensetivity;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float zoomMin = 3;
    [SerializeField] private float zoomMax = 10;
    [SerializeField] private float minYRotate = 20f;
    [SerializeField] private float maxYRotate = 80f;
    [SerializeField] private float _zoomSensetivity;
    [Space]
    [SerializeField] private Camera _camera;

    private float X;
    private float Y;
    private float _zoom;

    private void Start()
    {
        X = 0;
        _zoom = zoomMax;
    }

    private void LateUpdate()
    {
        Vector3 movement = Vector3.zero;

        if (HotKey.GetKey(HotKeyType.CameraMoveForward))
            movement.z++;
        if (HotKey.GetKey(HotKeyType.CameraMoveBack))
            movement.z--;
        if (HotKey.GetKey(HotKeyType.CameraMoveLeft))
            movement.x--;
        if (HotKey.GetKey(HotKeyType.CameraMoveRight))
            movement.x++;

        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        _zoom = Mathf.Clamp(_zoom - mouseScroll * _zoomSensetivity, zoomMin, zoomMax);
        _offset.z = -_zoom;

        if (HotKey.GetKey(HotKeyType.CameraRotateLeft))
            X += _keysSensetivity * Time.deltaTime;
        if (HotKey.GetKey(HotKeyType.CameraRotateRight))
            X -= _keysSensetivity * Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            X += Input.GetAxis("Mouse X") * _sensetivity;
        }
        Y = Mathf.Lerp(minYRotate, maxYRotate, 1f / (zoomMax - zoomMin) * (_zoom - zoomMin));
        _camera.transform.eulerAngles = new Vector3(Y, X, 0);
        transform.eulerAngles = new Vector3(0, X, 0);

        transform.Translate(movement * _speed * Time.deltaTime);

        Vector3 cameraPosition = _camera.transform.rotation * _offset + transform.position;
        _camera.transform.position = cameraPosition;
    }

}
