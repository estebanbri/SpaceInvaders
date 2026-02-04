using UnityEngine;

public class Enemigo : MonoBehaviour, IDamageable
{
    [SerializeField] private PickupBonus bonusPickupPrefab;
    [SerializeField] private GameObject scorePickupPrefab;
    [SerializeField] private int health = 100;
    [SerializeField] private WeaponController weaponController;
    [Range(0,1)]
    [SerializeField] private float dropProbability;
    [SerializeField] private float fireDelay;

    private float fireTimer;
    private bool isDead;
    private EnemigoVisual enemigoVisual;
    private Collider2D col;
    private MovementPatternController movement;
    public EnemyState State { get; private set; }

    void Awake() {
        enemigoVisual = GetComponentInChildren<EnemigoVisual>();
        col = GetComponent<Collider2D>();
        movement = GetComponent<MovementPatternController>();
        movement.OnPatternChanged += HandlePatternChange;
        SetState(EnemyState.Entering);
    }

    void Update()
    {
        if (weaponController == null || State != EnemyState.Attacking || isDead)
            return;

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

        health -= damageAmount;

        if (enemigoVisual != null)
        enemigoVisual.PlayHitEffect();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die() {
        isDead = true;
        col.enabled= false; 
        enemigoVisual.PlayDeath();
        CreateScorePickup();
        TryCreateBonusPickup();
        LevelManager.Instance.OnEnemyKilled();
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }

    void CreateScorePickup() {
        for (int i = 0; i < Random.Range(1, 4); i++) {
            Instantiate(scorePickupPrefab, new Vector3(transform.position.x + i + 1, transform.position.y - i, 0)  , Quaternion.identity);
        }
    }

    void TryCreateBonusPickup()
    {
        if (Random.value <= dropProbability)
        {
            if (BonusManager.Instance.IsBonusActive(bonusPickupPrefab.GetBonusDefinition())) {
                return;
            }
            Instantiate(bonusPickupPrefab, transform.position, Quaternion.identity);
        }
    }

    public void SetState(EnemyState newState)
    {
        State = newState;
    }

    void HandlePatternChange(int patternIndex)
    {
        // Ejemplo de reglas
        if (patternIndex % 2 == 0)
            SetState(EnemyState.Attacking);
        else
            SetState(EnemyState.Moving);
    }
}
