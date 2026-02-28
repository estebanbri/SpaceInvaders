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

        for (int i = 0; i < spawnEvent.quantity; i++)
        {
            EnemigoSpawner.Instance.SpawnEnemy(
                spawnEvent.enemyPrefab,
                spawnEvent.path
            );

            yield return new WaitForSeconds(spawnEvent.spawnInterval);
        }

        activeSpawnEvents--;
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