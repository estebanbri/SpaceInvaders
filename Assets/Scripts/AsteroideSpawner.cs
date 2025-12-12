using UnityEngine;

public class AsteroideSpawner : MonoBehaviour
{
    
    [SerializeField] private Asteroide asteroidePrefab;
    [SerializeField] private float startTime = 1f;     // Initial delay before the first call
    [SerializeField] private float repeatRate = 3f;    // Time in seconds between subsequent calls

    [Header("Zona de Spawn")]
    [SerializeField] private float minX = -7.5f;
    [SerializeField] private float maxX = 7.5f;
    [SerializeField] private float spawnY = 6f; // Arriba de la cámara
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnAsteroide), startTime, repeatRate);
    }

    void SpawnAsteroide()
    {
        Instantiate(asteroidePrefab, new Vector2(Random.Range(minX, maxX), spawnY), Quaternion.identity);
    }

    void CancelSpawnBrickReaping()
    {
        CancelInvoke(nameof(SpawnAsteroide));
    }
}
