using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnEvent
{
    public float startTime;

    public GameObject enemyPrefab;

    public List<PathComponent> possiblePaths;

    public int quantity = 5;
    public float spawnInterval = 0.2f;

    [HideInInspector] public bool hasStarted;

    public PathDirection pathDirection;
}