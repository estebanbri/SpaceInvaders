using System;
using System.Collections;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public LevelData levelData;

    private float levelTimer;
    private bool levelRunning;
    private bool timelineFinished;
    private bool bossSpawned;

    private int activeSpawnEvents;
    private Action onLevelCompleted;

    [Header("Spawn Safe Area (Viewport %)")]
    [SerializeField, Range(0f, 0.4f)] private float horizontalSafePercent = 0.12f;
    [SerializeField, Range(0f, 0.4f)] private float verticalSafePercent = 0.08f;

    private void Update()
    {
        if (!levelRunning)
            return;

        levelTimer += Time.deltaTime;

        CheckSpawnEvents();
        CheckBossCondition();
    }

    // =====================================
    // PUBLIC API
    // =====================================

    public void StartLevel(Action onCompleted)
    {
        onLevelCompleted = onCompleted;
        ResetLevel();
        levelRunning = true;
    }

    public void StopLevel()
    {
        levelRunning = false;
        StopAllCoroutines();
    }

    // =====================================
    // TIMELINE SYSTEM
    // =====================================

    private void CheckSpawnEvents()
    {
        bool allStarted = true;

        foreach (var spawnEvent in levelData.spawnEvents)
        {
            if (!spawnEvent.hasStarted)
            {
                allStarted = false;

                if (levelTimer >= spawnEvent.startTime)
                {
                    spawnEvent.hasStarted = true;
                    StartCoroutine(ExecuteSpawnEvent(spawnEvent));
                }
            }
        }

        if (allStarted && activeSpawnEvents == 0)
            timelineFinished = true;
    }

    private IEnumerator ExecuteSpawnEvent(SpawnEvent spawnEvent)
    {
        activeSpawnEvents++;

        PathComponent selectedPath = GetRandomPath(spawnEvent);
        Vector2 formationOffset = CalculateFormationOffset(selectedPath);
        bool reverse = IsReversePathDirection(spawnEvent.pathDirection);
        MovementModifier modifier = GetFormationModifier();

        for (int i = 0; i < spawnEvent.quantity; i++)
        {
            EnemigoSpawner.Instance.SpawnEnemy(
                spawnEvent.enemyPrefab,
                selectedPath,
                formationOffset,
                reverse, 
                modifier
            );

            yield return new WaitForSeconds(spawnEvent.spawnInterval);
        }

        activeSpawnEvents--;
    }

    private MovementModifier GetFormationModifier()
    {
        int r = UnityEngine.Random.Range(0, 100);

        if (r < 60)
            return MovementModifier.None;
        else if (r < 85)
            return MovementModifier.Sine;
        else if (r < 95)
            return MovementModifier.ZigZag;
        else
            return MovementModifier.BurstOffset;
    }

    private PathComponent GetRandomPath(SpawnEvent spawnEvent)
    {
        if (spawnEvent.possiblePaths == null || spawnEvent.possiblePaths.Count == 0)
        {
            Debug.LogError("No paths assigned in SpawnEvent!");
            return null;
        }

        int index = UnityEngine.Random.Range(0, spawnEvent.possiblePaths.Count);
        return spawnEvent.possiblePaths[index];
    }

    private bool IsReversePathDirection(PathDirection direction)
    {
        bool reverse = false;

        if (direction == PathDirection.Reverse)
            reverse = true;

        else if (direction == PathDirection.Random)
            reverse = UnityEngine.Random.value > 0.5f;

        return reverse;
    }

    private Vector2 CalculateFormationOffset(PathComponent path)
    {
        Camera cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Bounds pathBounds = path.GetBounds();

        Vector2 offset = Vector2.zero;

        // Safe zone en unidades del mundo
        float safeX = halfWidth * horizontalSafePercent;
        float safeY = halfHeight * verticalSafePercent;

        if (path.orientation == PathOrientation.Horizontal)
        {
            float pathTop = pathBounds.max.y;
            float pathBottom = pathBounds.min.y;

            float minOffset = (-halfHeight + safeY) - pathBottom;
            float maxOffset = (halfHeight - safeY) - pathTop;

            offset.y = UnityEngine.Random.Range(minOffset, maxOffset);
        }
        else if (path.orientation == PathOrientation.Vertical)
        {
            float pathRight = pathBounds.max.x;
            float pathLeft = pathBounds.min.x;

            float minOffset = (-halfWidth + safeX) - pathLeft;
            float maxOffset = (halfWidth - safeX) - pathRight;

            offset.x = UnityEngine.Random.Range(minOffset, maxOffset);
        }

        return offset;
    }

    // =====================================
    // BOSS LOGIC
    // =====================================

    private void CheckBossCondition()
    {
        if (bossSpawned)
            return;

        if (timelineFinished &&
            activeSpawnEvents == 0 &&
            EnemigoManager.Instance.ActiveEnemies == 0)
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        bossSpawned = true;
        Instantiate(levelData.bossPrefab);
    }

    public void CompleteLevel()
    {
        levelRunning = false;
        onLevelCompleted?.Invoke();
    }

    // =====================================
    // RESET
    // =====================================

    private void ResetLevel()
    {
        levelTimer = 0f;
        timelineFinished = false;
        bossSpawned = false;
        activeSpawnEvents = 0;

        foreach (var spawnEvent in levelData.spawnEvents)
            spawnEvent.hasStarted = false;
    }
}