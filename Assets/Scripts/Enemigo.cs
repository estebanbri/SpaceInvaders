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
    [SerializeField] private List<BossWeaponCycle> bossWeaponsByCycle;

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

    public void TakeDamage(int damageAmount, Vector3? attackerPos)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        Vector3 hitDir = attackerPos.HasValue
            ? (transform.position - attackerPos.Value).normalized
            : Vector3.up;

        enemigoVisual?.PlayHitEffect(hitDir);

        if (currentHealth <= 0)
            Die();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[ENEMY] " + gameObject.name + " died.");

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

    public void ConfigureByCycle(int cycle)
    {
        
        WeaponDefinition selectedWeapon = null;

        // Selecciona arma según tier
        if (isBoss && bossWeaponsByCycle != null)
        {
            foreach (var entry in bossWeaponsByCycle)
                if (cycle >= entry.minCycle) {
                    selectedWeapon = entry.weaponDefinition;
                }

            if (selectedWeapon != null)
            {
                weaponController?.Equip(selectedWeapon, cycle);
            }
        }
        else if (weaponController != null)
        {

            selectedWeapon = weaponController.GetWeaponDefault();
            weaponController.Equip(selectedWeapon, cycle);
        }

        // Escalado vida
        float healthMultiplier = 1f + cycle * 0.5f;
        maxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentHealth = maxHealth;
        Debug.Log($"[Enemigo] {gameObject.name} | Cycle: {cycle} | MaxHealth: {maxHealth}");
    }


    public void OnDeathAnimationFinished() { Destroy(gameObject); }
}
