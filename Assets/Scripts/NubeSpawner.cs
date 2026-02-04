using UnityEngine;

public class NubeSpawner : MonoBehaviour
{
    [SerializeField] private Nube nubePrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float spawnXRange = 8f;
    [SerializeField] private int maxScale = 5;

    private float nubeTime;

    void Update()
    {
        nubeTime += Time.deltaTime;

        if (nubeTime >= spawnInterval)
        {
            float randomX = Random.Range(-spawnXRange, spawnXRange);
            Vector3 spawnPos = new Vector3(randomX, transform.position.y, 0f);

            Nube nube = Instantiate(nubePrefab, spawnPos, Quaternion.identity);

            int scale = Random.Range(1, maxScale);
            nube.transform.localScale = Vector3.one * scale;

            float zRotation = Random.value > 0.5f ? 180f : 0f;
            nube.transform.rotation = Quaternion.Euler(0f, 0f, zRotation);

            nubeTime = 0f;
        }
    }
}
