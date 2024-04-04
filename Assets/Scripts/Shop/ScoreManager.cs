using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int playerScore = 0;

    public int scoreMultiplier = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persistent
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void AddScore(int amount)
    {
        playerScore += amount;
    }
    public void SubtractScore(int amount)
    {
        playerScore -= amount;
    }

    public int GetScore()
    {
        return playerScore;
    }

    public void SetScoreMultiplier(int multiplier)
    {
        scoreMultiplier = multiplier;
    }

    public int GetScoreMultiplier()
    {
        return scoreMultiplier;
    }
}
