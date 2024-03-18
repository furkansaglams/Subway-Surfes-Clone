using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    public Transform player;
    [SerializeField] private float score;
    [SerializeField] private int scoreSpeedDivinding;
    public int highScore;
    private float playerFrwdSpeed;
    private bool polishScoreOnce;
    private bool hasPlayedOnce;
    private Color originalColor;
    private Vector3 originalScale;

    public static ScoreSystem instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerFrwdSpeed = PlayerController.instance.frwdSpeed;
        if (PlayerPrefs.HasKey("highScore"))
        {
            hasPlayedOnce = true;
        }
        highScore = PlayerPrefs.GetInt("highScore", 0);
        originalColor = scoreText.color;
        originalScale = scoreText.transform.localScale;
    }

    void Update()
    {
        score = player.position.z / (playerFrwdSpeed / scoreSpeedDivinding);
        score = Mathf.FloorToInt(score);
        scoreText.text = score.ToString();

        if (score > highScore)
        {
            highScore = (int)score;
            if (!polishScoreOnce && hasPlayedOnce)
            {
                polishScoreOnce = true;
                StartCoroutine(ChangeScoreAppearance());

            }
        }

    }
    IEnumerator ChangeScoreAppearance()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            scoreText.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);
            scoreText.color = Color.Lerp(originalColor, Color.red, t);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            scoreText.transform.localScale = Vector3.Lerp(originalScale * 1.5f, originalScale, t);
            scoreText.color = Color.Lerp(Color.red, originalColor, t);
            yield return null;
        }
    }


    public void UpdateHighScore()
    {
        highscoreText.text = "NewHighScore! " + highScore;
    }

    private void OnApplicationQuit()
    {
        if (highScore > 0)
        {
            PlayerPrefs.SetInt("highScore", highScore);
        }

    }
}
