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
    [SerializeField] private float fireDelay = 1f;

    [Header("Boss")]
    [SerializeField] private bool isBoss = false;

    private int currentHealth;
    private float fireTimer;
    private bool isDead;

    private EnemigoVisual enemigoVisual;
    private Collider2D col;
    private MovementController movementController;
    [HideInInspector] public GameObject prefab; // Prefab original
    [HideInInspector] public MovementPatternDefinition movementPattern; // Patrón de movimiento original


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
        if (isDead)
            return;

        if (weaponController != null && State == EnemyState.Attacking)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireDelay)
            {
                weaponController.Fire();
                fireTimer = 0f;
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        ModifyHealth(-damageAmount);

        enemigoVisual?.PlayHitEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ModifyHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        col.enabled = false;

        OnEnemyDied?.Invoke();

        enemigoVisual?.PlayDeath();
        CreateScorePickup();
        TryCreateBonusPickup();

        LevelManager.Instance.OnEnemyKilled();
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }

    private void CreateScorePickup()
    {
        int count = Random.Range(1, 4);
        for (int i = 0; i < count; i++)
        {
            Vector3 offset = new Vector3(i + 1, -i, 0);
            Instantiate(scorePickupPrefab, transform.position + offset, Quaternion.identity);
        }
    }

    private void TryCreateBonusPickup()
    {
        if (bonusPickupPrefab == null) return;
        if (Random.value > dropProbability) return;

        if (BonusManager.Instance.IsBonusActive(bonusPickupPrefab.GetBonusDefinition()))
            return;

        Instantiate(bonusPickupPrefab, transform.position, Quaternion.identity);
    }

    public void SetState(EnemyState newState)
    {
        State = newState;
    }


}
