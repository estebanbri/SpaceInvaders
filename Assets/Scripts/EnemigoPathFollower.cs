using UnityEngine;

public class EnemigoPathFollower : MonoBehaviour
{
    private PathComponent pathComponent;
    private Vector2[] waypoints;
    private float timer;
    private Vector2 pathOffset;

    public void Initialize(PathComponent path, Vector2 offset)
    {
        pathComponent = path;
        waypoints = path.GetWaypoints();
        pathOffset = offset;

        timer = 0f;

        if (waypoints.Length > 0)
            transform.position = waypoints[0] + pathOffset;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        timer += Time.deltaTime;

        float t = timer / pathComponent.duration;
        t = Mathf.Clamp01(t);

        transform.position = EvaluatePath(t);
    }

    private Vector2 EvaluatePath(float t)
    {
        int count = waypoints.Length;

        float scaledT = t * (count - 1);
        int index = Mathf.FloorToInt(scaledT);

        if (index >= count - 1)
            return waypoints[count - 1];

        float localT = scaledT - index;

        return Vector2.Lerp(
        waypoints[index],
        waypoints[index + 1],
        localT
        ) + pathOffset;
    }
}