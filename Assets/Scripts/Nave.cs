using System;
using System.Collections;
using UnityEngine;

public class Nave : MonoBehaviour, IDamageable
{
    public static Nave Instance { get; private set; }

    [SerializeField] private float speed = 5f;
    private float screenMinX = -8f;
    private float screenMaxX = 8f;
    private float screenMinY = -4.5f;
    private float screenMaxY = 4.5f;
    private bool isDead;
    private Collider2D colliderComponent;
    private NaveVisual naveVisualComponent;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private Escudo escudo;
    [SerializeField] private BonusDefinition escudobonusDefinition;


    private void Awake()
    {
        Instance = this;
        colliderComponent = GetComponent<Collider2D>();
        naveVisualComponent = GetComponentInChildren<NaveVisual>();
    }

    private void Update()
    {
        if (isDead) return;
        naveVisualComponent.HidePropulsoresParticles();
        HandleInput();
        HandleMovimientoHorizontal();
        HandleMovimientoVertical();
    }

    private void HandleInput() {
        if (Input.GetKey(KeyCode.Space))
        {
            this.weaponController.Fire();
        }
    }

    private void HandleMovimientoHorizontal()
    {
        float moveX = Input.GetAxis("Horizontal");
        naveVisualComponent.AddHorizontalMoveVisual(moveX);
        transform.Translate(moveX * speed * Time.deltaTime, 0, 0);
        // Limitar el movimiento dentro de los bordes de la pantalla en X
        float clampedX = Mathf.Clamp(transform.position.x, screenMinX, screenMaxX);
        transform.position = new Vector3(clampedX, transform.position.y, 0);
    }

    private void HandleMovimientoVertical()
    {
        float moveY = Input.GetAxis("Vertical");
        naveVisualComponent.AddVerticallMoveVisual(moveY);
        transform.Translate(0, moveY * speed * Time.deltaTime, 0);
        // Limitar el movimiento dentro de los bordes de la pantalla en Y
        float clampedY = Mathf.Clamp(transform.position.y, screenMinY, screenMaxY);
        transform.position = new Vector3(transform.position.x, clampedY, 0);
    }

    private void Morir(int cantidadDeVidasQuitadas = 1)
    {
        if (isDead) return;

        isDead = true;

        OnDeath(cantidadDeVidasQuitadas);
    }

    private void OnDeath(int cantidadDeVidasQuitadas)
    {
        bool isDead = GameManager.Instance.OnPlayerDeath(this, cantidadDeVidasQuitadas);
        if (isDead)
        {
            naveVisualComponent.playDeathEffect();
        }
    }

    public void Respawn() {
        StartCoroutine(InvulneravilityCoroutine());
        isDead = false;
    }

    private IEnumerator InvulneravilityCoroutine()
    {
        colliderComponent.enabled = false;
        yield return StartCoroutine(naveVisualComponent.BlinkSpriteDuringInvulnerabilityCoroutine());
        colliderComponent.enabled = true;
    }

    public Escudo GetEscudo() { 
        return this.escudo; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.TryGetComponent<IDamageable>(out _))
        {
            if (escudo.IsActive())
            {
                escudo.RemoveEscudo();
            }
            else
            {
                Morir();
            }
        }
    }

    public WeaponController GetWeaponController()
    {  
        return weaponController; 
    }

    public void TakeDamage(int damageAmount, Vector3? attackerPos, bool isCritical = false)
    {
        if (isDead) return;

        if (escudo.IsActive())
        {
            escudo.PlayEscudoDestroyedEffect();
            return;
        }

        Morir();
    }

    public void OnEscudoAnimationDestroyedFinished()
    {
        BonusManager.Instance.RemoveBonus(escudobonusDefinition);
        escudo.RemoveEscudo();
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}
