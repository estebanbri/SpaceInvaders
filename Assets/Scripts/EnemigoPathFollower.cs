using UnityEngine;

public class EnemigoPathFollower : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationOffset = -90f;
    public bool rotateToMovement = true;

    [Header("Movement Modifier")]
    [SerializeField] private float modifierAmplitude = 0.5f;
    [SerializeField] private float modifierFrequency = 2f;

    [Header("Random Settings")]
    [SerializeField] private bool randomizeModifierValues = false;
    [SerializeField] private Vector2 amplitudeRange = new Vector2(0.3f, 1.2f);
    [SerializeField] private Vector2 frequencyRange = new Vector2(1f, 3f);

    private float currentAmplitude;
    private float currentFrequency;

    private MovementModifier movementModifier = MovementModifier.None;

    private PathComponent pathComponent;
    private Vector2[] waypoints;
    private float timer;
    private Vector2 pathOffset;
    private Vector2 previousPosition;

    public void Initialize(
        PathComponent path,
        Vector2 offset,
        bool reverse,
        MovementModifier modifier)
    {
        pathComponent = path;
        waypoints = path.GetWaypoints();

        if (reverse)
            System.Array.Reverse(waypoints);

        pathOffset = offset;
        movementModifier = modifier;
        timer = 0f;

        SetupModifierValues();

        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0] + pathOffset;
            previousPosition = transform.position;
        }
    }

    private void SetupModifierValues()
    {
        if (randomizeModifierValues)
        {
            currentAmplitude = Random.Range(amplitudeRange.x, amplitudeRange.y);
            currentFrequency = Random.Range(frequencyRange.x, frequencyRange.y);
        }
        else
        {
            currentAmplitude = modifierAmplitude;
            currentFrequency = modifierFrequency;
        }
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length < 2 || !enabled)
            return;

        timer += Time.deltaTime;

        float t = timer / pathComponent.duration;
        t = Mathf.Clamp01(t);

        if (t >= 1f)
        {
            OnPathFinished();
            return;
        }

        Vector2 basePosition = EvaluatePath(t);
        Vector2 newPosition = ApplyMovementModifier(basePosition);

        transform.position = newPosition;

        if (rotateToMovement)
            RotateToMovement(newPosition);

        previousPosition = newPosition;
    }

    private Vector2 ApplyMovementModifier(Vector2 basePosition)
    {
        if (movementModifier == MovementModifier.None)
            return basePosition;

        Vector2 forwardSample = EvaluatePath(Mathf.Min(timer / pathComponent.duration + 0.01f, 1f));
        Vector2 direction = (forwardSample - basePosition).normalized;

        if (direction.sqrMagnitude < 0.0001f)
            return basePosition;

        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        switch (movementModifier)
        {
            case MovementModifier.Sine:
                float sine = Mathf.Sin(Time.time * currentFrequency);
                return basePosition + perpendicular * sine * currentAmplitude;
        }

        return basePosition;
    }

    private void OnPathFinished()
    {
        transform.position = waypoints[waypoints.Length - 1] + pathOffset;
        enabled = false;

        if (TryGetComponent(out Enemigo enemigo))
            enemigo.OnPathFinished();
    }

    private void RotateToMovement(Vector2 currentPosition)
    {
        Vector2 direction = currentPosition - previousPosition;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private Vector2 EvaluatePath(float t)
    {
        int count = waypoints.Length;

        float scaledT = t * (count - 1);
        int index = Mathf.FloorToInt(scaledT);

        if (index >= count - 1)
            return waypoints[count - 1] + pathOffset;

        float localT = scaledT - index;

        return Vector2.Lerp(
            waypoints[index],
            waypoints[index + 1],
            localT
        ) + pathOffset;
    }
}