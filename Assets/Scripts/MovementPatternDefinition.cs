using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementPattern", menuName = "Enemies/Movement Pattern")]
public class MovementPatternDefinition : ScriptableObject
{
    public MovementType type;

    [Header("Lineal")]
    public Vector2 direction = Vector2.down;
    public float speed = 5f;

    [Header("Circular")]
    public float radius = 1f;
    public float angularSpeed = 180f; // grados por segundo

    [Header("Senoidal / ZigZag")]
    public float amplitude = 1f;
    public float frequency = 1f;

    [Header("Path Movement")]
    public List<Vector2> pathPoints;
    public float pathSpeed = 3f;
    public bool loopPath = true;

}