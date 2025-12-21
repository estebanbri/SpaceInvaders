using UnityEngine;

public class Enemigo : MonoBehaviour, IDamageable
{
    [SerializeField] private PickupBonus bonusPickupPrefab;
    [SerializeField] private GameObject scorePickupPrefab;
    [SerializeField] private int health = 100;
    [SerializeField] private MovementPattern movement;
    [SerializeField] private WeaponController weaponController;
    [Range(0,1)]
    [SerializeField] private float dropProbability;
    private float time;
    private Vector3 startPos;
    private bool isDead;
    private EnemigoVisual enemigoVisual;
    private Collider2D col;
    private bool canAttack;
    private bool movementEnabled = true;

    void Start() {
        startPos = transform.position;
        movement.Init(startPos, Nave.Instance.transform.position);
    }

    void Awake() {
        enemigoVisual = GetComponentInChildren<EnemigoVisual>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (movementEnabled)
        {
            time += Time.deltaTime;
            transform.position = startPos + movement.Evaluate(time);
        }
        if (canAttack && !isDead)
        {
            // disparar, cambiar fases, etc
            Debug.Log("Habilitado para disparar");
            weaponController.Fire();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        health -= damageAmount;

        enemigoVisual.PlayHitEffect();

        if (health <= 0)
        {
            Die();
        }
    }

    public void EnableCombat()
    {
        canAttack = true;
    }

    public void StopMovement()
    {
        movementEnabled = false;
    }

    void Die() {
        isDead = true;
        col.enabled= false; 
        enemigoVisual.PlayDeath();
        CreateScorePickup();
        TryCreateBonusPickup();
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
        float ran = Random.value;
        if (Random.value <= dropProbability)
        {
            if (BonusManager.Instance.IsBonusActive(bonusPickupPrefab.GetBonusDefinition())) {
                return;
            }
            Instantiate(bonusPickupPrefab, transform.position, Quaternion.identity);
        }
    }

}
