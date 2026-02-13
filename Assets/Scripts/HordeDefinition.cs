using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HordeDefinition
{
    [System.Serializable]
    public struct FormationPoint
    {
        public Vector2 offset;
    }

    [Header("Custom Formation")]
    public bool useCustomFormation;
    public List<FormationPoint> customFormationPoints;
    public enum HordeFormationType
    {
        Horizontal,
        Vertical,
        Diagonal
    }

    [Header("Formation")]
    public HordeFormationType formationType = HordeFormationType.Horizontal;
    public float formationOffset = 2f;
    public GameObject enemyPrefab;
    public int hordeCount = 1;
    public MovementPatternDefinition movementPattern;
}