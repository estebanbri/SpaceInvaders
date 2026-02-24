using System.Collections;
using UnityEngine;
using System;

public class EnemigoVisual : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private Animator animator;

    [SerializeField] private Enemigo enemigo;

    public Action OnAttackFrame;

    // 🔵 NUEVOS EVENTOS (NO QUITA NADA EXISTENTE)
    public Action OnPreAttackFinished;
    public Action OnAttackFinished;
    public Action OnRecoverFinished;
    public Action OnRecoverStarted;
    public Action OnIdleFinished;
    public Action OnPreAttackStarted;

    [Header("Knockback")]
    [SerializeField] private float knockbackRecoverSpeed = 8f;
    [SerializeField] private float knockbackForce = 0.3f;

    [Header("Boss Death FX")]
    [SerializeField] private GameObject smallExplosionPrefab;
    [SerializeField] private GameObject bigExplosionPrefab;
    [SerializeField] private int smallExplosionCount = 6;
    [SerializeField] private float deathDuration = 2f;

    private Coroutine hitCoroutine;
    private float flashDuration = 0.03f;
    private Vector3 knockbackOffset;
    private bool isDying = false;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enemigo != null && !enemigo.IsBoss && !isDying)
        {
            knockbackOffset = Vector3.Lerp(
                knockbackOffset,
                Vector3.zero,
                Time.deltaTime * knockbackRecoverSpeed);

            transform.localPosition = knockbackOffset;
        }
    }

    #region HIT EFFECT

    public void PlayHitEffect(Vector3 hitDir)
    {
        if (isDying) return;

        AddKnockback(hitDir, knockbackForce);

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        SetFlash(1f);
        yield return new WaitForSeconds(flashDuration);
        SetFlash(0f);
    }

    private void SetFlash(float value)
    {
        foreach (var sr in spriteRenderers)
        {
            if (sr != null && sr.material.HasProperty("_Flash"))
                sr.material.SetFloat("_Flash", value);
        }
    }

    public void AddKnockback(Vector3 direction, float force)
    {
        knockbackOffset += direction.normalized * force;
    }

    #endregion

    #region DEATH SEQUENCE

    public void PlayDeath()
    {
        if (isDying) return;

        isDying = true;

        if (enemigo.IsBoss)
        {
            StartCoroutine(BossDeathSequence());
        }
        else
        {
            StartCoroutine(PlayDeathForNonBoss());
        }

        // animator.SetBool("IsDead", true);
    }

    private IEnumerator PlayDeathForNonBoss()
    {
        SpawnSmallExplosion();
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator BossDeathSequence()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = 1f;

        SetFlash(1f);
        yield return new WaitForSeconds(0.15f);
        SetFlash(0f);

        if (animator != null)
            animator.enabled = false;

        for (int i = 0; i < smallExplosionCount; i++)
        {
            SpawnSmallExplosion();
            yield return new WaitForSeconds(0.15f);
        }

        foreach (var sr in spriteRenderers)
            sr.enabled = false;

        if (bigExplosionPrefab != null)
            Instantiate(bigExplosionPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.8f);

        if (enemigo != null)
            Destroy(enemigo.gameObject);
    }

    private GameObject SpawnSmallExplosion()
    {
        if (smallExplosionPrefab == null) return null;

        GameObject explosion = Instantiate(smallExplosionPrefab, transform.position, Quaternion.identity);
        return explosion;
    }

    #endregion

    public void PerformAttack()
    {
        OnAttackFrame?.Invoke();
    }

    public void OnRecoverAnimation()
    {
        enemigo.SetVulnerable(!enemigo.IsVulnerable);
    }

    // 🔵 NUEVOS MÉTODOS PARA SINCRONIZAR CON BOSS FIXED
    // (SE USAN COMO ANIMATION EVENTS)

    public void PreAttackFinished()
    {
        OnPreAttackFinished?.Invoke();
    }

    public void IdleFinished()
    {
        OnIdleFinished?.Invoke();
    }

    public void AttackFinished()
    {
        OnAttackFinished?.Invoke();
    }

    public void RecoverStarted()
    {
        OnRecoverStarted?.Invoke();
    }

    public void RecoverFinished()
    {
        OnRecoverFinished?.Invoke();
    }

    public void PreAttackStarted()
    {
        OnPreAttackStarted?.Invoke();
    }

    public void PlayPreAttack()
    {
        animator.SetTrigger("PreAttack");
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void PlayRecover()
    {
        animator.SetTrigger("Recover");
    }
}