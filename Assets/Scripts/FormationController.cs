using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    [Header("Formation Settings")]
    [SerializeField] private int fixedRows = 4;
    [SerializeField] private int fixedColumns = 7;
    [SerializeField] private float spacingX = 1.5f;
    [SerializeField] private float spacingY = 1.3f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Subgroup Settings")]
    [SerializeField] private int enemiesPerSubgroup = 3;
    [SerializeField] private float maxSubgroupDelay = 1.5f;
    [SerializeField] private float entryHeight = 6f;
    [SerializeField] private int totalEnemiesPerWave = 30;

    [Header("Movement & Organic")]
    [SerializeField] private float stepDown = 0.5f;
    [SerializeField] private float organicAmplitude = 0.15f;
    [SerializeField] private float organicSpeed = 2f;

    [Header("Random Offset")]
    [SerializeField] private float maxOffsetX = 0.5f;
    [SerializeField] private float maxOffsetY = 0.3f;

    private List<GameObject> enemyPrefabs;
    private List<SpriteRenderer> cachedRenderers = new List<SpriteRenderer>();
    private List<Transform> enemyTransforms = new List<Transform>();
    private List<Vector3> baseLocalPositions = new List<Vector3>();

    private int direction = 1;
    private float leftLimit;
    private float rightLimit;
    private bool isActive = false;
    private int enemiesKilled;
    private int enemiesAlive;
    private int totalEnemiesInWave;

    private bool[,] occupiedPositions;

    public System.Action OnFormationCleared;

    private bool allEnemiesSpawned = false;

    void Update()
    {
        if (!isActive || enemiesAlive <= 0) return;

        // No esta funcionado.
        // Move();
        ApplyOrganicMovement();
    }
    void ApplyOrganicMovement()
    {
        for (int i = 0; i < enemyTransforms.Count; i++) { 
            if (enemyTransforms[i] == null) continue; 
            EntryAnimation anim = enemyTransforms[i].GetComponent<EntryAnimation>(); 
            if (anim != null && !anim.HasFinished) continue; 
            float waveX = Mathf.Sin(Time.time * organicSpeed + i * 0.4f) * organicAmplitude; 
            float waveY = Mathf.Cos(Time.time * organicSpeed * 0.8f + i * 0.3f) * (organicAmplitude * 0.5f); 
            enemyTransforms[i].localPosition = baseLocalPositions[i] + new Vector3(waveX, waveY, 0); 
        }
    }

    public void InitializeProcedural(ProceduralWaveData data)
    {
        enemyPrefabs = data.enemyPrefabs;

        direction = 1;
        isActive = false;

        cachedRenderers.Clear();
        enemyTransforms.Clear();
        baseLocalPositions.Clear();

        CalculateBounds();

        // Inicializamos la grilla de posiciones ocupadas
        occupiedPositions = new bool[fixedRows, fixedColumns];
        totalEnemiesInWave = totalEnemiesPerWave;

        StartCoroutine(SpawnSubgroups());
    }

    void CalculateBounds()
    {
        Camera cam = Camera.main;
        float screenHeight = 2f * cam.orthographicSize;
        float screenWidth = screenHeight * cam.aspect;

        leftLimit = -screenWidth / 2f + 0.5f;
        rightLimit = screenWidth / 2f - 0.5f;
    }


    IEnumerator SpawnSubgroups()
    {
        int index = 0;

        while (index < totalEnemiesPerWave)
        {
            int count = Mathf.Min(enemiesPerSubgroup, totalEnemiesPerWave - index);
            List<Transform> currentSubgroup = new List<Transform>();

            Vector2Int seedPos = GetRandomFreePosition();
            occupiedPositions[seedPos.y, seedPos.x] = true;

            List<Vector2Int> availablePositions = GetNeighborFreePositions(seedPos);
            availablePositions.Insert(0, seedPos);

            for (int i = 0; i < count; i++)
            {
                Vector2Int pos;
                if (availablePositions.Count > 0)
                {
                    pos = availablePositions[0];
                    availablePositions.RemoveAt(0);
                }
                else pos = GetRandomFreePosition();

                occupiedPositions[pos.y, pos.x] = true;

                Vector3 targetPos = new Vector3(
                    -(fixedColumns - 1) * spacingX * 0.5f + pos.x * spacingX,
                    -pos.y * spacingY,
                    0
                );

                float offsetX = Random.Range(-maxOffsetX * 0.3f, maxOffsetX * 0.3f);
                float offsetY = Random.Range(-maxOffsetY * 0.3f, maxOffsetY * 0.3f);
                targetPos += new Vector3(offsetX, offsetY, 0);

                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                GameObject enemyGO = Instantiate(prefab, transform);
                enemyGO.transform.localPosition = targetPos + Vector3.up * entryHeight;

                SpriteRenderer sr = enemyGO.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) cachedRenderers.Add(sr);

                Enemigo enemigo = enemyGO.GetComponent<Enemigo>();
                enemigo.SetFormation(this);

                EntryAnimation anim = enemyGO.AddComponent<EntryAnimation>();
                anim.Initialize(enemyGO.transform.position, transform.position + targetPos, 1.5f);

                enemyTransforms.Add(enemyGO.transform);
                baseLocalPositions.Add(targetPos);
                currentSubgroup.Add(enemyGO.transform);

                index++;
                enemiesAlive++;
            }

            // Esperamos que el subgrupo llegue a su posición (solo animación visual)
            bool subgroupDone = false;
            while (!subgroupDone)
            {
                subgroupDone = true;
                foreach (var t in currentSubgroup)
                {
                    if (t == null) continue;
                    EntryAnimation e = t.GetComponent<EntryAnimation>();
                    if (e != null && !e.HasFinished) subgroupDone = false;
                }
                yield return null;
            }

            float randomDelay = Random.Range(0f, maxSubgroupDelay);
            yield return new WaitForSeconds(randomDelay);
        }

        // 🔹 Todos los enemigos ya han sido generados
        allEnemiesSpawned = true;
        isActive = true;
    }


    /// <summary>
    /// Obtiene las posiciones vecinas libres de la semilla (arriba, abajo, izquierda, derecha)
    /// </summary>
    List<Vector2Int> GetNeighborFreePositions(Vector2Int seed)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = seed.x + dx[i];
            int ny = seed.y + dy[i];

            if (nx >= 0 && nx < fixedColumns && ny >= 0 && ny < fixedRows)
            {
                if (!occupiedPositions[ny, nx])
                    neighbors.Add(new Vector2Int(nx, ny));
            }
        }

        return neighbors;
    }


    Vector2Int GetRandomFreePosition()
    {
        List<Vector2Int> freePositions = new List<Vector2Int>();
        for (int y = 0; y < fixedRows; y++)
        {
            for (int x = 0; x < fixedColumns; x++)
            {
                if (!occupiedPositions[y, x])
                    freePositions.Add(new Vector2Int(x, y));
            }
        }

        if (freePositions.Count == 0) return new Vector2Int(0, 0);
        return freePositions[Random.Range(0, freePositions.Count)];
    }
    
    void Move()
    {
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        GetFormationHorizontalBounds(out float leftEdge, out float rightEdge);

        if ((rightEdge >= rightLimit && direction > 0) ||
            (leftEdge <= leftLimit && direction < 0))
        {
            direction *= -1;
            transform.position += Vector3.down * stepDown;
        }
    }

    private void GetFormationHorizontalBounds(out float leftEdge, out float rightEdge)
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        for (int i = cachedRenderers.Count - 1; i >= 0; i--)
        {
            if (cachedRenderers[i] == null)
            {
                cachedRenderers.RemoveAt(i);
                continue;
            }

            Bounds b = cachedRenderers[i].bounds;
            if (b.min.x < minX) minX = b.min.x;
            if (b.max.x > maxX) maxX = b.max.x;
        }

        leftEdge = minX;
        rightEdge = maxX;
    }


    public void NotifyEnemyKilled()
    {
        // Reducimos la cantidad de enemigos vivos en pantalla
        enemiesAlive--;

        // Aumentamos el contador de enemigos muertos de la wave total
        enemiesKilled++;

        // Calculamos enemigos restantes de la wave
        int remainingEnemies = totalEnemiesPerWave - enemiesKilled;

        // Log para ver la wave completa
        Debug.Log($"[FormationController] Enemigos restantes de la wave: {remainingEnemies} / {totalEnemiesPerWave}");

        // Solo destruimos la wave cuando todos los enemigos fueron generados y no queda ninguno vivo
        if (allEnemiesSpawned && enemiesAlive <= 0)
        {
            Debug.Log("[FormationController] ¡Wave completada!");
            OnFormationCleared?.Invoke();
            Destroy(gameObject);
        }
    }

}
