using Unity.VisualScripting;
using UnityEngine;

public class EstrellaSpawner : MonoBehaviour
{
    [Header("Estrella Prefabs")]
    [SerializeField] private Estrella smallEstrellaPrefab;
    [SerializeField] private Estrella mediumEstrellaPrefab;
    [SerializeField] private Estrella largeEstrellaPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 0.1f;
    [SerializeField] private float spawnMinX = -8f;
    [SerializeField] private float spawnMaxX = 8f;
    [SerializeField] private float spawnY = 6f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnStar();
            timer = 0f;
        }
    }

    void SpawnStar()
    {
        Estrella prefabToSpawn = GetRandomStarPrefab();

        float randomX = Random.Range(spawnMinX, spawnMaxX);
        float randomZ = Random.Range(0f, 2f);

        Instantiate(prefabToSpawn, new Vector3(randomX, spawnY, randomZ), Quaternion.identity);
    }

    Estrella GetRandomStarPrefab()
    {
        int roll = Random.Range(0, 3);

        return roll switch
        {
            0 => smallEstrellaPrefab,
            1 => mediumEstrellaPrefab,
            _ => largeEstrellaPrefab
        };
    }
}
