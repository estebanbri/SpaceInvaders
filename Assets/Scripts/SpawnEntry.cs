using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    [Range(0f, 1f)]
    public float enemySpawnProbability = 0.5f; // peso relativo a ser seleccionado para spawnear

    public GameObject prefab;

    public int ordaQuantity = 1;

    public int maxAliveCount = 5;

    [HideInInspector] public int aliveCount;
}
