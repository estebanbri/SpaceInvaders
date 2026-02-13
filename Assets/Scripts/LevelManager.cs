using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Enemy Pools")]
    [SerializeField] private List<GameObject> tier1Enemies;
    [SerializeField] private List<GameObject> tier2Enemies;
    [SerializeField] private List<GameObject> tier3Enemies;

    [Header("Formation")]
    [SerializeField] private FormationController formationPrefab;
    [SerializeField] private int initialRowCount = 1;
    [SerializeField] private int initialColumnCount = 1;

    [Header("MiniBoss")]
    [SerializeField] private GameObject miniBossPrefab;
    [SerializeField] private BossHealthBarUI bossHealthBar;
    [SerializeField] private int wavesPerCycle = 5;

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
        Instance = this;
    }

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        int tier = GetTier();

        Debug.Log($"[WAVE START] Wave: {currentWave} | Tier: {tier}");

        UpdateWaveUI();

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

    bool IsMiniBossWave()
    {
        return currentWave % (wavesPerCycle + 1) == 0;
    }

    int GetTier()
    {
        return (currentWave - 1) / (wavesPerCycle + 1);
    }

    void SpawnProceduralFormation()
    {
        activeFormation = Instantiate(formationPrefab);

        int tier = GetTier();

        ProceduralWaveData waveData = GenerateWaveData(tier);

        activeFormation.InitializeProcedural(waveData);

        activeFormation.OnFormationCleared += () =>
        {
            currentWave++;
            StartWave();
        };
    }

    ProceduralWaveData GenerateWaveData(int tier)
    {
        ProceduralWaveData data = new ProceduralWaveData();

        if (tier == 0)
            data.enemyPrefabs = tier1Enemies;
        else if (tier == 1)
            data.enemyPrefabs = tier2Enemies;
        else
            data.enemyPrefabs = tier3Enemies;

        data.rows = Mathf.Clamp(initialRowCount + tier, initialRowCount, 8);
        data.columns = Mathf.Clamp(initialColumnCount + tier, initialColumnCount, 8);

        data.moveSpeed = 2f + tier * 0.3f;
        data.healthMultiplier = 1f + tier * 0.5f;
        data.fireRateMultiplier = 1f + tier * 0.2f;
        data.tier = tier;

        Debug.Log(
            $"[WAVE DATA] Tier: {tier} | Grid: {data.rows}x{data.columns} | " +
            $"MoveSpeed: {data.moveSpeed:F2} | " +
            $"HealthMult: {data.healthMultiplier:F2} | " +
            $"FireRateMultiplier: {data.fireRateMultiplier:F2}"
        );

        return data;
    }

    void SpawnMiniBoss()
    {
        GameObject bossGO = Instantiate(miniBossPrefab, new Vector3(0, 4f, 0), Quaternion.identity);

        Enemigo boss = bossGO.GetComponent<Enemigo>();

        int tier = GetTier();

        if (boss != null)
        {
            // Configura vida y arma según tier (FireRate dentro de WeaponInstance)
            boss.ConfigureByTier(tier);
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.Bind(boss);
        }

        boss.OnEnemyDied += () =>
        {
            if (bonusBoxSpawner != null)
            {
                bonusBoxSpawner.Spawn(new Vector3(0, 5f, 0));
            }

            currentWave++;
            StartWave();
        };
    }

    void UpdateWaveUI()
    {
        waveText.text = "WAVE " + currentWave;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}
