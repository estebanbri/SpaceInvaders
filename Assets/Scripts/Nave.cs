using UnityEngine;

public class Nave : MonoBehaviour
{
    [SerializeField] private Bala balaPrefab;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float fireRate = 0.1f;
    private float minX = -8f;
    private float maxX = 8f;
    private float nextFireTime = 0f;

    void Update()
    {
        
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Disparar();
        }

        float h = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector2.right * h * speed * Time.deltaTime);

        // Limitar a los bordes
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, 0);

    }

    void Disparar() { 
        Instantiate(balaPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemigo>(out Enemigo enemigo)
            || collision.gameObject.TryGetComponent<Asteroide>(out Asteroide asteroide))
        {
            Debug.Log("Game Over!");
            Destroy(gameObject);
        }
    }
}
