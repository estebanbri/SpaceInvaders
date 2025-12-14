using System.Collections;
using UnityEngine;

public class Nave : MonoBehaviour
{
    [SerializeField] private Bala balaPrefab;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float fireRate = 0.1f;
    private float screenMinX = -8f;
    private float screenMaxX = 8f;
    private float screenMinY = -4.5f;
    private float screenMaxY = 4.5f;
    private float nextFireTime;
    private bool isDead;
    private Collider2D colliderComponent;
    private NaveVisual naveVisualComponent;

    private void Awake()
    {
        colliderComponent = GetComponent<Collider2D>();
        naveVisualComponent = GetComponentInChildren<NaveVisual>();
    }
    void Update()
    {
        if (isDead) return;
        naveVisualComponent.HidePropulsoresParticles();
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Disparar();
        }

        HandleMovimientoHorizontal();
        HandleMovimientoVertical();
    }

    void HandleMovimientoHorizontal()
    {
        float moveX = Input.GetAxis("Horizontal");
        naveVisualComponent.UpdateHorizontalThrusters(moveX);
        transform.Translate(moveX * speed * Time.deltaTime, 0, 0);
        // Limitar el movimiento dentro de los bordes de la pantalla en X
        float clampedX = Mathf.Clamp(transform.position.x, screenMinX, screenMaxX);
        transform.position = new Vector3(clampedX, transform.position.y, 0);
    }

    void HandleMovimientoVertical()
    {
        float moveY = Input.GetAxis("Vertical");
        naveVisualComponent.UpdateVerticalThrusters(moveY);
        transform.Translate(0, moveY * speed * Time.deltaTime, 0);
        // Limitar el movimiento dentro de los bordes de la pantalla en Y
        float clampedY = Mathf.Clamp(transform.position.y, screenMinY, screenMaxY);
        transform.position = new Vector3(transform.position.x, clampedY, 0);
    }


    void Disparar() { 
        Instantiate(balaPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.TryGetComponent<Enemigo>(out Enemigo enemigo)
            || collision.gameObject.TryGetComponent<Asteroide>(out Asteroide asteroide))
        {
            Morir();
        }
    }

    void Morir() {
        isDead = true;
        GameManager.Instance.DecreaseRetry();
        if (GameManager.Instance.HasPendingRetries())
        {
            Respawn();
        }
        else
        {
            Destroy(gameObject);
            GameManager.Instance.GameOver();
        }
    }

    void Respawn() {
        StartCoroutine(InvulneravilityCoroutine());
        isDead = false;
    }

    IEnumerator InvulneravilityCoroutine()
    {
        colliderComponent.enabled = false;
        yield return StartCoroutine(naveVisualComponent.BlinkSpriteDuringInvulnerabilityCoroutine());
        colliderComponent.enabled = true;
    }

    
}
