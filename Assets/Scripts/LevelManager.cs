using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DestroyOutsideCamera;
using static HordeDefinition;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<LevelDefinition> levels;
    [SerializeField] private BossHealthBarUI bossHealthBar;

    [Header("Parallax")]
    [SerializeField] private VerticalParallax parallax;
    [SerializeField] private float turretSpacingY = 2f;

    [Header("Bonus")]
    [SerializeField] private BonusBoxSpawner bonusBoxSpawner;

    [SerializeField] private TextMeshProUGUI waveText;

    private int currentLevelIndex = 0;
    private int currentWaveIndex = 0;
    private float currentSpawnDelay = 0;

    private float screenLimitMinX = -6f;
    private float screenLimitMaxX = 6f;
    private float spawnPositionY = 7f;

    private int enemiesAlive;

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
        if (currentWaveIndex > LastWaveIndex()) return;

        WaveDefinition wave = levels[currentLevelIndex].waves[currentWaveIndex];

        StartCoroutine(SpawnWaveCoroutine(wave));
        UpdateWaveUI();
    }

    IEnumerator SpawnWaveCoroutine(WaveDefinition wave)
    {
        currentSpawnDelay = wave.spawnDelay;
        enemiesAlive = 0;

        int hordeNumber = 1;

        foreach (var horde in wave.hordes)
        {
            Debug.Log("Iniciando horda " + hordeNumber + " con " + horde.hordeCount + " enemigos.");

            Vector3 basePosition = GetSpawnPosition();

            // ===============================
            // FORMACION CUSTOM
            // ===============================
            if (horde.useCustomFormation &&
                horde.customFormationPoints != null &&
                horde.customFormationPoints.Count > 0)
            {
                for (int i = 0; i < horde.customFormationPoints.Count; i++)
                {
                    Vector2 offset = horde.customFormationPoints[i].offset;

                    Vector3 spawnPos = basePosition + new Vector3(offset.x, offset.y, 0);

                    SpawnEnemyAtPosition(
                        horde.enemyPrefab,
                        horde.movementPattern,
                        spawnPos
                    );

                    enemiesAlive++;
                }
            }
            else
            {
                // ===============================
                // FORMACION LINEAL (fallback)
                // ===============================
                for (int i = 0; i < horde.hordeCount; i++)
                {
                    Vector3 spawnPos = GetFormationPosition(
                        basePosition,
                        i,
                        horde.hordeCount,
                        horde.formationType,
                        horde.formationOffset
                    );

                    SpawnEnemyAtPosition(
                        horde.enemyPrefab,
                        horde.movementPattern,
                        spawnPos
                    );

                    enemiesAlive++;
                }
            }

            yield return new WaitForSeconds(wave.spawnDelay);
            hordeNumber++;
        }

        SpawnWaveTurrets(wave);
    }

    public void OnEnemyKilled()
    {
        WaveDefinition wave = levels[currentLevelIndex].waves[currentWaveIndex];
        if (wave.isBossWave)
        {
            if (bonusBoxSpawner != null)
            {
                Vector3 bonusPos = GetBonusSpawnPosition();
                bonusBoxSpawner.Spawn(bonusPos);
            }

            currentWaveIndex++;
            StartWave();
            return;
        }

        enemiesAlive--;
        Debug.Log("Enemigos vivos actualizado: " + enemiesAlive);
        if (enemiesAlive <= 0)
        {
            currentWaveIndex++;
            StartWave();
        }
    }

    void SpawnBoss(GameObject bossPrefab)
    {
       

        Enemigo boss = bossPrefab.GetComponent<Enemigo>();

        if (boss != null && bossHealthBar != null)
        {
            bossHealthBar.Bind(boss);
        }
    }

    Vector3 GetSpawnPosition()
    {
        return new Vector3(
            Random.Range(screenLimitMinX, screenLimitMaxX),
            spawnPositionY,
            0
        );
    }

    private Vector3 GetBonusSpawnPosition()
    {
        float x = Random.value < 0.5f ? screenLimitMinX : screenLimitMaxX;
        return new Vector3(x, spawnPositionY, 0f);
    }

    private int LastWaveIndex()
    {
        return levels[currentLevelIndex].waves.Count - 1;
    }

    void SpawnWaveTurrets(WaveDefinition wave)
    {
        float centralX = (screenLimitMinX + screenLimitMaxX) / 2f;

        for (int i = 0; i < wave.turretsToSpawn; i++)
        {
            GameObject turretPrefab =
                wave.turretPrefabs[Random.Range(0, wave.turretPrefabs.Count)];

            Vector3 spawnPos = new Vector3(
                centralX,
                spawnPositionY + i * turretSpacingY,
                0f
            );

            GameObject turret = Instantiate(turretPrefab, spawnPos, Quaternion.identity);
            turret.transform.SetParent(parallax.transform, true);
        }
    }

    void UpdateWaveUI()
    {
        waveText.text = "WAVE  " + (currentWaveIndex + 1);
    }

    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }

    private Vector3 GetFormationPosition(
    Vector3 basePos,
    int index,
    int total,
    HordeFormationType type,
    float offset)
    {
        // Centramos la formación
        float centerOffset = (total - 1) * 0.5f;

        switch (type)
        {
            case HordeFormationType.Horizontal:
                return basePos + new Vector3(
                    (index - centerOffset) * offset,
                    0,
                    0
                );

            case HordeFormationType.Vertical:
                return basePos + new Vector3(
                    0,
                    (index - centerOffset) * offset,
                    0
                );

            case HordeFormationType.Diagonal:
                return basePos + new Vector3(
                    (index - centerOffset) * offset,
                    (index - centerOffset) * offset,
                    0
                );

            default:
                return basePos;
        }
    }

    private void SpawnEnemyAtPosition(
    GameObject prefab,
    MovementPatternDefinition pattern,
    Vector3 position)
    {
        GameObject enemyGO = Instantiate(prefab, position, Quaternion.identity);

        if (enemyGO.TryGetComponent<MovementController>(out var mc))
        {
            mc.SetPattern(pattern);
        }

        if (enemyGO.TryGetComponent<Enemigo>(out var enemy))
        {
            enemy.prefab = prefab;
            enemy.movementPattern = pattern;
        }
    }
}
