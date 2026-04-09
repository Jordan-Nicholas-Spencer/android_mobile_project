using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float minZoom = 2f;       // Most zoomed in
    public float maxZoom = 12f;      // Fully zoomed out — shows whole radar
    public float zoomSpeed = 0.01f;

    [Header("Pan Settings")]
    public float panSpeed = 0.01f;
    public float panDeadZone = 11f;  // At this zoom or higher, panning is disabled
                                     // so the radar stays centered when zoomed out

    [Header("Map Bounds")]
    public float mapHalfWidth = 9f;
    public float mapHalfHeight = 5f;

    private Camera cam;
    private Vector2 prevSingleTouchPos;
    private float prevPinchDistance;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = maxZoom; // Start fully zoomed out
    }

    void Update()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null) return;

        int count = CountTouches(touchscreen);

        if (count == 1)
            HandlePan(touchscreen);
        else if (count == 2)
            HandlePinch(touchscreen);
    }

    int CountTouches(Touchscreen s)
    {
        int n = 0;
        foreach (var t in s.touches)
            if (t.isInProgress) n++;
        return n;
    }

    void HandlePan(Touchscreen s)
    {
        // Disable pan when close to fully zoomed out
        if (cam.orthographicSize >= panDeadZone) return;

        foreach (var touch in s.touches)
        {
            if (!touch.isInProgress) continue;

            Vector2 pos = touch.position.ReadValue();

            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                prevSingleTouchPos = pos;
                return;
            }

            Vector2 delta = pos - prevSingleTouchPos;
            prevSingleTouchPos = pos;

            Vector3 move = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0);
            transform.position += move;

            // Clamp within map
            float x = Mathf.Clamp(transform.position.x, -mapHalfWidth, mapHalfWidth);
            float y = Mathf.Clamp(transform.position.y, -mapHalfHeight, mapHalfHeight);
            transform.position = new Vector3(x, y, transform.position.z);
            break;
        }
    }

    void HandlePinch(Touchscreen s)
    {
        UnityEngine.InputSystem.Controls.TouchControl t0 = null, t1 = null;
        foreach (var t in s.touches)
        {
            if (!t.isInProgress) continue;
            if (t0 == null) t0 = t;
            else { t1 = t; break; }
        }
        if (t0 == null || t1 == null) return;

        float dist = Vector2.Distance(
            t0.position.ReadValue(), t1.position.ReadValue());

        if (t0.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began ||
            t1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            prevPinchDistance = dist;
            return;
        }

        float delta = prevPinchDistance - dist;
        prevPinchDistance = dist;

        cam.orthographicSize += delta * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        // When zooming back out to max, re-center the camera
        if (cam.orthographicSize >= maxZoom - 0.1f)
        {
            transform.position = new Vector3(0, 0, transform.position.z);
        }
    }
}