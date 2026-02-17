using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float spawnInterval = 0.5f;
    [SerializeField] private float spawnWidth = 8f;

    private bool spawning = false;

    public void StartSpawning()
    {
        spawning = true;
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
            float randomX = Random.Range(-spawnWidth, spawnWidth);
            Vector3 spawnPos = new Vector3(randomX, 7f, 0);

            Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
