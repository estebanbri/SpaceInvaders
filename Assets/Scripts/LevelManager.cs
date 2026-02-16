using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Enemy Progression")]
    [SerializeField] private GameObject baseEnemy;
    [SerializeField] private List<GameObject> advancedEnemies;

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

    [Header("Grid Limits")]
    [SerializeField] private float horizontalSpacing = 1.5f;
    [SerializeField] private float verticalSpacing = 1.3f;

    private int currentWave = 1;
    private FormationController activeFormation;

    private float screenLimitMinX = -6f;
    private float screenLimitMaxX = 6f;
    private float spawnPositionY = 7f;

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

        if (activeFormation != null)
        {
            activeFormation.OnFormationCleared = null;
            Destroy(activeFormation.gameObject);
            activeFormation = null;
        }

        if (IsMiniBossWave())
            SpawnMiniBoss();
        else
            SpawnProceduralFormation();
    }

    private bool IsMiniBossWave()
    {
        return currentWave % (wavesPerCycle + 1) == 0;
    }

    private void SpawnProceduralFormation()
    {
        Vector3 formationPos = new Vector3(0, 4f, 0);
        activeFormation = Instantiate(formationPrefab, formationPos, Quaternion.identity);

        ProceduralWaveData waveData = GenerateWaveData();
        activeFormation.InitializeProcedural(waveData);

        activeFormation.OnFormationCleared += OnFormationCleared;
    }

    private void OnFormationCleared()
    {
        if (activeFormation != null)
            activeFormation.OnFormationCleared -= OnFormationCleared;

        currentWave++;
        StartWave();
    }

    private ProceduralWaveData GenerateWaveData()
    {
        ProceduralWaveData data = new ProceduralWaveData();
        data.enemyPrefabs = new List<GameObject>();

        int rows = 4;
        int columns = 7;
        int totalSlots = rows * columns;

        data.fixedRows = rows;
        data.fixedColumns = columns;
        data.spacingX = horizontalSpacing;
        data.spacingY = verticalSpacing;
        data.invertShape = currentWave % 2 == 0;

        int unlockedTypes = Mathf.Clamp((currentWave - 2) / 3 + 1, 0, advancedEnemies.Count);
        int specialCount = Mathf.Clamp(currentWave - 1, 0, totalSlots);

        Vector2 center = new Vector2(columns / 2f, rows / 2f);
        List<(int index, float distance)> positions = new List<(int, float)>();
        for (int i = 0; i < totalSlots; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float dist = Vector2.Distance(new Vector2(col, row), center);
            positions.Add((i, dist));
        }
        positions.Sort((a, b) => a.distance.CompareTo(b.distance));

        GameObject[] finalArray = new GameObject[totalSlots];
        for (int i = 0; i < totalSlots; i++)
            finalArray[i] = baseEnemy;

        for (int i = 0; i < specialCount && i < positions.Count; i++)
        {
            if (unlockedTypes <= 0) break;
            int posIndex = positions[i].index;
            int randomType = Random.Range(0, unlockedTypes);
            finalArray[posIndex] = advancedEnemies[randomType];
        }

        data.enemyPrefabs.AddRange(finalArray);
        data.moveSpeed = 2f + currentWave * 0.15f;
        data.healthMultiplier = 1f + currentWave * 0.1f;
        data.fireRateMultiplier = 1f + currentWave * 0.05f;
        data.cycle = currentWave;

        return data;
    }

    private void SpawnMiniBoss()
    {
        GameObject bossGO = Instantiate(miniBossPrefab, new Vector3(0, 4f, 0), Quaternion.identity);
        Enemigo boss = bossGO.GetComponent<Enemigo>();

        if (boss != null)
        {
            boss.ConfigureByCycle(currentWave);
            boss.OnEnemyDied += OnMiniBossDied;
        }

        if (bossHealthBar != null)
            bossHealthBar.Bind(boss);
    }

    private void OnMiniBossDied()
    {
        if (bonusBoxSpawner != null)
            bonusBoxSpawner.Spawn(GetBonusSpawnPosition());

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

    private Vector3 GetBonusSpawnPosition()
    {
        float x = Random.value < 0.5f ? screenLimitMinX : screenLimitMaxX;
        return new Vector3(x, spawnPositionY, 0f);
    }

    
}
