using UnityEngine;

public class Bala : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemigo>(out Enemigo enemigo))
        {
            enemigo.TakeDamage();
            Destroy(gameObject);
            GameManager.Instance.AddScore(10);
        }
        if (collision.gameObject.TryGetComponent<Asteroide>(out Asteroide asteroide))
        {
            asteroide.TakeDamage();
            Destroy(gameObject);
            GameManager.Instance.AddScore(20);
        }
    }

   
}
