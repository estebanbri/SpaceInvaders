using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject asteroidPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float spawnWidth = 8f;
    [SerializeField] private float spawnY = 7f;

    [Header("Timing")]
    [SerializeField] private float baseSpawnRate = 0.5f;
    [SerializeField] private float minSpawnRate = 0.15f;
    [SerializeField] private float intensitySpeed = 0.5f;

    private bool spawning = false;
    private float intensityTime;

    public void StartSpawning()
    {
        spawning = true;
        intensityTime = Random.Range(0f, 100f);
        StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        spawning = false;
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawning)
        {
            // Intensidad variable usando Perlin
            float intensity = Mathf.PerlinNoise(intensityTime, 0f);
            intensityTime += Time.deltaTime * intensitySpeed;

            // Interpolamos el spawn rate
            float currentRate = Mathf.Lerp(baseSpawnRate, minSpawnRate, intensity);

            SpawnAsteroid();

            yield return new WaitForSeconds(currentRate);
        }
    }

    private void SpawnAsteroid()
    {
        float randomX = Random.Range(-spawnWidth, spawnWidth);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        // 15% chance de micro ráfaga
        if (Random.value < 0.15f)
        {
            float offsetX = Random.Range(-1.5f, 1.5f);
            Vector3 extraPos = new Vector3(randomX + offsetX, spawnY, 0f);
            Instantiate(asteroidPrefab, extraPos, Quaternion.identity);
        }
    }
}
