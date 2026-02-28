using UnityEngine;

public class PathComponent : MonoBehaviour
{
    public float duration = 4f;

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