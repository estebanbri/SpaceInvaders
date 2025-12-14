using UnityEngine;

public class Enemigo : MonoBehaviour, IDamageable
{   
    private int health = 100;
    private EnemigoVisual enemigoVisual;
    private bool isDead;
    private Collider2D col;
    void Awake() {
        enemigoVisual = GetComponentInChildren<EnemigoVisual>();
        col = GetComponent<Collider2D>();
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
        GameManager.Instance.AddScore(10);
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }

}
