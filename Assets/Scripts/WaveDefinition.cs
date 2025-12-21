using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave")]
public class WaveDefinition: ScriptableObject
{
    public List<GameObject> enemiesToSpawn;
    public float spawnDelay = 0.5f;
    public bool isBossWave;
    public GameObject bossPrefab;
}