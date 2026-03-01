using UnityEngine;

public class EnemigoPathFollower : MonoBehaviour
{
    private PathComponent pathComponent;
    private Vector2[] waypoints;
    private float timer;
    private Vector2 pathOffset;
    private Vector2 previousPosition;
    public bool rotateToMovement = true;
    public float rotationOffset = -90f;

    public void Initialize(PathComponent path, Vector2 offset)
    {
        pathComponent = path;
        waypoints = path.GetWaypoints();
        pathOffset = offset;

        timer = 0f;

        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0] + pathOffset;
            previousPosition = transform.position;
        }
    }


    private void Update()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        timer += Time.deltaTime;

        float t = timer / pathComponent.duration;
        t = Mathf.Clamp01(t);

        Vector2 newPosition = EvaluatePath(t);
        transform.position = newPosition;

        if (rotateToMovement)
        {
            Vector2 direction = newPosition - previousPosition;

            if (direction.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle + rotationOffset);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }

        previousPosition = newPosition;
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