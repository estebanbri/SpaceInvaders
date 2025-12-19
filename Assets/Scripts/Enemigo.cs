using UnityEngine;

public class Enemigo : MonoBehaviour, IDamageable
{
    [SerializeField] private int deathScore;
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private int health = 100;
    [SerializeField] private MovementPattern movement;
    [Range(0,1)]
    [SerializeField] private float dropProbability;
    private float time;
    private Vector3 startPos;
    private bool isDead;
    private EnemigoVisual enemigoVisual;
    private Collider2D col;

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
        time += Time.deltaTime;
        transform.position = startPos + movement.Evaluate(time);
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

    void Die() {
        isDead = true;
        col.enabled= false; 
        enemigoVisual.PlayDeath();
        GameManager.Instance.AddScore(deathScore);
        TryCreatePickup();
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }

    void TryCreatePickup()
    {
        float ran = Random.value;
        if (Random.value <= dropProbability)
        {
            Instantiate(pickupPrefab, transform.position, Quaternion.identity);
        }
    }

}
