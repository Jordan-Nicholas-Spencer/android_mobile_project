using UnityEngine;

public class DragonBallPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseCycleDuration = 2f;
    public float pulseScaleAmount = 0.2f;
    public float minBrightness = 0.6f;
    public float maxBrightness = 1.0f;

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
        // Smooth sine wave 0-1 over the cycle duration
        float t = (Mathf.Sin(Time.time * (2f * Mathf.PI / pulseCycleDuration)) + 1f) / 2f;

        // Scale
        transform.localScale = baseScale * (1f + t * pulseScaleAmount);

        // Brightness
        if (sr != null)
        {
            float b = Mathf.Lerp(minBrightness, maxBrightness, t);
            sr.color = new Color(baseColor.r * b, baseColor.g * b, baseColor.b * b, baseColor.a);
        }
    }
}