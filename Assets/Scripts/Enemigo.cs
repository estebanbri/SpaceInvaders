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

    [SerializeField] private GameObject damageTextPrefab;

    private int currentHealth;
    
    [SerializeField] private EnemigoVisual enemigoVisual;
    private Collider2D col;
    [SerializeField]  private MovementController movementController;
    private FormationController formation;
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
    [SerializeField] private float burstInterval = 1f; // tiempo entre disparos de la ráfaga
    [SerializeField] private float minWaitTime = 1f;   // espera aleatoria mínima antes de ráfaga
    [SerializeField] private float maxWaitTime = 2f;   // espera aleatoria máxima
    [SerializeField] private float moveDuration = 1f;    // cuánto se mueve entre ráfagas
    private float targetX;                                // posición horizontal a moverse


    [Header("Kamikaze")]
    [SerializeField] private float kamikazeSpeed = 6f;
    private Transform playerTransform;
    private Vector3 attackDirection;
    private bool isKamikazeActive;

    [Header("Returning")]
    [SerializeField] private float returnSpeed = 4f;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Vector3 returnStartPos;
    private Vector3 returnControlPoint;
    private float returnTimer = 0f;
    [SerializeField] private float returnDuration = 1.2f; // duración del retorno

    private enum AttackType { Curve, Arc }

    private AttackType attackType;
    private float attackTimer;
    private Vector3 attackStartPos;
    private Vector3 attackTargetPos;

    [SerializeField] private float attackDuration = 1.5f;
    [SerializeField] private float curveHeight = 2f;
    [SerializeField] private float moveSpeed = 5f;
    private int pendingDamage = 0;
    private bool pendingCrit = false;
    private float damageDisplayDelay = 0.05f; // tiempo para acumular impactos en el mismo frame
    private float damageTimer = 0f;

    private float nextFireTime = 0f; // tiempo en el que se disparará el próximo tiro

    private float curveDirection;
    private int currentCycle = 0;

    void Start()
    {
        currentHealth = maxHealth;
        col = GetComponent<Collider2D>();
        SetState(EnemyState.Entering);
        playerTransform = Nave.Instance.transform;
    }

    void Update()
    {
        if (isDead) return;

        if (State == EnemyState.Kamikaze)
        {
            UpdateKamikaze();
            return;
        }
        if (State == EnemyState.Returning)
        {
            UpdateReturning();
            return;
        }

        if (isBoss && weaponController != null && State == EnemyState.Idle)
        {
            HandleBossBurstShooting();
        }
        else if (weaponController != null && State == EnemyState.Idle)
        {
            weaponController.TryFire();
        }
        // 🔹 Mostrar daño acumulado cuando el timer llega a cero
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f && pendingDamage > 0)
            {
                ShowDamageText(pendingDamage, pendingCrit);
                pendingDamage = 0;
                pendingCrit = false;
            }
        }
    }

    private void UpdateKamikaze()
    {
        attackTimer += Time.deltaTime;

        float t = attackTimer / attackDuration;
        t = Mathf.Clamp01(t);

        Vector3 newPos;

        // 🔥 Calculamos límite inferior de la cámara
        float minY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 0.5f;

        if (attackType == AttackType.Curve)
        {
            float zigzagFrequency = 10f;
            float zigzagAmplitude = curveHeight;

            // Eliminamos el "Vector3.down * 8f" y limitamos directamente el target
            Vector3 baseTarget = attackTargetPos;
            if (baseTarget.y < minY)
                baseTarget.y = minY;

            Vector3 basePos = Vector3.Lerp(attackStartPos, baseTarget, t);

            float zigzagOffset = Mathf.Sin(t * Mathf.PI * zigzagFrequency) *
                                 zigzagAmplitude * (1f - t);

            Vector3 lateralOffset = Vector3.right * curveDirection * zigzagOffset;

            newPos = basePos + lateralOffset;
        }
        else
        {
            Vector3 controlPoint = attackStartPos +
                                   Vector3.up * curveHeight +
                                   Vector3.right * curveDirection * curveHeight;

            // Limitamos endPoint para que no cruce el borde
            Vector3 endPoint = attackTargetPos;
            if (endPoint.y < minY)
                endPoint.y = minY;

            Vector3 a = Vector3.Lerp(attackStartPos, controlPoint, t);
            Vector3 b = Vector3.Lerp(controlPoint, endPoint, t);

            newPos = Vector3.Lerp(a, b, t);
        }

        transform.position = newPos;

        if (t >= 1f)
        {
            StartReturning();
        }
    }


    public void StartReturning()
    {
        State = EnemyState.Returning;

        attackStartPos = transform.position;
        attackTargetPos = originalParent.TransformPoint(originalLocalPosition);

        // Limitar altura superior para no salirse
        float maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - 0.5f;
        if (attackTargetPos.y > maxY)
            attackTargetPos.y = maxY;

        // Reutilizamos exactamente los mismos parámetros de Kamikaze
        // Para que la curva lateral y duración sean idénticas
        // Solo invertimos dirección vertical
        curveDirection = curveDirection; // o -curveDirection si querés que zigzag sea espejo
                                         // NO tocamos curveHeight ni attackDuration
                                         // curveHeight = curveHeight;
                                         // attackDuration = attackDuration;

        attackTimer = 0f;
        transform.SetParent(null);
    }



    private void UpdateReturning()
    {
        attackTimer += Time.deltaTime;
        float t = Mathf.Clamp01(attackTimer / attackDuration);

        Vector3 newPos;

        if (attackType == AttackType.Curve)
        {
            float zigzagFrequency = 10f;
            float zigzagAmplitude = curveHeight;

            Vector3 basePos = Vector3.Lerp(attackStartPos, attackTargetPos, t);
            float zigzagOffset = Mathf.Sin(t * Mathf.PI * zigzagFrequency) * zigzagAmplitude * (1f - t);
            Vector3 lateralOffset = Vector3.right * curveDirection * zigzagOffset;

            newPos = basePos + lateralOffset;
        }
        else // Arc
        {
            Vector3 controlPoint = attackStartPos + Vector3.up * curveHeight + Vector3.right * curveDirection * curveHeight;
            Vector3 a = Vector3.Lerp(attackStartPos, controlPoint, t);
            Vector3 b = Vector3.Lerp(controlPoint, attackTargetPos, t);
            newPos = Vector3.Lerp(a, b, t);
        }

        transform.position = newPos;

        if (t >= 1f)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPosition;
            State = EnemyState.Idle;
        }
    }



    public void StartKamikaze()
    {
        State = EnemyState.Kamikaze;
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;

        attackTimer = 0f;

        attackType = (Random.value < 0.5f) ? AttackType.Curve : AttackType.Arc;
        curveDirection = (Random.value < 0.5f) ? -1f : 1f;
        Debug.Log("Kamikaze iniciado: " + attackType + " dirección: " + curveDirection);

        attackStartPos = transform.position;
        attackTargetPos = Nave.Instance.transform.position;

        // 🔥 Calculamos límite inferior de la cámara
        float minY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 0.5f;
        if (attackTargetPos.y < minY)
            attackTargetPos.y = minY;

        if (attackType == AttackType.Curve)
        {
            attackDuration = 2f;
            curveHeight = Random.Range(1.2f, 1.8f);
        }
        else
        {
            attackDuration = 2.2f;
            curveHeight = Random.Range(3f, 4.5f);
        }

        transform.SetParent(null);
    }

    public void TakeDamage(int damageAmount, Vector3? attackerPos, bool isCritical = false)
    {
        if (isDead || !IsVulnerable) return;

        // 🔹 Solo activamos el timer si estaba en cero
        if (damageTimer <= 0f)
            damageTimer = damageDisplayDelay;

        // 🔹 Acumulamos daño y crit
        pendingDamage += damageAmount;
        pendingCrit |= isCritical;

        // Aplicamos daño al health
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

    private float nextWaitTime;

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
                    FireBossShotImmediate();
                    burstShotsDone++;
                }

                if (burstShotsDone >= shotsPerBurst)
                {
                    bossAttackPhase = BossAttackPhase.Waiting;
                    phaseTimer = 0f;
                    nextWaitTime = Random.Range(minWaitTime, maxWaitTime);
                }
                break;

            case BossAttackPhase.Moving:
                // opcional: agregar log si querés ver el movimiento entre ráfagas
                break;
        }
    }


    // 🔹 Disparo que ignora FireRate
    private void FireBossShotImmediate()
    {
        if (weaponController == null) return;
        weaponController.FireImmediate(); // dispara ignorando FireRate
        Debug.Log($"[Boss] FireImmediate ejecutado en {Time.time:F2}s");
    }

    private void ShowDamageText(int totalDamage, bool isCritical)
    {
        if (damageTextPrefab == null) return;

        Vector3 offset = Vector3.up * 0.6f * transform.localScale.y;
        GameObject dmgText = Instantiate(damageTextPrefab, transform.position + offset, Quaternion.identity);

        var floating = dmgText.GetComponent<FloatingDamageText>();
        if (floating != null)
            floating.SetDamage(totalDamage, isCritical);

        // 🔹 Opcional: hacer que mire siempre a la cámara
        dmgText.transform.forward = Camera.main.transform.forward;
    }


    public void OnDeathAnimationFinished() { Destroy(gameObject); }

    public void SetVulnerable(bool value)
    {
        IsVulnerable = value;

        // Opcional: cambiar color / animación para que el jugador vea que es vulnerable
        // ejemplo: material.color = value ? Color.red : Color.white;
    }

}
