using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int initialLives = 3;

    private int score;
    private int liveCount;
    

    private void Awake()
    {
        Instance = this;
        liveCount = initialLives;
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    public int GetLiveCount()
    {
        return liveCount;
    }

}
