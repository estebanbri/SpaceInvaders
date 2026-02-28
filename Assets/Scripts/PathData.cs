using UnityEngine;

[CreateAssetMenu(menuName = "Level/PathData")]
public class PathData : ScriptableObject
{
    public Vector2[] waypoints;
    public float duration = 5f;
}