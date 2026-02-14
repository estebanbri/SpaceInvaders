using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Waves")]
    [SerializeField] private List<WaveConfig> waveConfig;

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
    private FormationPattern lastFormation;
    private bool hasLastFormation = false;
    private bool miniBossDead = false;
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

        // Limpiar formación activa si existe
        if (activeFormation != null)
        {
            Debug.Log("[LEVEL MANAGER] Destroying previous formation before starting wave.");
            activeFormation.OnFormationCleared = null;
            Destroy(activeFormation.gameObject);
            activeFormation = null;
        }

        int tier = GetWave();
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

    private int GetWave()
    {
        return (currentWave - 1) % waveConfig.Count;
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

        int wave = GetWave();
        ProceduralWaveData waveData = GenerateWaveData(wave);

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

    private ProceduralWaveData GenerateWaveData(int waveIndex)
    {
        ProceduralWaveData data = new ProceduralWaveData();
        WaveConfig config = waveConfig[waveIndex];

        data.enemyPrefabs = new List<GameObject>();
        int cycle = GetCycle();  //  obtenemos el ciclo actual

        foreach (var enemyCount in config.enemies)
        {
            int count = enemyCount.baseCount + Mathf.FloorToInt(cycle * 1.5f);  //  escalado dinámico

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

        data.moveSpeed = 2f + cycle * 0.3f;
        data.healthMultiplier = 1f + cycle * 0.5f;
        data.fireRateMultiplier = 1f + cycle * 0.2f;
        data.cycle = cycle;
        data.formationPattern = GetRandomFormation();

        Debug.Log(
            $"[WAVE DATA] Wave: {waveIndex} | TotalEnemies: {data.enemyPrefabs.Count} | " +
            $"MoveSpeed: {data.moveSpeed:F2} | HealthMult: {data.healthMultiplier:F2} | FireRateMultiplier: {data.fireRateMultiplier:F2} | Pattern: {data.formationPattern}"
        );

        return data;
    }

    private FormationPattern GetRandomFormation()
    {
        FormationPattern[] allPatterns =
            (FormationPattern[])System.Enum.GetValues(typeof(FormationPattern));

        if (allPatterns.Length == 0)
            return FormationPattern.Grid;

        FormationPattern selected;

        if (allPatterns.Length == 1)
        {
            selected = allPatterns[0];
        }
        else
        {
            do
            {
                int index = Random.Range(0, allPatterns.Length);
                selected = allPatterns[index];
            }
            while (hasLastFormation && selected == lastFormation);
        }

        lastFormation = selected;
        hasLastFormation = true;

        return selected;
    }



    private void SpawnMiniBoss()
    {
        miniBossDead = false; // reset al spawn

        GameObject bossGO = Instantiate(miniBossPrefab, new Vector3(0, 4f, 0), Quaternion.identity);
        Enemigo boss = bossGO.GetComponent<Enemigo>();
        int cycle = GetCycle();

        if (boss != null)
        {
            boss.ConfigureByCycle(cycle);

            // Prevención de doble trigger
            boss.OnEnemyDied -= OnMiniBossDied;
            boss.OnEnemyDied += OnMiniBossDied;
        }

        if (bossHealthBar != null)
            bossHealthBar.Bind(boss);

        Debug.Log($"[MINIBOSS SPAWNED] Cycle: {cycle}");
    }

    private void OnMiniBossDied()
    {
        if (miniBossDead) return;
        miniBossDead = true;

        Debug.Log("[MINIBOSS] Died. Cleaning up before next wave.");

        // Limpieza formación activa si queda alguna (previene caída infinita)
        if (activeFormation != null)
        {
            activeFormation.OnFormationCleared = null;
            Destroy(activeFormation.gameObject);
            activeFormation = null;
        }

        // Si tienes bonus, spawn pero comentado por debug
        // if (bonusBoxSpawner != null)
        //     bonusBoxSpawner.Spawn(new Vector3(0, 5f, 0));

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
