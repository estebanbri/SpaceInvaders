using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    private List<GameObject> enemyPrefabs;

    private int rows;
    private int columns;
    private float spacingX;
    private float spacingY;

    [Header("Movement")]
    [SerializeField] private float stepDown = 0.5f;
    [SerializeField] private float borderPadding = 0.5f;

    private float moveSpeed;

    private int direction = 1;
    private float leftLimit;
    private float rightLimit;
    private bool isActive = false;
    private int enemiesAlive;

    public void Initialize(WaveDefinition wave, int waveIndex)
    {
        enemyPrefabs = wave.enemyPrefabs;

        rows = wave.rows;
        columns = wave.columns;
        spacingX = wave.spacingX;
        spacingY = wave.spacingY;
        moveSpeed = wave.baseSpeed;

        ConfigureDifficulty(waveIndex);
        CalculateBounds();
        GenerateGrid();
        StartCoroutine(EnterAnimation());
    }

    void ConfigureDifficulty(int waveIndex)
    {
        moveSpeed += waveIndex * 0.3f;
        rows += waveIndex / 3;
    }

    void CalculateBounds()
    {
        Camera cam = Camera.main;
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        leftLimit = min.x + borderPadding;
        rightLimit = max.x - borderPadding;
    }

    void GenerateGrid()
    {
        enemiesAlive = 0;

        float startX = -(columns - 1) * spacingX * 0.5f;
        float startY = 0f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 localPos = new Vector3(
                    startX + c * spacingX,
                    startY - r * spacingY,
                    0f
                );

                GameObject prefab = GetRandomEnemyPrefab();

                GameObject enemyGO = Instantiate(prefab, transform);
                enemyGO.transform.localPosition = localPos;

                Enemigo enemigo = enemyGO.GetComponent<Enemigo>();
                enemigo.SetFormation(this);

                enemiesAlive++;
            }
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("No enemy prefabs assigned in WaveDefinition");
            return null;
        }

        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }

    IEnumerator EnterAnimation()
    {
        Vector3 start = new Vector3(0, 8f, 0);
        Vector3 target = new Vector3(0, 3f, 0);

        transform.position = start;

        float t = 0;
        float duration = 1.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }

        transform.position = target;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;
        Move();
    }

    void Move()
    {
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        float halfWidth = (columns - 1) * spacingX * 0.5f;
        float leftEdge = transform.position.x - halfWidth;
        float rightEdge = transform.position.x + halfWidth;

        if (rightEdge >= rightLimit && direction > 0)
            StepDown();
        else if (leftEdge <= leftLimit && direction < 0)
            StepDown();
    }

    void StepDown()
    {
        direction *= -1;
        transform.position += Vector3.down * stepDown;
    }

    public void NotifyEnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            LevelManager.Instance.OnFormationCleared();
            Destroy(gameObject);
        }
    }

    public void SetGrid(int r, int c, float sx, float sy, float speed)
    {
        rows = r;
        columns = c;
        spacingX = sx;
        spacingY = sy;
        moveSpeed = speed;
    }
}
