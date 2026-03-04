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
    [SerializeField] private float introHeightOffset = 3f;
    [SerializeField] private float introDuration = 2f;
    [SerializeField] private AnimationCurve introCurve;
    private bool canControl = false;

    public void EnableControl()
    {
        canControl = true;
    }

    private Vector3 introStartPos;
    private Vector3 introTargetPos;
    private float introTimer = 0f;
    private bool playingIntro = true;

    private void Start()
    {
        introTargetPos = transform.position;
        introStartPos = introTargetPos + Vector3.up * introHeightOffset;

        transform.position = introStartPos;
        transform.localScale = Vector3.one * 1.4f;
    }

    private void Awake()
    {
        Instance = this;
        colliderComponent = GetComponent<Collider2D>();
        naveVisualComponent = GetComponentInChildren<NaveVisual>();
    }

    private void Update()
    {
        if (isDead) return;

        if (playingIntro)
        {
            PlayIntro();
            return;
        }

        if (!canControl) return;

        HandleInput();
        HandleMovimientoHorizontal();
        HandleMovimientoVertical();
    }

    private void HandleInput() {
        if (Input.GetKey(KeyCode.Space))
        {
            weaponController.Fire();
        }
    }

    private void PlayIntro()
    {
        introTimer += Time.deltaTime;
        float t = introTimer / introDuration;

        float curveValue = introCurve.Evaluate(t);

        transform.position = Vector3.Lerp(introStartPos, introTargetPos, curveValue);
        transform.localScale = Vector3.Lerp(Vector3.one * 4f, Vector3.one, curveValue);

        if (t >= 1f)
        {
            playingIntro = false;
            EnableControl();
            GameManager.Instance.StartLevel();
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
        // bool isDead = GameManager.Instance.OnPlayerDeath(this, cantidadDeVidasQuitadas);
        naveVisualComponent.playDeathEffect();
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
