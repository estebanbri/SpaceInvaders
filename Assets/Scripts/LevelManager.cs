using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Levels")]
    [SerializeField] private List<LevelDefinition> levels;

    [Header("Formation")]
    [SerializeField] private FormationController formationPrefab;

    [Header("Boss")]
    [SerializeField] private BossHealthBarUI bossHealthBar;

    [Header("Parallax")]
    [SerializeField] private VerticalParallax parallax;

    [Header("Bonus")]
    [SerializeField] private BonusBoxSpawner bonusBoxSpawner;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;

    private int currentLevelIndex = 0;
    private int currentWaveIndex = 0;

    private FormationController activeFormation;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        if (currentWaveIndex > LastWaveIndex())
            return;

        WaveDefinition wave = levels[currentLevelIndex].waves[currentWaveIndex];

        UpdateWaveUI();

        if (wave.isBossWave)
        {
            SpawnBoss(wave.bossPrefab);
            return;
        }

        SpawnFormation(wave);
    }

    void SpawnFormation(WaveDefinition wave)
    {
        activeFormation = Instantiate(formationPrefab);

        activeFormation.Initialize(wave, currentWaveIndex);
    }

    public void OnFormationCleared()
    {
        currentWaveIndex++;
        StartWave();
    }

    void SpawnBoss(GameObject bossPrefab)
    {
        GameObject bossGO = Instantiate(bossPrefab, new Vector3(0, 4f, 0), Quaternion.identity);

        Enemigo boss = bossGO.GetComponent<Enemigo>();

        if (boss != null && bossHealthBar != null)
        {
            bossHealthBar.Bind(boss);
        }

        boss.OnEnemyDied += () =>
        {
            if (bonusBoxSpawner != null)
            {
                bonusBoxSpawner.Spawn(new Vector3(0, 5f, 0));
            }

            currentWaveIndex++;
            StartWave();
        };
    }

    private int LastWaveIndex()
    {
        return levels[currentLevelIndex].waves.Count - 1;
    }

    void UpdateWaveUI()
    {
        waveText.text = "WAVE " + (currentWaveIndex + 1);
    }

    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }
}
