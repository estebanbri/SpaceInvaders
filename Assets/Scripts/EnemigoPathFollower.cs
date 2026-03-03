using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class EnemigoPathFollower : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationOffset = -90f; // depende de tu sprite

    private PathComponent pathComponent;
    private Vector2[] waypoints;
    private float timer;
    private Vector2 pathOffset;
    private Vector2 previousPosition;
    public bool rotateToMovement = true;

    public void Initialize(PathComponent path, Vector2 offset, bool reverse)
    {
        pathComponent = path;
        waypoints = path.GetWaypoints();

        if (reverse)
            System.Array.Reverse(waypoints);

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

        Vector2 newPosition = EvaluatePath(t);
        transform.position = newPosition;

        if (rotateToMovement)
        {
            RotateToMovement(newPosition);
        }

        previousPosition = newPosition;
    }

    private void OnPathFinished()
    {
        // Forzar posición final exacta
        transform.position = waypoints[waypoints.Length - 1] + pathOffset;

        // Desactivar este componente para que deje de actualizar
        enabled = false;

        if (TryGetComponent(out Enemigo enemigo))
        {
            enemigo.OnPathFinished();
        }
    }

    private void RotateToMovement(Vector2 currentPosition)
    {
        Vector2 direction = currentPosition - previousPosition;

        if (direction.sqrMagnitude < 0.0001f)
            return; // evitar rotaciones cuando casi no se mueve

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
            return waypoints[count - 1];

        float localT = scaledT - index;

        return Vector2.Lerp(
        waypoints[index],
        waypoints[index + 1],
        localT
        ) + pathOffset;
    }
}