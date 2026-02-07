using UnityEngine;

public class TorretaCannonAim : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;

    private Transform target;
    private bool canAim;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetCanAim(bool value)
    {
        canAim = value;
    }

    void Update()
    {
        if (!canAim || target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;

        // Sprite apunta hacia ABAJO : +90°
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}