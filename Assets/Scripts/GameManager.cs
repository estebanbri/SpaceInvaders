using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int livesInitial = 4;
    [SerializeField] private int livesMax = 4;
    private int score;
    private int livesCurrent;

    private void Awake()
    {
        Instance = this;
        livesCurrent = livesInitial;
        UIVidas.Instance.Initialize(livesMax);
        UIVidas.Instance.SetLivesIcons(livesCurrent);
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public int GetScore()
    {
        return score;
    }

    public bool HasPendingRetries()
    {
        return livesCurrent > 0;
    }

    public int GetRetryCount()
    {
        return livesCurrent;
    }

    public bool IsFullVidas() {
        return livesCurrent == livesMax;
    }

    public void DecreaseRetry()
    {
        livesCurrent--;
        UIVidas.Instance.SetLivesIcons(livesCurrent);
        if (livesCurrent <= 0)
        {
            Debug.Log("Game Over!");
            // Aquí podrías agregar lógica para reiniciar el juego o mostrar una pantalla de Game Over
        }
    }

    public void GameOver()
    {
        Debug.Log("GAMEOVER!");
    }

    public void AddVida()
    {
        if (livesCurrent >= livesMax) return;
        livesCurrent++;
        UIVidas.Instance.SetLivesIcons(livesCurrent);
    }

    public void OnPlayerDeath(Nave nave)
    {
        DecreaseRetry();

        if (HasPendingRetries()) {
            nave.Respawn();
        } else {
            Destroy(nave.gameObject);
            GameOver();
        }
    }
}
