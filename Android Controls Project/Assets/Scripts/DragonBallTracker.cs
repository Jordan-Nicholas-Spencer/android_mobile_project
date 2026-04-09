using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DragonBallTracker : MonoBehaviour
{
    [Header("Hold Settings")]
    public float holdDuration = 1.0f;

    [Header("References")]
    public HoldProgressUI progressUI;
    public GameObject infoPanel;
    public TextMeshProUGUI infoTitleText;
    public TextMeshProUGUI infoBodyText;
    public GameManager gameManager;

    private Camera cam;
    private GameObject currentTarget;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool panelOpen = false;
    private float touchSoundCooldown = 0f;
    private float touchSoundCooldownDuration = 0.5f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    void Update()
    {
        if (panelOpen) return;

        if (touchSoundCooldown > 0f)
            touchSoundCooldown -= Time.deltaTime;

        var ts = Touchscreen.current;
        if (ts == null) return;

        int count = CountTouches(ts);
        if (count != 1) { CancelHold(); return; }

        UnityEngine.InputSystem.Controls.TouchControl active = null;
        foreach (var t in ts.touches)
            if (t.isInProgress) { active = t; break; }

        if (active == null) { CancelHold(); return; }

        var phase = active.phase.ReadValue();
        Vector2 pos = active.position.ReadValue();

        if (phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            TryStartHold(pos);
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Stationary ||
                 phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            if (isHolding && currentTarget != null)
            {
                holdTimer += Time.deltaTime;
                if (progressUI != null)
                    progressUI.SetProgress(Mathf.Clamp01(holdTimer / holdDuration));

                if (holdTimer >= holdDuration)
                    ShowInfo();
            }
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                 phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            CancelHold();
        }
    }

    int CountTouches(Touchscreen s)
    {
        int n = 0;
        foreach (var t in s.touches) if (t.isInProgress) n++;
        return n;
    }

    void TryStartHold(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.CompareTag("DragonBall"))
        {
            currentTarget = hit.collider.gameObject;
            isHolding = true;
            holdTimer = 0f;

            if (progressUI != null)
                progressUI.Show(new Vector3(screenPos.x, screenPos.y + 160f, 0));

            // Play touch sound when finger first contacts a dragon ball
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayTouch();
                touchSoundCooldown = touchSoundCooldownDuration;
        }
    }

    void ShowInfo()
    {
        if (currentTarget == null) return;

        DragonBallData data = currentTarget.GetComponent<DragonBallData>();
        if (data == null) return;

        if (infoTitleText != null) infoTitleText.text = data.GetDisplayName();
        if (infoBodyText != null) infoBodyText.text = data.currentStatus;
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMessageOpen();
        }

        if (gameManager != null) gameManager.DragonBallFound(currentTarget.GetInstanceID());

        panelOpen = true;
        CancelHold();
    }

    public void CloseInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        panelOpen = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMessageClose();
    }

    void CancelHold()
    {
        isHolding = false;
        holdTimer = 0f;
        currentTarget = null;
        if (progressUI != null) progressUI.Hide();
    }
}