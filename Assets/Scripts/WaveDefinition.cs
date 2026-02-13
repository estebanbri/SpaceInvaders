using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave")]
public class WaveDefinition : ScriptableObject
{
    [Header("Enemies")]
    public List<GameObject> enemyPrefabs;

    [Header("Formation")]
    public int rows;
    public int columns;
    public float spacingX;
    public float spacingY;
    public float baseSpeed;

    [Header("Boss")]
    public bool isBossWave;
    public GameObject bossPrefab;

    [Header("Fixed Turrets")]
    public List<GameObject> turretPrefabs;
    public int turretsToSpawn = 0;
}