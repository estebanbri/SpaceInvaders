using UnityEngine;

public class PathComponent : MonoBehaviour
{
    public float duration = 4f;

    public PathOrientation orientation;
    public Vector2[] GetWaypoints()
    {
        int count = transform.childCount;
        Vector2[] points = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            points[i] = transform.GetChild(i).position;
        }

        return points;
    }

    public Bounds GetBounds()
    {
        Vector2[] points = GetWaypoints();

        if (points == null || points.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Vector2 min = points[0];
        Vector2 max = points[0];

        foreach (var p in points)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        Vector2 center = (min + max) / 2f;
        Vector2 size = max - min;

        return new Bounds(center, size);
    }

    private void OnDrawGizmos()
    {
        if (transform.childCount < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(
                transform.GetChild(i).position,
                transform.GetChild(i + 1).position
            );
        }
    }
}