using UnityEngine;
using TMPro;

// GameManager tracks the score and handles the win condition
public class GameManager : MonoBehaviour
{
    // Drag these in from the Inspector
    public TextMeshProUGUI scoreText;
    public GameObject winPanel;

    private int score = 0;
    private int totalDragonBalls = 7;

    // This is called by DragonBallCollector when a DragonBall is collected
    public void DragonBallCollected()
    {
        score++;
        UpdateScoreUI();

        if (score >= totalDragonBalls)
        {
            ShowWin();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void ShowWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }
}