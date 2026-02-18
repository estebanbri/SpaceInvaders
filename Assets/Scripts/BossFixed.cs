using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Enemigo), typeof(Collider2D))]
public class BossFixed : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Enemigo enemigoComponent;
    [SerializeField] private EnemigoVisual enemigoVisualComponent;
    [SerializeField] private GameObject indicatorPrefab;

    [Header("Attack Settings")]
    [SerializeField] private float warnTime = 0.7f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int maxCombo = 2;
    [SerializeField] private int damageAmountToPlayer = 1;
    [SerializeField] private GameObject fuegoPrefab; // Prefab del Particle System

    [Header("Difficulty Scaling")]
    [SerializeField] private float attackSpeedMultiplier = 1.0f;

    private Transform player;
    private Coroutine attackCoroutine;
    private GameObject currentIndicator;

    private void Awake()
    {
        if (enemigoComponent == null)
            enemigoComponent = GetComponent<Enemigo>();
        enemigoComponent.SetVulnerable(false);
    }

    private void Start()
    {
        player = Nave.Instance.transform;

        enemigoVisualComponent.OnAttackFrame += PerformAttack;
        enemigoComponent.OnEnemyDied += OnBossDied;

        attackCoroutine = StartCoroutine(AttackLoop());
    }

    private void OnBossDied()
    {
        if (currentIndicator != null)
            Destroy(currentIndicator);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }

    private IEnumerator AttackLoop()
    {
        while (enemigoComponent.CurrentHealth > 0)
        {
            yield return new WaitForSeconds(attackCooldown / attackSpeedMultiplier);

            int combo = Random.Range(1, maxCombo + 1);

            for (int i = 0; i < combo; i++)
            {
                animator.SetTrigger("PreAttack");

                // ---------- INDICADOR ----------
                if (currentIndicator != null)
                    Destroy(currentIndicator);

                // Tomamos la posición actual del jugador para fijar el indicador en el piso
                Vector3 spawnPos = player.position;
                spawnPos.z = 0;

                currentIndicator = Instantiate(
                    indicatorPrefab,
                    spawnPos,
                    Quaternion.identity // círculo no necesita rotación
                );

                // Esperamos el warnTime sin mover el indicador
                yield return new WaitForSeconds(warnTime / attackSpeedMultiplier);

                // ---------- ATAQUE ----------
                animator.SetTrigger(Random.value > 0.5f ? "AttackRight" : "AttackLeft");
                yield return new WaitForSeconds(0.7f / attackSpeedMultiplier);

                animator.SetTrigger("Recover");
                Debug.Log("Boss es vulnerable!");
                enemigoComponent.SetVulnerable(true);
                yield return new WaitForSeconds(2f / attackSpeedMultiplier);
                enemigoComponent.SetVulnerable(false);
                Debug.Log("Boss no vulnerable!");

            }
        }
    }


    // ⚠ Se ejecuta por Animation Event
    public void PerformAttack()
    {
        if (currentIndicator == null)
            return;

        // ---------- Instanciar fuego ----------
        if (fuegoPrefab != null)
        {
            GameObject fuego = Instantiate(
                fuegoPrefab,
                currentIndicator.transform.position,
                Quaternion.identity
            );

            ParticleSystem ps = fuego.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(fuego, ps.main.duration);
            else
                Destroy(fuego, 1f);
        }

        // ---------- Detectar jugador ----------
        CircleCollider2D circle = currentIndicator.GetComponent<CircleCollider2D>();
        if (circle == null)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            circle.bounds.center,
            circle.radius
        );

        bool playerHit = false;
        foreach (var hit in hits)
        {
            Nave nave = hit.GetComponent<Nave>();
            if (nave != null)
            {
                nave.TakeDamage(damageAmountToPlayer, transform.position);
                playerHit = true;
                break;
            }
        }

        // ---------- Logs ----------
        if (playerHit)
            Debug.Log("Jugador dentro del rango REAL del indicador!");
        else
            Debug.Log("Jugador esquivó!");

        // ---------- Limpiar indicador ----------
        Destroy(currentIndicator);
        currentIndicator = null;
    }

    public void SetAttackSpeedMultiplier(float multiplier)
    {
        attackSpeedMultiplier = Mathf.Max(0.5f, multiplier);
    }

    private void OnDestroy()
    {
        if (enemigoComponent != null)
            enemigoComponent.OnEnemyDied -= OnBossDied;

        if (enemigoVisualComponent != null)
            enemigoVisualComponent.OnAttackFrame -= PerformAttack;
    }

    private void OnDrawGizmos()
    {
        if (currentIndicator != null)
        {
            CircleCollider2D circle = currentIndicator.GetComponent<CircleCollider2D>();
            if (circle != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(circle.bounds.center, circle.radius);
            }
        }
    }
}
