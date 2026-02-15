using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ProceduralWaveData
{
    // ----------------------------------
    // Enemigos
    // ----------------------------------
    public List<GameObject> enemyPrefabs;

    // ----------------------------------
    // Formación fija
    // ----------------------------------
    public int fixedRows;
    public int fixedColumns;
    public bool invertShape;

    // ----------------------------------
    // Spacing
    // ----------------------------------
    public float spacingX = 1.5f;
    public float spacingY = 1.3f;

    // ----------------------------------
    // Escalado procedural
    // ----------------------------------
    public float moveSpeed;
    public float healthMultiplier;
    public float fireRateMultiplier;

    public int cycle;
}

