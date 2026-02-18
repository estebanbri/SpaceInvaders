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
    [SerializeField] private int indicatorPosY;

    [Header("Attack Settings")]
    [SerializeField] private float warnTime = 0.7f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int maxCombo = 2;
    [SerializeField] private int damageAmountToPlayer = 1;

    [Header("Difficulty Scaling")]
    [SerializeField] private float attackSpeedMultiplier = 1.0f;

    private Transform player;
    private Coroutine attackCoroutine;

    private GameObject currentIndicator;

    private void Awake()
    {
        if (enemigoComponent == null)
            enemigoComponent = GetComponent<Enemigo>();
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
                yield return new WaitForSeconds(0.5f / attackSpeedMultiplier);

                // ---------- INDICADOR ----------
                Vector3 targetPos = new Vector3(player.position.x, indicatorPosY, 0);

                if (currentIndicator != null)
                    Destroy(currentIndicator);

                currentIndicator = Instantiate(indicatorPrefab, targetPos, indicatorPrefab.transform.rotation);

                yield return new WaitForSeconds(warnTime / attackSpeedMultiplier);

                // ---------- ATAQUE ----------
                animator.SetTrigger(Random.value > 0.5f ? "AttackRight" : "AttackLeft");

                yield return new WaitForSeconds(0.7f / attackSpeedMultiplier);

                animator.SetTrigger("Recover");
                yield return new WaitForSeconds(0.7f / attackSpeedMultiplier);
            }
        }
    }

    // ⚠ Se ejecuta por Animation Event
    public void PerformAttack()
    {
        if (currentIndicator == null)
            return;

        BoxCollider2D box = currentIndicator.GetComponent<BoxCollider2D>();
        if (box == null)
            return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            box.bounds.center,
            box.bounds.size,
            0f
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

        if (playerHit)
            Debug.Log("Jugador dentro del rango REAL del indicador!");
        else
            Debug.Log("Jugador esquivó!");

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
            BoxCollider2D box = currentIndicator.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
        }
    }
}
