using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Entrance")]
    [SerializeField] private float enterOffset = 1f;
    [SerializeField] private float enterSpeed = 3f;

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float verticalAmplitude = 0.5f;
    [SerializeField] private float verticalFrequency = 1f;

    [Header("Circle Movement")]
    [SerializeField] private float minCircleRadius = 2.5f;
    [SerializeField] private float maxCircleRadius = 4.5f;
    [SerializeField] private float minCircleDuration = 2.5f;
    [SerializeField] private float maxCircleDuration = 4f;
    [SerializeField] private float minTimeBetweenCircles = 6f;
    [SerializeField] private float maxTimeBetweenCircles = 12f;
    [SerializeField] private float bobAmplitude = 0.3f;
    [SerializeField] private float bobFrequency = 3f;

    private Vector3 targetPosition;
    private bool entering = true;

    private float halfWidth;
    private float halfHeight;

    private float leftLimit;
    private float rightLimit;
    private float topLimit;
    private float bottomLimit;

    private int direction = 1;

    private Enemigo enemigo;

    // Circle movement variables
    private bool isInCircle = false;
    private Vector2 circleCenter;
    private float circleRadius;
    private float circleDuration;
    private float circleTimer;
    private float circleDirection;
    private float angle;

    private float timeUntilNextCircle;

    void Start()
    {
        enemigo = GetComponent<Enemigo>();
        Camera cam = Camera.main;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        // 🔥 Calcular bounds combinados de TODOS los sprites hijos
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Bounds combinedBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        halfWidth = combinedBounds.extents.x;
        halfHeight = combinedBounds.extents.y;

        leftLimit = min.x + halfWidth;
        rightLimit = max.x - halfWidth;
        bottomLimit = min.y + halfHeight;
        topLimit = max.y - halfHeight;

        // Posición objetivo final
        targetPosition = new Vector3(0, topLimit - enterOffset, 0);

        // Empieza fuera de pantalla
        transform.position = new Vector3(0, max.y + halfHeight, 0);

        timeUntilNextCircle = Random.Range(minTimeBetweenCircles, maxTimeBetweenCircles);
    }

    void Update()
    {
        if (entering)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                enterSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                entering = false;

            return;
        }

        enemigo.SetState(EnemyState.Idle);

        if (isInCircle)
        {
            UpdateCircleMovement();
        }
        else
        {
            MoveHorizontal();
            timeUntilNextCircle -= Time.deltaTime;
            if (timeUntilNextCircle <= 0f)
            {
                StartCircleMovement();
            }
        }
    }

    void MoveHorizontal()
    {
        float newX = transform.position.x + direction * moveSpeed * Time.deltaTime;

        if (newX > rightLimit)
        {
            newX = rightLimit;
            direction = -1;
        }
        else if (newX < leftLimit)
        {
            newX = leftLimit;
            direction = 1;
        }

        float newY = targetPosition.y + Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;

        // 🔥 Clamp vertical
        newY = Mathf.Clamp(newY, bottomLimit, topLimit);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    void StartCircleMovement()
    {
        isInCircle = true;
        circleTimer = 0f;

        circleCenter = new Vector2(
            transform.position.x + Random.Range(-1f, 1f),
            targetPosition.y + Random.Range(-0.5f, 0.5f)
        );

        circleRadius = Random.Range(minCircleRadius, maxCircleRadius);
        circleDuration = Random.Range(minCircleDuration, maxCircleDuration);
        circleDirection = (Random.value < 0.5f) ? 1f : -1f;
        angle = 0f;

        // 🔥 Asegurar que el círculo no nazca fuera
        circleCenter.x = Mathf.Clamp(circleCenter.x, leftLimit + circleRadius, rightLimit - circleRadius);
        circleCenter.y = Mathf.Clamp(circleCenter.y, bottomLimit + circleRadius, topLimit - circleRadius);
    }

    void UpdateCircleMovement()
    {
        circleTimer += Time.deltaTime;

        angle += (2 * Mathf.PI / circleDuration) * Time.deltaTime * circleDirection;

        float x = circleCenter.x + Mathf.Cos(angle) * circleRadius;
        float y = circleCenter.y + Mathf.Sin(angle) * circleRadius;

        y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

        // 🔥 Clamp final de seguridad
        x = Mathf.Clamp(x, leftLimit, rightLimit);
        y = Mathf.Clamp(y, bottomLimit, topLimit);

        transform.position = new Vector3(x, y, transform.position.z);

        if (circleTimer >= circleDuration)
        {
            isInCircle = false;
            timeUntilNextCircle = Random.Range(minTimeBetweenCircles, maxTimeBetweenCircles);
        }
    }
}