using UnityEngine;

public class Torreta : MonoBehaviour, IDamageable
{
    [Header("Combat")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private float fireDelay = 1f;

    [Header("Visual")]
    [SerializeField] private Transform cannonTransform;

    [SerializeField] private TorretaCannonAim cannonAim;

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

        playerNave = FindFirstObjectByType<Nave>();

        if (playerNave != null && cannonAim != null)
        {
            cannonAim.SetTarget(playerNave.transform);
        }
    }

    private void Update()
    {
        if (isDead || weaponController == null || !isVisibleOnCamera)
            return;

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

        if (cannonAim != null)
            cannonAim.SetCanAim(visible);

        if (visible)
            fireTimer = 0f;
    }

}