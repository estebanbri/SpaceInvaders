using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelDefinition> levels;

    private int currentLevelIndex = 0;
    private int currentWaveIndex = 0;

    private int enemiesAlive;

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        if (currentWaveIndex > levels[currentLevelIndex].waves.Count - 1) return;

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
        enemiesAlive = wave.enemiesToSpawn.Count;

        foreach (var enemyPrefab in wave.enemiesToSpawn)
        {
            Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
            yield return new WaitForSeconds(wave.spawnDelay);
        }
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
        return new Vector3(Random.Range(-6f, 6f), 7f, 0);
    }
}
