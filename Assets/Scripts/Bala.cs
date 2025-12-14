using UnityEngine;

public class Bala : MonoBehaviour
{

    [SerializeField] private int damageAmount;

    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemigo>(out Enemigo enemigo))
        {
            enemigo.TakeDamage(damageAmount);
            Destroy(gameObject);
        }
        if (collision.gameObject.TryGetComponent<Asteroide>(out Asteroide asteroide))
        {
            asteroide.TakeDamage(damageAmount);
            Destroy(gameObject);
        }
    }

   
}
