using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyBehaviourType behaviourType;

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
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationOffset = -90f; // depende de tu sprite

    [SerializeField] private GameObject damageTextPrefab;

    private int currentHealth;

    [SerializeField] private EnemigoVisual enemigoVisual;
    private Collider2D col;

    public bool IsVulnerable { get; private set; } = true;

    public EnemyState State { get; private set; }
    public bool IsBoss => isBoss;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public System.Action<int, int> OnHealthChanged;
    public System.Action OnEnemyDied;

    private enum BossAttackPhase { Waiting, Burst, Moving }
    private BossAttackPhase bossAttackPhase = BossAttackPhase.Waiting;

    private int burstShotsDone = 0;
    private float burstTimer = 0f;
    private float phaseTimer = 0f;
    private bool isDead;
    public bool IsDead => isDead;

    [SerializeField] private int shotsPerBurst = 2;
    [SerializeField] private float burstInterval = 1f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 2f;



    private float nextWaitTime;

    void Awake()
    {
        currentHealth = maxHealth;
        col = GetComponent<Collider2D>();
        SetState(EnemyState.Entering);
    }

    void Update()
    {
        if (isDead) return;

        if (State == EnemyState.Hovering)
        {
            RotateTowardPlayer();
        }

        if (isBoss && weaponController != null && State == EnemyState.Hovering)
        {
            HandleBossBurstShooting();
        }
        else if (weaponController != null && State == EnemyState.Hovering)
        {
            weaponController.Fire();
        }
    }

    private void RotateTowardPlayer()
    {
        if (Nave.Instance == null) return;
        Vector2 direction = Nave.Instance.transform.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void TakeDamage(int damageAmount, Vector3? attackerPos, bool isCritical = false)
    {
        if (isDead || !IsVulnerable) return;

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);

        SpawnFloatingDamage(damageAmount, isCritical);

        Vector3 hitDir = attackerPos.HasValue
            ? (transform.position - attackerPos.Value).normalized
            : Vector3.up;

        enemigoVisual?.PlayHitEffect(hitDir);

        if (currentHealth <= 0)
            Die();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void SpawnFloatingDamage(int damage, bool isCritical)
    {
        if (!isCritical) return;

        if (damageTextPrefab == null) return;

        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-0.3f, 0.3f),
            UnityEngine.Random.Range(0f, 0.5f),
            0f
        );

        Vector3 spawnPos = transform.position + randomOffset;

        GameObject dmgObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

        // Evitar que herede rotación del enemigo
        dmgObj.transform.rotation = Quaternion.identity;

        FloatingDamageText floating = dmgObj.GetComponent<FloatingDamageText>();
        if (floating != null)
        {
            floating.SetDamage(damage, true); // siempre true porque ya validamos
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[ENEMY] " + gameObject.name + " died.");

        col.enabled = false;

        // 🔥 Notifica al Spawner (caos controlado)
        OnEnemyDied?.Invoke();
        
        TryCreatePickups();
        EnemigoManager.Instance.UnregisterEnemy();
        enemigoVisual?.PlayDeath();
    }

    public void OnPathFinished()
    {
        if (behaviourType == EnemyBehaviourType.PassThrough)
        {
            Despawn();
            return;
        }

        SetState(EnemyState.Hovering);
    }

    private void Despawn()
    {
        if (isDead) return;

        Debug.Log("[ENEMY] " + gameObject.name + " despawn.");

        col.enabled = false;

        EnemigoManager.Instance.UnregisterEnemy();

        Destroy(gameObject);
    }

    private void TryCreatePickups()
    {
        int scoreCount = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < scoreCount; i++)
        {
            Vector3 offset = new Vector3(i + 1, -i, 0);
            Instantiate(scorePickupPrefab, transform.position + offset, Quaternion.identity);
        }

        if (bonusPickupPrefab == null) return;
        if (UnityEngine.Random.value > dropProbability) return;
        if (BonusManager.Instance.IsBonusActive(bonusPickupPrefab.GetBonusDefinition())) return;

        Instantiate(bonusPickupPrefab, transform.position, Quaternion.identity);
    }

    public void SetState(EnemyState newState)
    {
        State = newState;
    }

    public void ConfigureByCycle(int cycle)
    {
        WeaponDefinition selectedWeapon = null;

        if (isBoss && bossWeaponsByCycle != null)
        {
            foreach (var entry in bossWeaponsByCycle)
                if (cycle >= entry.minCycle)
                    selectedWeapon = entry.weaponDefinition;

            if (selectedWeapon != null)
                weaponController?.Equip(selectedWeapon, cycle);
        }
        else if (weaponController != null)
        {
            selectedWeapon = weaponController.GetWeaponDefault();
            weaponController.Equip(selectedWeapon, cycle);
        }

        float healthMultiplier = 1f + cycle * 0.5f;
        maxHealth = Mathf.RoundToInt(maxHealth * healthMultiplier);
        currentHealth = maxHealth;
    }

    private void HandleBossBurstShooting()
    {
        if (!isBoss || weaponController == null || State != EnemyState.Idle)
            return;

        switch (bossAttackPhase)
        {
            case BossAttackPhase.Waiting:
                phaseTimer += Time.deltaTime;
                if (phaseTimer >= nextWaitTime)
                {
                    bossAttackPhase = BossAttackPhase.Burst;
                    burstShotsDone = 0;
                    burstTimer = 0f;
                    phaseTimer = 0f;
                }
                break;

            case BossAttackPhase.Burst:
                burstTimer += Time.deltaTime;

                if (burstShotsDone < shotsPerBurst && burstTimer >= burstInterval)
                {
                    burstTimer = 0f;
                    weaponController.FireImmediate();
                    burstShotsDone++;
                }

                if (burstShotsDone >= shotsPerBurst)
                {
                    bossAttackPhase = BossAttackPhase.Waiting;
                    phaseTimer = 0f;
                    nextWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
                }
                break;
        }
    }

    public void SetVulnerable(bool value)
    {
        IsVulnerable = value;
    }

   
   
}