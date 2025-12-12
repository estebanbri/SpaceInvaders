using UnityEngine;

public class Asteroide : MonoBehaviour, IDamageable
{
    [SerializeField] float lateralForce = 3f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // fuerzas aleatorias hacia la derecha o izquierda
        float dir = Random.Range(-1f, 1f);

        rb.gravityScale = Random.Range(0.005f, 0.5f);
        rb.AddForce(new Vector2(dir * lateralForce, 0), ForceMode2D.Impulse);

        // rotación
        rb.AddTorque(dir, ForceMode2D.Impulse);
    }

    public void TakeDamage()
    {
        // sonido, animación, partículas, etc.
        Destroy(gameObject);
    }
}