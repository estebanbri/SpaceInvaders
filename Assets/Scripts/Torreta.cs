using UnityEngine;

public class Torreta : MonoBehaviour, IDamageable
{
    [Header("Combat")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private float fireDelay = 1f;
    [SerializeField] private float rotationSpeed = 180f; // Grados por segundo

    private int currentHealth;
    private float fireTimer;
    private bool isDead;

    private Collider2D col;

    private Nave playerNave; // Referencia a la nave

    private bool isVisibleOnCamera;

    private void Awake()
    {
        isDead = false;
        currentHealth = maxHealth;
        col = GetComponent<Collider2D>();

        // Buscar la nave
        playerNave = FindFirstObjectByType<Nave>();
    }

    private void Update()
    {
        if (isDead || weaponController == null || !isVisibleOnCamera)
            return;

        // 1️⃣ Girar suavemente hacia la nave
        if (playerNave != null)
        {
            Vector2 direction = (playerNave.transform.position - transform.position).normalized;

            // Sprite apunta hacia ABAJO → compensamos con +90°
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // 2️⃣ Manejar disparo
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireDelay)
        {
            weaponController.Fire();
            fireTimer = 0f;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        ModifyHealth(-damageAmount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ModifyHealth(int amount)
    {
        currentHealth += amount;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        col.enabled = false;
        // Aquí podrías agregar animaciones o efectos de muerte
    }

    public void SetVisible(bool visible)
    {
        isVisibleOnCamera = visible;

        if (visible)
            fireTimer = 0f; // evita disparo instantáneo
    }

}