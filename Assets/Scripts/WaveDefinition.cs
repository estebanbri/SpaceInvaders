using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave")]
public class WaveDefinition: ScriptableObject
{
    [Header("Enemies")]
    public List<GameObject> enemiesToSpawn;
    public float spawnDelay = 0.5f;
    public bool isBossWave;
    public GameObject bossPrefab;

    [Header("Fixed Turrets")]
    public List<GameObject> turretPrefabs; // Prefabs de torretas que pueden aparecer en esta wave
    public int turretsToSpawn = 0;          // Cuántas torretas aleatorias spawnearán
}