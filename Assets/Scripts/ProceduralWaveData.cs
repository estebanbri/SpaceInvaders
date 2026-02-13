using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProceduralWaveData
{
    public List<GameObject> enemyPrefabs;

    public int rows;
    public int columns;

    public float spacingX = 1.5f;
    public float spacingY = 1.5f;

    public float moveSpeed;
    public float healthMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public int tier;
    public FormationPattern formationPattern = FormationPattern.Grid;
}
