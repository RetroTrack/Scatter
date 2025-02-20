using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    public bool canMoveMouse = true;
    public bool canMoveKeyboard = true;
    [SerializeField] private float _movementSpeed = 0.2f;

    [SerializeField] private Tilemap _tilemap;

    [Header("Camera Zoom")]
    [SerializeField] private float _zoomSpeed = 1f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 20f;

    public Vector3 startMousePosition;
    public bool isDragging;

    private Camera _camera;
    [HideInInspector] public static CameraController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        _camera = Camera.main;
    }

    void Update()
    {
        _camera = Camera.main;
        if (canMoveKeyboard)
        {
            MoveCameraKeyboard();
            ZoomCameraKeyboard();
        }

        //Check if pointer isn't over UI and if mouse movement is enabled
        if (canMoveMouse && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            MoveCameraMouse();
            ZoomCameraMouse();
        }

        LimitCamera();
    }

    private void MoveCameraMouse()
    {

        //Mouse Dragging
        if (Input.GetMouseButtonDown(1))
        {
            startMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging && Input.GetMouseButton(1))
        {
            Vector3 currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = startMousePosition - currentMousePosition;
            offset.z = 0;
            _camera.transform.position += offset;
        }
    }

    private void MoveCameraKeyboard()
    {
        //Keyboard Movement
        if (!isDragging)
        {
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            _camera.transform.position += moveDirection * _movementSpeed * Time.deltaTime * 100;
        }

    }

    private void ZoomCameraKeyboard()
    {

        float scrollDelta = 0;
        if(Input.GetKey(KeyCode.Q))
        {
            scrollDelta = 0.3f;
        }
        else
        if (Input.GetKey(KeyCode.E))
        {
            scrollDelta = -0.3f;
        }
        if (scrollDelta != 0)
        {
            float newSize = Mathf.Clamp(_camera.orthographicSize - scrollDelta * _zoomSpeed, _minZoom, _maxZoom);
            Vector3 zoomDirection = _camera.ScreenToWorldPoint(Input.mousePosition) - _camera.transform.position;

            float zoomFactor = (newSize - _camera.orthographicSize) / _camera.orthographicSize;
            _camera.transform.position += zoomDirection * zoomFactor * -1;

            _camera.orthographicSize = newSize;
        }
    }

    private void ZoomCameraMouse()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            float newSize = Mathf.Clamp(_camera.orthographicSize - scrollDelta * _zoomSpeed, _minZoom, _maxZoom);
            Vector3 zoomDirection = _camera.ScreenToWorldPoint(Input.mousePosition) - _camera.transform.position;

            float zoomFactor = (newSize - _camera.orthographicSize) / _camera.orthographicSize;
            _camera.transform.position += zoomDirection * zoomFactor * -1;

            _camera.orthographicSize = newSize;
        }
    }

    private void LimitCamera()
    {
        if (_tilemap == null) return;

        Vector3 cameraSize = new Vector3(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize, 0);
        Vector3 minPosition = _tilemap.localBounds.min + cameraSize;
        Vector3 maxPosition = _tilemap.localBounds.max - cameraSize;

        Vector3 clampedPosition = _camera.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minPosition.x, maxPosition.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minPosition.y, maxPosition.y);
        clampedPosition.z = -10;

        _camera.transform.position = clampedPosition;
    }
}
