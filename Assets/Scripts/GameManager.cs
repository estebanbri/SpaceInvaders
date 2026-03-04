using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    private int score;

    [SerializeField] private LevelController levelController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // -------------------------
    // LEVEL FLOW
    // -------------------------

    public void StartLevel()
    {
        score = 0;
        CurrentState = GameState.Playing;

        levelController.StartLevel(OnLevelCompleted);
    }

    public void OnPlayerDeath()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.GameOver;

        levelController.StopLevel();
    }

    private void OnLevelCompleted()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.LevelCompleted;

        levelController.StopLevel();
    }

    // -------------------------
    // SCORE SYSTEM
    // -------------------------

    public void AddScore(int amount)
    {
        score += amount;
    }

    public int GetScore()
    {
        return score;
    }

    public bool CanAfford(int cost)
    {
        return score >= cost;
    }

    public bool SpendScore(int cost)
    {
        if (score < cost)
            return false;

        score -= cost;
        return true;
    }

    // -------------------------
    // UTILITIES
    // -------------------------

    public bool IsPlaying()
    {
        return CurrentState == GameState.Playing;
    }
}