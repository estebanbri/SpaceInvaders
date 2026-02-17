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

    private float leftLimit;
    private float rightLimit;
    private int direction = 1;

    private Enemigo enemigo;

    // Circle movement variables
    private bool isInCircle = false;
    private Vector2 circleCenter;
    private float circleRadius;
    private float circleDuration;
    private float circleTimer;
    private float circleDirection; // 1 = clockwise, -1 = counterclockwise
    private float angle;

    private float timeUntilNextCircle;

    void Start()
    {
        enemigo = GetComponent<Enemigo>();
        Camera cam = Camera.main;

        // Obtener borde superior de cámara
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, cam.nearClipPlane));

        // Calcular tamaño real del sprite
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        float halfHeight = sr.bounds.extents.y;
        halfWidth = sr.bounds.extents.x;

        // Posición objetivo final (un poco debajo del borde superior)
        targetPosition = new Vector3(0, top.y - enterOffset - halfHeight, 0);

        // Empieza fuera de pantalla
        transform.position = new Vector3(0, top.y + halfHeight, 0);

        CalculateHorizontalLimits();

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

    void CalculateHorizontalLimits()
    {
        Camera cam = Camera.main;
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        leftLimit = min.x + halfWidth;
        rightLimit = max.x - halfWidth;
    }

    void MoveHorizontal()
    {
        // Movimiento horizontal
        float newX = transform.position.x + direction * moveSpeed * Time.deltaTime;

        // Cambiar dirección si llega a los límites
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

        // Movimiento vertical oscilante suave
        float newY = targetPosition.y + Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    void StartCircleMovement()
    {
        isInCircle = true;
        circleTimer = 0f;

        // Centro del círculo alrededor de la posición actual con algo de variación
        circleCenter = new Vector2(
            transform.position.x + Random.Range(-1f, 1f),
            targetPosition.y + Random.Range(-0.5f, 0.5f)
        );

        circleRadius = Random.Range(minCircleRadius, maxCircleRadius);
        circleDuration = Random.Range(minCircleDuration, maxCircleDuration);
        circleDirection = (Random.value < 0.5f) ? 1f : -1f;
        angle = 0f;
    }

    void UpdateCircleMovement()
    {
        circleTimer += Time.deltaTime;
        float t = circleTimer / circleDuration;

        // Incrementar ángulo según duración y dirección
        angle += (2 * Mathf.PI / circleDuration) * Time.deltaTime * circleDirection;

        float x = circleCenter.x + Mathf.Cos(angle) * circleRadius;
        float y = circleCenter.y + Mathf.Sin(angle) * circleRadius;

        // Agregamos pequeño bob vertical
        y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;

        transform.position = new Vector3(x, y, transform.position.z);

        if (circleTimer >= circleDuration)
        {
            isInCircle = false;
            timeUntilNextCircle = Random.Range(minTimeBetweenCircles, maxTimeBetweenCircles);
        }
    }
}
