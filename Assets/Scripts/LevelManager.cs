using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Tiers")]
    [SerializeField] private List<TierConfig> tiers;

    [Header("Formation")]
    [SerializeField] private FormationController formationPrefab;

    [Header("MiniBoss")]
    [SerializeField] private GameObject miniBossPrefab;
    [SerializeField] private BossHealthBarUI bossHealthBar;
    [SerializeField] private int wavesPerCycle = 3;

    [Header("Parallax")]
    [SerializeField] private VerticalParallax parallax;

    [Header("Bonus")]
    [SerializeField] private BonusBoxSpawner bonusBoxSpawner;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;

    private int currentWave = 1;
    private FormationController activeFormation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartWave();
    }

    private void StartWave()
    {
        UpdateWaveUI();

        int tier = GetTier();
        Debug.Log($"[WAVE START] Wave: {currentWave} | Tier: {tier}");

        if (IsMiniBossWave())
        {
            Debug.Log($"[MINIBOSS SPAWN] Tier: {tier}");
            SpawnMiniBoss();
        }
        else
        {
            SpawnProceduralFormation();
        }
    }

    private bool IsMiniBossWave()
    {
        return currentWave % (wavesPerCycle + 1) == 0;
    }

    private int GetTier()
    {
        return (currentWave - 1) % tiers.Count;
    }

    private int GetCycle()
    {
        return (currentWave - 1) / (wavesPerCycle + 1);
    }

    private void SpawnProceduralFormation()
    {
        // Limpieza de la formación anterior
        if (activeFormation != null)
        {
            activeFormation.OnFormationCleared = null;
            Destroy(activeFormation.gameObject);
        }
        Vector3 spawnPosition = new Vector3(0, 3.5f, 0); // spawn posicion de la wave
        activeFormation = Instantiate(formationPrefab, spawnPosition, Quaternion.identity);

        int tier = GetTier();
        ProceduralWaveData waveData = GenerateWaveData(tier);

        activeFormation.InitializeProcedural(waveData);

        // Suscripción segura al evento
        activeFormation.OnFormationCleared += OnFormationCleared;
    }

    private void OnFormationCleared()
    {
        // Limpieza del evento antes de avanzar
        if (activeFormation != null)
            activeFormation.OnFormationCleared -= OnFormationCleared;

        currentWave++;
        StartWave();
    }

    private ProceduralWaveData GenerateWaveData(int tier)
    {
        ProceduralWaveData data = new ProceduralWaveData();
        TierConfig config = tiers[tier];

        data.enemyPrefabs = new List<GameObject>();
        int cycle = GetCycle();  //  obtenemos el ciclo actual

        foreach (var enemyCount in config.enemies)
        {
            int count = enemyCount.baseCount + cycle;  //  escalado dinámico

            for (int i = 0; i < count; i++)
            {
                data.enemyPrefabs.Add(enemyCount.enemyPrefab);
            }
        }

        // Mezclar la lista
        for (int i = 0; i < data.enemyPrefabs.Count; i++)
        {
            int swapIndex = Random.Range(i, data.enemyPrefabs.Count);
            var temp = data.enemyPrefabs[i];
            data.enemyPrefabs[i] = data.enemyPrefabs[swapIndex];
            data.enemyPrefabs[swapIndex] = temp;
        }

        data.moveSpeed = 2f + tier * 0.3f;
        data.healthMultiplier = 1f + tier * 0.5f;
        data.fireRateMultiplier = 1f + tier * 0.2f;
        data.tier = tier;
        data.formationPattern = config.formationPattern;

        Debug.Log(
            $"[WAVE DATA] Tier: {tier} | TotalEnemies: {data.enemyPrefabs.Count} | " +
            $"MoveSpeed: {data.moveSpeed:F2} | HealthMult: {data.healthMultiplier:F2} | FireRateMultiplier: {data.fireRateMultiplier:F2} | Pattern: {data.formationPattern}"
        );

        return data;
    }

    private void SpawnMiniBoss()
    {
        GameObject bossGO = Instantiate(miniBossPrefab, new Vector3(0, 4f, 0), Quaternion.identity);
        Enemigo boss = bossGO.GetComponent<Enemigo>();
        int tier = GetTier();

        if (boss != null)
        {
            boss.ConfigureByTier(tier);
            boss.OnEnemyDied += OnMiniBossDied;
        }

        if (bossHealthBar != null)
            bossHealthBar.Bind(boss);
    }

    private void OnMiniBossDied()
    {
        if (bonusBoxSpawner != null)
            bonusBoxSpawner.Spawn(new Vector3(0, 5f, 0));

        currentWave++;
        StartWave();
    }

    private void UpdateWaveUI()
    {
        waveText.text = "WAVE " + currentWave;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}
