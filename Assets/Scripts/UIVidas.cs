using System.Collections.Generic;
using UnityEngine;

public class UIVidas : MonoBehaviour
{
    public static UIVidas Instance { get; private set; }

    [SerializeField] private GameObject shipIconPrefab;

    private readonly List<GameObject> icons = new();
    private int livesMax;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(int livesMax)
    {
        this.livesMax = livesMax;
        ClearIcons();

        // Creamos todas las vidas posibles una sola vez
        for (int i = 0; i < livesMax; i++)
        {
            GameObject icon = Instantiate(shipIconPrefab, transform);
            icons.Add(icon);
        }

        SetLives(livesMax);
    }

    public void SetLives(int livesCurrent)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].SetActive(i < livesCurrent);
        }
    }

    private void ClearIcons()
    {
        foreach (var icon in icons)
            Destroy(icon);

        icons.Clear();
    }
}