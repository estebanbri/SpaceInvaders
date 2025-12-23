using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<LevelDefinition> levels;

    private int currentLevelIndex = 0;
    private int currentWaveIndex = 0;
    private float currentSpawnDelay = 0;
    private float screenLimitMinX = -6f;
    private float screenLimitMaxX = 6f;

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
        foreach (var enemyPrefab in wave.enemiesToSpawn)
        {
            enemiesAlive = wave.enemiesToSpawn.Count;
            /*int randomEnemiesCount = Random.Range(1, 5);
            enemiesAlive += randomEnemiesCount;
            for (int i = 0; i < randomEnemiesCount; i++) {
                Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
                yield return new WaitForSeconds(wave.spawnDelay);
            }*/

            Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnDelay);
        }
    }

    public IEnumerator OnEnemyKilledByOutsideCamera(GameObject go)
    {
        Instantiate(go, GetSpawnPosition(), Quaternion.identity);
        yield return new WaitForSeconds(currentSpawnDelay);
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            currentWaveIndex++;
            StartWave();
        }
    }

    void SpawnBoss(GameObject bossPrefab)
    {
        Instantiate(bossPrefab, new Vector3(0, 7, 0), Quaternion.identity);
    }

    Vector3 GetSpawnPosition()
    {
        return new Vector3(Random.Range(screenLimitMinX, screenLimitMaxX), 7f, 0);
    }

    private int LastWaveIndex() {
        return levels[currentLevelIndex].waves.Count - 1;
    }
}
