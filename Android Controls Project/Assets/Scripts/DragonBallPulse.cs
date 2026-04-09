using UnityEngine;

public class DragonBallPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseCycleDuration = 2f;
    public float pulseScaleAmount = 0.2f;

    [Header("Sound")]
    public bool playPulseSound = true;
    // Small window around the peak where the sound fires (prevents double-firing)
    private float soundTriggerThreshold = 0.1f;
    private bool soundPlayedThisCycle = false;

    private Vector3 baseScale;
    private SpriteRenderer sr;
    private Color baseColor;

    void Start()
    {
        baseScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) baseColor = sr.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * (2f * Mathf.PI / pulseCycleDuration)) + 1f) / 2f;

        // Scale
        transform.localScale = baseScale * (1f + t * pulseScaleAmount);

        // Alpha fade — invisible at bottom, visible at peak
        if (sr != null)
        {
            sr.color = new Color(
                baseColor.r,
                baseColor.g,
                baseColor.b,
                Mathf.Pow(t, 0.4f)
            );
        }

        // Play pulse sound once when t crosses the peak threshold
        if (playPulseSound && AudioManager.Instance != null)
        {
            if (t >= soundTriggerThreshold && !soundPlayedThisCycle)
            {
                AudioManager.Instance.PlayPulse();
                soundPlayedThisCycle = true;
            }
            else if (t < soundTriggerThreshold)
            {
                // Reset so it can fire again next cycle
                soundPlayedThisCycle = false;
            }
        }
    }
}