using UnityEngine;

public class Asteroide : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float minFallSpeed = 2f;
    [SerializeField] private float maxFallSpeed = 5f;

    [Header("Drift")]
    [SerializeField] private float driftStrength = 1.5f;
    [SerializeField] private float driftFrequency = 1.2f;

    [Header("Rotation")]
    [SerializeField] private float minRotationSpeed = 50f;
    [SerializeField] private float maxRotationSpeed = 200f;

    [Header("Bounds")]
    [SerializeField] private float destroyY = -7f;

    private float fallSpeed;
    private float rotationSpeed;
    private float driftOffset;

    private void Start()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);

        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.value > 0.5f)
            rotationSpeed *= -1f;

        driftOffset = Random.Range(0f, 100f);

        // Variación de tamaño
        float scale = Random.Range(0.7f, 1.3f);
        transform.localScale = Vector3.one * scale;
    }

    private void Update()
    {
        float drift = (Mathf.PerlinNoise(Time.time * driftFrequency, driftOffset) - 0.5f) * driftStrength;

        transform.position += new Vector3(
            drift * Time.deltaTime,
            -fallSpeed * Time.deltaTime,
            0f
        );

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (transform.position.y < destroyY)
            Destroy(gameObject);
    }

    public void TakeDamage(int damageAmount, Vector3? attackerPos, bool isCritical = false)
    {
    }
}
