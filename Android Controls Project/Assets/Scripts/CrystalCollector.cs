using UnityEngine;
using UnityEngine.InputSystem;

// DragonBallCollector handles press-and-hold to collect DragonBalls
public class DragonBallCollector : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdDuration = 1.2f;   // Seconds needed to collect

    [Header("References")]
    public HoldProgressUI progressUI;   // Drag in the HoldProgressRing object
    public GameManager gameManager;     // Drag in the GameManager object

    private Camera cam;
    private GameObject currentTarget;  // The DragonBall being held
    private float holdTimer = 0f;
    private bool isHolding = false;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null) return;

        // Only process DragonBall holding if exactly one finger is touching
        // (Prevents conflicts with pinch zoom)
        int touchCount = CountActiveTouches(touchscreen);
        if (touchCount != 1)
        {
            CancelHold();
            return;
        }

        // Find the first active touch
        UnityEngine.InputSystem.Controls.TouchControl activeTouch = null;
        foreach (var t in touchscreen.touches)
        {
            if (t.isInProgress) { activeTouch = t; break; }
        }
        if (activeTouch == null) { CancelHold(); return; }

        var phase = activeTouch.phase.ReadValue();
        Vector2 touchPos = activeTouch.position.ReadValue();

        if (phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            // Finger just touched — check if it hit a DragonBall
            TryStartHold(touchPos);
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Stationary ||
                 phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            // Finger is still down — continue hold timer if we're targeting a DragonBall
            if (isHolding && currentTarget != null)
            {
                holdTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(holdTimer / holdDuration);

                // Update the progress ring
                if (progressUI != null)
                    progressUI.SetProgress(progress);

                // Check if hold is complete
                if (holdTimer >= holdDuration)
                {
                    CollectDragonBall();
                }
            }
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                 phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            // Finger lifted — cancel hold
            CancelHold();
        }
    }

    int CountActiveTouches(Touchscreen screen)
    {
        int count = 0;
        foreach (var t in screen.touches)
            if (t.isInProgress) count++;
        return count;
    }

    void TryStartHold(Vector2 screenPos)
    {
        // Cast a ray from the camera through the touch position into the 2D scene
        Ray ray = cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.CompareTag("DragonBall"))
        {
            // We hit a DragonBall — start holding
            currentTarget = hit.collider.gameObject;
            isHolding = true;
            holdTimer = 0f;

            // Show progress ring at touch position
            if (progressUI != null)
                progressUI.Show(new Vector3(screenPos.x, screenPos.y + 160f, 0));
        }
    }

    void CollectDragonBall()
    {
        if (currentTarget != null)
        {
            currentTarget.SetActive(false);   // Hide the DragonBall
            currentTarget = null;

            if (gameManager != null)
                gameManager.DragonBallCollected();  // Tell GameManager
        }

        CancelHold();
    }

    void CancelHold()
    {
        isHolding = false;
        holdTimer = 0f;
        currentTarget = null;

        if (progressUI != null)
            progressUI.Hide();
    }
}