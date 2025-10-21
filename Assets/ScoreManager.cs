using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;   // Singleton để dễ truy cập
    public int score = 0;

    public TextMeshProUGUI scoreText;     // Gán UI Text vào đây

    private void Awake()
    {
        // Đảm bảo chỉ có 1 Score tồn tại
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}
