using UnityEngine;

[System.Serializable]
public class SpawnEvent
{
    public float startTime;

    public GameObject enemyPrefab;

    public PathData path;

    public int quantity = 5;
    public float spawnInterval = 0.2f;

    [HideInInspector] public bool hasStarted;
}