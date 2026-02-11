using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DestroyOutsideCamera;

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
    [SerializeField] private bool spawnBonusBetweenWaves = true;

    [Header("Bonus")]
    [Range(0f, 1f)]
    public float bonusSpawnChance = 1f; // 1 = siempre, 0 = nunca

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

        if (wave.isBossWave)
        {
            SpawnBoss(wave.bossPrefab);
            return;
        }

        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    IEnumerator SpawnWaveCoroutine(WaveDefinition wave)
    {
        currentSpawnDelay = wave.spawnDelay;
        enemiesAlive = wave.enemiesToSpawn.Count;
        Debug.Log("Enemigos vivos total: " + enemiesAlive);

        foreach (var enemyPrefab in wave.enemiesToSpawn)
        {
            Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        SpawnWaveTurrets(wave);

    }

    public void OnEnemyExitedCamera(GameObject enemy)
    {
        if (!enemy.TryGetComponent(out DestroyOutsideCamera doc))
            return;

        if (doc.GetBehavior() == OutsideCameraBehavior.DestroyAndRespawn)
        {
            RespawnEnemy(enemy);
        }
    }

    public void RespawnEnemy(GameObject go)
    {
        StartCoroutine(RespawnEnemyCoroutine(go));
    }

    public IEnumerator RespawnEnemyCoroutine(GameObject go)
    {
        Instantiate(go, GetSpawnPosition(), Quaternion.identity);
        yield return new WaitForSeconds(currentSpawnDelay);
    }

    private HashSet<int> bonusSpawnedWaves = new HashSet<int>();

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        Debug.Log("Enemigos vivos actualizado: " + enemiesAlive);

        if (enemiesAlive <= 0)
        {
            WaveDefinition wave = levels[currentLevelIndex].waves[currentWaveIndex];

            // Verificar si la siguiente wave es un boss
            bool nextWaveIsBoss = false;
            if (currentWaveIndex + 1 <= LastWaveIndex())
            {
                nextWaveIsBoss = levels[currentLevelIndex].waves[currentWaveIndex + 1].isBossWave;
            }

            // Spawn de caja de bonus entre waves, solo si no es la primera wave,
            // y solo si no se spawneo aún para esta wave
            if (spawnBonusBetweenWaves
            && bonusBoxSpawner != null
            && currentWaveIndex > 0
            && !wave.isBossWave
            && !nextWaveIsBoss
            && !bonusSpawnedWaves.Contains(currentWaveIndex))
            {
                if (!bonusSpawnedWaves.Contains(currentWaveIndex))
                {
                    // Probabilidad de spawn
                    if (Random.value <= bonusSpawnChance)
                    {
                        Vector3 bonusPos = GetBonusSpawnPosition();
                        bonusBoxSpawner.Spawn(bonusPos);
                    }

                    bonusSpawnedWaves.Add(currentWaveIndex); // marca la wave como spawneada
                }
            }

            // Avanzar a la siguiente wave
            currentWaveIndex++;
            StartWave();
        }
    }

    void SpawnBoss(GameObject bossPrefab)
    {
        GameObject bossGO = Instantiate(bossPrefab, new Vector3(0, spawnPositionY, 0), Quaternion.identity);

        Enemigo boss = bossGO.GetComponent<Enemigo>();
        if (boss != null && bossHealthBar != null)
        {
            bossHealthBar.Bind(boss);
        }
    }

    Vector3 GetSpawnPosition()
    {
        return new Vector3(Random.Range(screenLimitMinX, screenLimitMaxX), spawnPositionY, 0);
    }

    private Vector3 GetBonusSpawnPosition()
    {
        float y = spawnPositionY;

        // Elegir aleatoriamente izquierda (-X) o derecha (+X)
        float x = Random.value < 0.5f ? screenLimitMinX : screenLimitMaxX;

        return new Vector3(x, y, 0f);
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
}
