using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI scoreText;

    private HashSet<int> foundBallIds = new HashSet<int>();
    private int total = 7;

    // Called when a ball is viewed. instanceID uniquely identifies each GameObject.
    public void DragonBallFound(int instanceID)
    {
        if (!foundBallIds.Contains(instanceID))
        {
            foundBallIds.Add(instanceID);
        }
        UpdateUI();
    }

    // Called when a ball moves — removes it from the found set
    public void DragonBallLost(int instanceID)
    {
        if (foundBallIds.Contains(instanceID))
        {
            foundBallIds.Remove(instanceID);
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Balls Found: " + foundBallIds.Count + " / " + total;
    }
}