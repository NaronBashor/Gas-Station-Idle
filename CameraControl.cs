using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minOrthoSize = 2f;
    [SerializeField] private float maxOrthoSize = 20f;

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 20f;

    [Header("Gameplay Area Settings")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;
    private Vector3 lastMousePosition;

    private void Start()
    {
        cam = Camera.main; // Assuming this script is controlling the main camera
    }

    private void Update()
    {
        HandleZoom();
        HandlePan();
        ClampCameraPosition();
    }

    private void HandleZoom()
    {
        if (PlayerPrefs.GetString("Zoom") == "False") { return; }
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scrollInput * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthoSize, maxOrthoSize);
    }

    private void HandlePan()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button clicked
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;

            // Convert the delta to world space, considering the zoom level
            Vector3 panMovement = new Vector3(-deltaMousePosition.x * panSpeed * Time.deltaTime, -deltaMousePosition.y * panSpeed * Time.deltaTime, 0);

            cam.transform.Translate(panMovement, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }

    private void ClampCameraPosition()
    {
        // Calculate camera boundaries in world space based on zoom level
        float cameraHeight = 2f * cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        float halfHeight = cameraHeight / 2f;
        float halfWidth = cameraWidth / 2f;

        Vector3 position = cam.transform.position;

        // Clamp the camera position to ensure it stays within the boundaries
        position.x = Mathf.Clamp(position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        position.y = Mathf.Clamp(position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);

        cam.transform.position = position;
    }
}
