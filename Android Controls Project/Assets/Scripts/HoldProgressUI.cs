using UnityEngine;
using UnityEngine.UI;

// HoldProgressUI controls the progress ring filling while a DragonBall is held
public class HoldProgressUI : MonoBehaviour
{
    private Image ringImage;

    void Awake()
    {
        // Get the Image component on this same GameObject
        ringImage = GetComponent<Image>();
    }

    // Call this from DragonBallCollector to update the fill (0 = empty, 1 = full)
    public void SetProgress(float progress)
    {
        if (ringImage != null)
        {
            ringImage.fillAmount = progress;
        }
    }

    // Show the ring at a specific position on screen
    public void Show(Vector3 screenPosition)
    {
        gameObject.SetActive(true);
        transform.position = screenPosition;
    }

    // Hide the ring
    public void Hide()
    {
        gameObject.SetActive(false);
        if (ringImage != null) ringImage.fillAmount = 0;
    }
}