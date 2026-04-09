using UnityEngine;

public class DragonBallData : MonoBehaviour
{
    [Header("Identity")]
    public int starCount = 1;

    [Header("Current Status")]
    [TextArea(3, 6)]
    public string currentStatus = "Whereabouts unknown.";

    public void UpdateStatus(string newStatus)
    {
        currentStatus = newStatus;
    }

    public string GetDisplayName()
    {
        return starCount + "-Star Dragon Ball";
    }
}