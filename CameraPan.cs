using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;  // Speed of the camera movement

    private Camera cam;
    private Vector3 lastMousePosition;

    private void Start()
    {
        cam = Camera.main; // Assuming this script is controlling the main camera
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button clicked
        {
            lastMousePosition = Input.mousePosition; // Store the mouse position when the right button is first pressed
        }

        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition; // Calculate mouse movement
            Vector3 panMovement = new Vector3(-deltaMousePosition.x * panSpeed * Time.deltaTime, -deltaMousePosition.y * panSpeed * Time.deltaTime, 0);

            // Apply the pan movement
            cam.transform.Translate(panMovement, Space.Self);

            lastMousePosition = Input.mousePosition; // Update the last mouse position
        }
    }
}