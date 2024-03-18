using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverHighScoreText;
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverHighScoreText.text = "High Score: " + ScoreSystem.instance.highScore;
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
