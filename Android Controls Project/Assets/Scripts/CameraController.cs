using UnityEngine;
using UnityEngine.InputSystem;

// CameraController handles:
// 1. One-finger swipe to pan
// 2. Two-finger pinch to zoom
public class CameraController : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 0.01f;      // How fast the camera pans per pixel of swipe

    [Header("Zoom Settings")]
    public float minZoom = 1.5f;        // Closest zoom (smaller number = more zoomed in)
    public float maxZoom = 7f;          // Furthest zoom (larger number = more zoomed out)
    public float zoomSpeed = 0.01f;     // How fast zooming responds to pinch

    [Header("Map Limits")]
    public float mapHalfWidth = 9f;     // Half the width of the map background
    public float mapHalfHeight = 9f;    // Half the height of the map background

    private Camera cam;

    // We store previous touch positions to calculate deltas
    private Vector2 prevSingleTouchPos;
    private float prevPinchDistance;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Get all current touches from the new Input System
        var touches = Touchscreen.current;

        // If there is no touchscreen detected (e.g. in editor without Remote), do nothing
        if (touches == null) return;

        int touchCount = GetActiveTouchCount(touches);

        if (touchCount == 1)
        {
            HandlePan(touches);
        }
        else if (touchCount == 2)
        {
            HandlePinchZoom(touches);
        }
    }

    // Count how many fingers are actively touching the screen
    int GetActiveTouchCount(Touchscreen screen)
    {
        int count = 0;
        foreach (var touch in screen.touches)
        {
            if (touch.isInProgress) count++;
        }
        return count;
    }

    void HandlePan(Touchscreen screen)
    {
        // Find the first active touch
        foreach (var touch in screen.touches)
        {
            if (!touch.isInProgress) continue;

            Vector2 currentPos = touch.position.ReadValue();

            // On the frame the finger just pressed, record position without moving
            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                prevSingleTouchPos = currentPos;
                return;
            }

            // Calculate how far the finger moved since last frame
            Vector2 delta = currentPos - prevSingleTouchPos;
            prevSingleTouchPos = currentPos;

            // Move the camera in the opposite direction of the swipe (drag feel)
            Vector3 move = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0);
            transform.position += move;

            // Clamp camera within map bounds
            float x = Mathf.Clamp(transform.position.x, -mapHalfWidth, mapHalfWidth);
            float y = Mathf.Clamp(transform.position.y, -mapHalfHeight, mapHalfHeight);
            transform.position = new Vector3(x, y, transform.position.z);

            break; // Only handle the first touch
        }
    }

    void HandlePinchZoom(Touchscreen screen)
    {
        // Get the two active touches
        UnityEngine.InputSystem.Controls.TouchControl touch0 = null;
        UnityEngine.InputSystem.Controls.TouchControl touch1 = null;

        foreach (var touch in screen.touches)
        {
            if (!touch.isInProgress) continue;
            if (touch0 == null) touch0 = touch;
            else if (touch1 == null) { touch1 = touch; break; }
        }

        if (touch0 == null || touch1 == null) return;

        float currentDistance = Vector2.Distance(
            touch0.position.ReadValue(),
            touch1.position.ReadValue()
        );

        // On the first frame of a two-finger touch, just record the distance
        if (touch0.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began ||
            touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            prevPinchDistance = currentDistance;
            return;
        }

        float pinchDelta = prevPinchDistance - currentDistance;
        prevPinchDistance = currentDistance;

        // Adjust orthographic camera size (zoom)
        cam.orthographicSize += pinchDelta * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}