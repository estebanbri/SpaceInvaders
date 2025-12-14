using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int initialRetryCount = 3;

    private int score;
    private int retryCount;
    

    private void Awake()
    {
        Instance = this;
        retryCount = initialRetryCount;
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

    public bool HasPendingRetries()
    {
        return retryCount > 0;
    }

    public int GetRetryCount()
    {
        return retryCount;
    }

    public void DecreaseRetry()
    {
        retryCount--;
        if (retryCount <= 0)
        {
            Debug.Log("Game Over!");
            // Aquí podrías agregar lógica para reiniciar el juego o mostrar una pantalla de Game Over
        }
    }

    internal void GameOver()
    {
        Debug.Log("GAMEOVER!");
    }
}
