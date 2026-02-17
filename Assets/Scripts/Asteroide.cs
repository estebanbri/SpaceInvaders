using UnityEngine;

public class Asteroide : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float minFallSpeed = 2f;
    [SerializeField] private float maxFallSpeed = 5f;
    [SerializeField] private float maxLateralDrift = 1.5f;

    [Header("Rotation")]
    [SerializeField] private float minRotationSpeed = 50f;
    [SerializeField] private float maxRotationSpeed = 200f;

    [Header("Bounds")]
    [SerializeField] private float destroyY = -7f;

    private float fallSpeed;
    private float lateralDrift;
    private float rotationSpeed;

    private void Start()
    {
        // Velocidad vertical constante
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

        // Pequeña deriva lateral
        lateralDrift = Random.Range(-maxLateralDrift, maxLateralDrift);

        // Rotación continua
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.value > 0.5f)
            rotationSpeed *= -1f;
    }

    private void Update()
    {
        // Movimiento descendente controlado
        transform.position += new Vector3(
            lateralDrift * Time.deltaTime,
            -fallSpeed * Time.deltaTime,
            0f
        );

        // Rotación constante
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

    }

    public void TakeDamage(int damageAmount, Vector3? attackerPos)
    {
    }
}