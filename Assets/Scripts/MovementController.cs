using UnityEngine;

/// <summary>
/// Controla el movimiento de un enemigo según un patrón definido por ScriptableObject
/// </summary>
public class MovementController : MonoBehaviour
{
    private MovementPatternDefinition pattern;

    private Vector3 startPos;
    private float elapsedTime = 0f;
    private float currentAngle = 0f;

    // PATH
    private int currentPathIndex;
    private Vector3 spawnOrigin;
    private float halfWidth;
    void Awake()
    {
        startPos = transform.position;
    }

    private void Start()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        halfWidth = sr.bounds.extents.x;
    }

    void Update()
    {
        if (pattern == null)
            return;

        elapsedTime += Time.deltaTime;

        switch (pattern.type)
        {
            case MovementType.Lineal:
                transform.position += (Vector3)(pattern.direction.normalized * pattern.speed * Time.deltaTime);
                break;

            case MovementType.Circular:
                currentAngle += pattern.angularSpeed * Time.deltaTime;
                float rad = currentAngle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * pattern.radius;
                transform.position = startPos + offset;
                break;

            case MovementType.Senoidal:
                transform.position = startPos + new Vector3(
                    Mathf.Sin(elapsedTime * pattern.frequency) * pattern.amplitude,
                    -pattern.speed * elapsedTime,
                    0f
                );
                break;

            case MovementType.ZigZag:
                float x = Mathf.PingPong(elapsedTime * pattern.frequency, pattern.amplitude) - pattern.amplitude / 2f;
                transform.position = startPos + new Vector3(x, -pattern.speed * elapsedTime, 0f);
                break;

            case MovementType.Path:
                HandlePathMovement();
                break;
        }
        ClampToCamera();
    }

    private void HandlePathMovement()
    {
        if (pattern.pathPoints == null || pattern.pathPoints.Count == 0)
            return;

        Vector3 target = spawnOrigin + (Vector3)pattern.pathPoints[currentPathIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            pattern.pathSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            currentPathIndex++;

            if (currentPathIndex >= pattern.pathPoints.Count)
            {
                if (pattern.loopPath)
                    currentPathIndex = 0;
                else
                    enabled = false;
            }
        }
    }

    public void SetPattern(MovementPatternDefinition newPattern)
    {
        pattern = newPattern;

        startPos = transform.position;
        spawnOrigin = transform.position;

        elapsedTime = 0f;
        currentAngle = 0f;
        currentPathIndex = 0;
    }

    private void ClampToCamera()
    {
        Camera cam = Camera.main;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        Vector3 pos = transform.position;

        if (pos.x - halfWidth <= min.x && pattern.direction.x < 0)
        {
            pattern.direction.x *= -1f;
            pos.x = min.x + halfWidth;
        }

        if (pos.x + halfWidth >= max.x && pattern.direction.x > 0)
        {
            pattern.direction.x *= -1f;
            pos.x = max.x - halfWidth;
        }

        transform.position = pos;
    }

}
