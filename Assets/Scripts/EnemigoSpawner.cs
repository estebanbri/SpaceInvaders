using UnityEngine;

public class EnemigoSpawner : MonoBehaviour
{

    [SerializeField] private Enemigo enemigoPrefab;
    [SerializeField] private float minX = -7f;
    [SerializeField] private float maxX = 7f;
    [SerializeField] private float spawnY = 10f;

    public float startTimeInSec = 1f;
    public float repeatRateInSec = 2f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), startTimeInSec, repeatRateInSec);
    }

    void SpawnEnemy()
    {
        float randomX = Random.Range(minX, maxX);
        float clampedX = Mathf.Clamp(randomX, minX, maxX);
        Instantiate(enemigoPrefab, new Vector3(clampedX, spawnY, 0), Quaternion.identity);
    }

    public void CancelSpawnEnemy()
    {
        CancelInvoke(nameof(SpawnEnemy));
    }
}
