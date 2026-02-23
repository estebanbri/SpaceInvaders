using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Enemigo), typeof(Collider2D))]
public class BossFixed : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemigo enemigoComponent;
    [SerializeField] private EnemigoVisual enemigoVisualComponent;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject fuegoPrefab;
    [SerializeField] private GameObject muzzleDer;
    [SerializeField] private GameObject muzzleIzq;

    [Header("Attack Settings")]
    [SerializeField] private int maxCombo = 2;
    [SerializeField] private int damageAmountToPlayer = 1;

    private Transform player;
    private GameObject currentIndicator;

    private int comboRemaining;

    private void Awake()
    {
        if (enemigoComponent == null)
            enemigoComponent = GetComponent<Enemigo>();

        enemigoComponent.SetVulnerable(false);
    }

    private void Start()
    {
        player = Nave.Instance.transform;

        enemigoComponent.OnEnemyDied += OnBossDied;

        enemigoVisualComponent.OnPreAttackStarted += HandlePreAttackStarted;
        enemigoVisualComponent.OnPreAttackFinished += HandlePreAttackFinished;
        enemigoVisualComponent.OnAttackFinished += HandleAttackFinished;
        enemigoVisualComponent.OnRecoverStarted += HandleRecoverStarted;
        enemigoVisualComponent.OnRecoverFinished += HandleRecoverFinished;
        enemigoVisualComponent.OnIdleFinished += HandleIdleFinished;
        enemigoVisualComponent.OnAttackFrame += PerformAttack;
    }

    public void HandlePreAttackStarted()
    {
        SpawnIndicator();
    }

    public void HandleIdleFinished()
    {

        enemigoVisualComponent.PlayPreAttack();
        StartNewCombo();
    }

    private void StartNewCombo()
    {
        comboRemaining = Random.Range(1, maxCombo + 1);
        StartPreAttack();
    }

    private void StartPreAttack()
    {
        
        SpawnIndicator();
    }

    private void HandlePreAttackFinished()
    {
        enemigoVisualComponent.PlayAttack();
    }

    private void HandleAttackFinished()
    {
        enemigoVisualComponent.PlayRecover();
    }

    private void HandleRecoverStarted()
    {
        enemigoComponent.SetVulnerable(true);
    }

    private void HandleRecoverFinished()
    {
        enemigoComponent.SetVulnerable(false);

        comboRemaining--;


        if (comboRemaining > 0)
        {
            StartPreAttack();
        }
    }
    private void SpawnIndicator()
    {
        if (currentIndicator != null)
            Destroy(currentIndicator);

        Vector3 spawnPos = player.position;
        spawnPos.z = 0;

        currentIndicator = Instantiate(indicatorPrefab, spawnPos, Quaternion.identity);
    }

    private void PerformAttack()
    {
        FireFlash();

        if (fuegoPrefab != null && currentIndicator != null)
        {
            GameObject fuegoInstance = Instantiate(
                fuegoPrefab,
                currentIndicator.transform.position,
                Quaternion.identity
            );

            ParticleSystem ps = fuegoInstance.GetComponent<ParticleSystem>();

            if (ps != null)
                Destroy(fuegoInstance, ps.main.duration);
            else
                Destroy(fuegoInstance, 1f);
        }

        DetectPlayerHit();

        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
    }

    private void DetectPlayerHit()
    {
        if (currentIndicator == null) return;

        CircleCollider2D circle = currentIndicator.GetComponent<CircleCollider2D>();
        if (circle == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            circle.bounds.center,
            circle.radius
        );

        foreach (var hit in hits)
        {
            Nave nave = hit.GetComponent<Nave>();
            if (nave != null)
            {
                nave.TakeDamage(damageAmountToPlayer, transform.position);
                break;
            }
        }
    }

    public void OnIdleFinished()
    {
        if (!enemigoComponent.IsDead)
            return;

        StartNewCombo();
    }

    private void FireFlash()
    {
        StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine()
    {
        if (muzzleDer != null) muzzleDer.SetActive(true);
        if (muzzleIzq != null) muzzleIzq.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        if (muzzleDer != null) muzzleDer.SetActive(false);
        if (muzzleIzq != null) muzzleIzq.SetActive(false);
    }

    private void OnBossDied()
    {
        StopAllCoroutines();

        if (currentIndicator != null)
            Destroy(currentIndicator);
    }

    private void OnDestroy()
    {
        if (enemigoComponent != null)
            enemigoComponent.OnEnemyDied -= OnBossDied;

        if (enemigoVisualComponent != null)
        {
            enemigoVisualComponent.OnPreAttackFinished -= HandlePreAttackFinished;
            enemigoVisualComponent.OnAttackFinished -= HandleAttackFinished;
            enemigoVisualComponent.OnRecoverStarted -= HandleRecoverStarted;
            enemigoVisualComponent.OnRecoverFinished -= HandleRecoverFinished;
            enemigoVisualComponent.OnAttackFrame -= PerformAttack;
        }
    }
}