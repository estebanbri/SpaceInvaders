using UnityEngine;

public class EnemigoPathFollower : MonoBehaviour
{
    private PathData pathData;
    private float timer;

    public void Initialize(PathData path)
    {
        pathData = path;
        timer = 0f;

        if (pathData.waypoints.Length > 0)
            transform.position = pathData.waypoints[0];
    }

    private void Update()
    {
        if (pathData == null)
            return;

        timer += Time.deltaTime;

        float t = timer / pathData.duration;
        t = Mathf.Clamp01(t);

        transform.position = EvaluatePath(t);
    }

    private Vector2 EvaluatePath(float t)
    {
        int count = pathData.waypoints.Length;

        if (count < 2)
            return transform.position;

        float scaledT = t * (count - 1);
        int index = Mathf.FloorToInt(scaledT);

        if (index >= count - 1)
            return pathData.waypoints[count - 1];

        float localT = scaledT - index;

        return Vector2.Lerp(
            pathData.waypoints[index],
            pathData.waypoints[index + 1],
            localT
        );
    }
}