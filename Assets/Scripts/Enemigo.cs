using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour, IDamageable
{
    [Header("Drops")]
    [SerializeField] private PickupBonus bonusPickupPrefab;
    [SerializeField] private GameObject scorePickupPrefab;
    [Range(0, 1)]
    [SerializeField] private float dropProbability;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private WeaponController weaponController;
    public WeaponController GetWeaponController => weaponController;

    [Header("Boss")]
    [SerializeField] private bool isBoss = false;
    [SerializeField] private List<BossWeaponTier> bossWeaponsByTier;

    private int currentHealth;
    private bool isDead;
    private EnemigoVisual enemigoVisual;
    private Collider2D col;
    private MovementController movementController;
    private FormationController formation;

    public EnemyState State { get; private set; }
    public bool IsBoss => isBoss;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnEnemyDied;

    void Awake()
    {
        currentHealth = maxHealth;
        enemigoVisual = GetComponentInChildren<EnemigoVisual>();
        col = GetComponent<Collider2D>();
        movementController = GetComponent<MovementController>();
        SetState(EnemyState.Entering);
    }

    void Update()
    {
        if (isDead) return;
        // Maneja disparo usando WeaponInstance.FireRate
        if (weaponController != null && State == EnemyState.Attacking)
        {
            weaponController.TryFire();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
        enemigoVisual?.PlayHitEffect();

        if (currentHealth <= 0)
        {
             Die();
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[ENEMY]  died.");

        col.enabled = false;
        OnEnemyDied?.Invoke();
        formation?.NotifyEnemyKilled();
        enemigoVisual?.PlayDeath();
        TryCreatePickups();
    }

    private void TryCreatePickups()
    {
        // Score y bonus pickups
        int scoreCount = Random.Range(1, 4);
        for (int i = 0; i < scoreCount; i++)
        {
            Vector3 offset = new Vector3(i + 1, -i, 0);
            Instantiate(scorePickupPrefab, transform.position + offset, Quaternion.identity);
        }

        if (bonusPickupPrefab == null) return;
        if (Random.value > dropProbability) return;
        if (BonusManager.Instance.IsBonusActive(bonusPickupPrefab.GetBonusDefinition())) return;

        Instantiate(bonusPickupPrefab, transform.position, Quaternion.identity);
    }

    public void SetState(EnemyState newState)
    {
        State = newState;
    }

    public void SetFormation(FormationController controller)
    {
        formation = controller;
    }

    public void ApplyProceduralScaling(float healthMultiplier, float fireRateMultiplier)
    {
        maxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentHealth = maxHealth;

        // No modificamos fireDelay aquí, FireRate queda en WeaponInstance
    }

    public void ConfigureByTier(int tier)
    {
        Debug.Log($"[Enemigo] {gameObject.name} | Tier: {tier} | MaxHealth: {maxHealth} | Weapon: {weaponController?.GetWeaponDefault()?.name}");

        // Selecciona arma según tier
        if (isBoss && bossWeaponsByTier != null)
        {
            WeaponDefinition selectedWeapon = null;
            foreach (var entry in bossWeaponsByTier)
                if (tier >= entry.minTier) selectedWeapon = entry.weaponDefinition;

            if (selectedWeapon != null)
                weaponController?.Equip(selectedWeapon, tier);
        }
        else if (weaponController != null)
        {

            // Enemigos normales usan tier para escalar stats de su arma
            var w = weaponController.GetWeaponDefault();
            weaponController.Equip(w, tier);
            Debug.Log($"[Enemigo] {gameObject.name} Weapon Stats | AmmoSpeed: {w.ammoSpeed:F2} | FireRate: {w.fireRate:F2} | Bullets: {w.bulletCount}");
        }

        // Escalado vida
        float healthMultiplier = 1f + tier * 0.5f;
        maxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentHealth = maxHealth;
    }


    public void OnDeathAnimationFinished() { Destroy(gameObject); }
}
