using UnityEngine;
using UnityEngine.UI;

public class HoldProgressUI : MonoBehaviour
{
    private Image ringImage;

    void Awake()
    {
        ringImage = GetComponent<Image>();
    }

    public void SetProgress(float progress)
    {
        if (ringImage != null)
            ringImage.fillAmount = progress;
    }

    public void Show(Vector3 screenPosition)
    {
        gameObject.SetActive(true);
        transform.position = screenPosition;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (ringImage != null)
            ringImage.fillAmount = 0;
    }
}