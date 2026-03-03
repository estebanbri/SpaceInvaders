using UnityEngine;

public class HoverMovementController : MonoBehaviour
{
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
}

public enum MovementAxis
{
    Horizontal,
    Vertical
}