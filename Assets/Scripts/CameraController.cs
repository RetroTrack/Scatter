using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public static CameraController Instance { get; private set; }

    [Header("Camera Movement")]
    public bool canMoveMouse = true;
    public bool canMoveKeyboard = true;
    [SerializeField] private float _movementSpeed = 0.2f;

    [SerializeField] private Tilemap _tilemap;

    [Header("Camera Zoom")]
    [SerializeField] private float _zoomSpeed = 1f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 20f;

    public Vector3 _startMousePosition;
    private bool _isDragging = false;

    private Camera _camera;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        Instance = this;
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        _camera = Camera.main;

        if (canMoveKeyboard)
        {
            MoveCameraKeyboard();
            ZoomCameraKeyboard();
        }

        //Check if mouse movement is enabled and if pointer isn't over UI 
        if (canMoveMouse && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            MoveCameraMouse();
            ZoomCameraMouse();
        }

        //Make sure camera stays within bounds of tilemap
        LimitCamera();
    }

    #region Movement
    private void MoveCameraMouse()
    {

        //Mouse Dragging
        if (Input.GetMouseButtonDown(1))
        {
            _startMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _isDragging = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            _isDragging = false;
        }

        if (_isDragging && Input.GetMouseButton(1))
        {
            Vector3 currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = _startMousePosition - currentMousePosition;
            offset.z = 0;
            _camera.transform.position += offset;
        }
    }

    private void MoveCameraKeyboard()
    {
        //Keyboard Movement
        if (!_isDragging)
        {
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            _camera.transform.position += moveDirection * _movementSpeed * Time.deltaTime * 100;
        }

    }
    #endregion

    #region Zooming
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

    #endregion

    #region Limiting
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
    #endregion
}
