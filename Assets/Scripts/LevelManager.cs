using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private List<LevelDefinition> levels;

    private int currentLevelIndex = 0;
    private int currentWaveIndex = 0;

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

        foreach (var enemyPrefab in wave.enemiesToSpawn)
        {
            int randomEnemiesCount = Random.Range(1, 5);
            enemiesAlive += randomEnemiesCount;
            for (int i = 0; i < randomEnemiesCount; i++) {
                Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity);
                yield return new WaitForSeconds(wave.spawnDelay);
            }
            
        }
    }

    public void OnEnemyKilled()
    {
        Debug.Log("OnEnemyKilled");
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

    private int LastWaveIndex() {
        return levels[currentLevelIndex].waves.Count - 1;
    }
}
