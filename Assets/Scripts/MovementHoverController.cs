using UnityEngine;

public class HoverMovementController : MonoBehaviour
{
    [Header("Safe Area")]
    [SerializeField, Range(0f, 0.4f)] private float horizontalSafePercent = 0.2f;
    [SerializeField, Range(0f, 0.4f)] private float verticalSafePercent = 0.1f;

    public float moveSpeed = 2f;

    public float hoverDuration = 2f;
    public float moveDuration = 1.5f;

    public float driftAmplitude = 0.3f;
    public float driftSpeed = 3f;

    public MovementAxis movementAxis = MovementAxis.Horizontal;

    private Enemigo enemigo;

    private float stateTimer;
    private Vector2 moveDirection;
    private Vector2 hoverCenter;

    private float randomOffset;

    private InternalState currentState;

    private float lastOscillation;


    private enum InternalState
    {
        Hover,
        Move
    }

    void Start()
    {
        enemigo = GetComponent<Enemigo>();
        randomOffset = Random.Range(0f, 10f);
    }

    void OnEnable()
    {
        EnterMove();
    }

    void Update()
    {
        if (enemigo.State != EnemyState.Hovering) return;

        stateTimer -= Time.deltaTime;

        if (currentState == InternalState.Hover)
        {
             HoverMovement();

            if (stateTimer <= 0f)
                EnterMove();
        }
        else // Move
        {
            MoveRandom();

            if (stateTimer <= 0f)
                EnterHover();
        }

        ClampToSafeArea();
    }

    void EnterMove()
    {
        currentState = InternalState.Move;
        stateTimer = moveDuration;
        PickRandomDirection();
    }

    Vector2 basePosition;

    void EnterHover()
    {
        currentState = InternalState.Hover;
        stateTimer = hoverDuration;
        basePosition = transform.position;
    }


    void HoverMovement()
    {
        float currentOscillation = Mathf.Sin(Time.time * driftSpeed + randomOffset) * driftAmplitude;

        float delta = currentOscillation - lastOscillation;

        Vector2 offset;

        if (movementAxis == MovementAxis.Horizontal)
            offset = new Vector2(0f, delta);
        else
            offset = new Vector2(delta, 0f);

        transform.position += (Vector3)offset;

        lastOscillation = currentOscillation;
    }

    void MoveRandom()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void PickRandomDirection()
    {
        if (movementAxis == MovementAxis.Horizontal)
        {
            float x = Random.Range(-1f, 1f);
            moveDirection = new Vector2(x, 0f).normalized;
        }
        else
        {
            float y = Random.Range(-1f, 1f);
            moveDirection = new Vector2(0f, y).normalized;
        }
    }
    void ClampToSafeArea()
    {
        Camera cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float safeX = halfWidth * horizontalSafePercent;
        float safeY = halfHeight * verticalSafePercent;

        float minX = -halfWidth + safeX;
        float maxX = halfWidth - safeX;

        float minY = -halfHeight + safeY;
        float maxY = halfHeight - safeY;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}



public enum MovementAxis
{
    Horizontal,
    Vertical
}