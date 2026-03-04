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
    [SerializeField] private bool applyKnockback = false;
    [SerializeField] private float knockbackRecoverSpeed = 8f;
    [SerializeField] private float knockbackForce = 0.12f;

    [Header("Boss Death FX")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int explosionCount = 6;

    private Coroutine hitCoroutine;
    private float flashDuration = 0.03f;
    private Vector3 knockbackOffset;
    private bool isDying = false;

    private Vector3 recoilOffset;

    private WeaponController weaponController;

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        animator = GetComponent<Animator>();
        weaponController = GetComponentInParent<Enemigo>().GetWeaponController;
        if (weaponController != null) weaponController.GetCurrentWeapon().OnWeaponFired += HandleWeaponFired;
    }

    void Update()
    {
        if (enemigo != null && !enemigo.IsBoss && !isDying)
        {
            knockbackOffset = Vector3.Lerp(
                                knockbackOffset,
                                Vector3.zero,
                                Time.deltaTime * knockbackRecoverSpeed);

            recoilOffset = Vector3.Lerp(
                recoilOffset,
                Vector3.zero,
                Time.deltaTime * 12f);

            transform.localPosition = knockbackOffset + recoilOffset;
        }
    }

    private void HandleWeaponFired()
    {
        PlayRecoil();
    }

    private void PlayRecoil()
    {
        recoilOffset += Vector3.up * 0.1f;
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
        if (!applyKnockback) return; // 🔥 BLOQUEO REAL
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
        Destroy(transform.root.gameObject);
    }

    private IEnumerator PlayDeathForNonBoss()
    {
        // Desactivar sprite inmediatamente
        foreach (var sr in spriteRenderers)
            sr.enabled = false;

        SpawnExplosion();

        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator BossDeathSequence()
    {

        for (int i = 0; i < explosionCount; i++)
        {
            SpawnExplosion(1.5f);
            yield return new WaitForSeconds(0.15f);
        }

        foreach (var sr in spriteRenderers)
        {
            sr.sortingOrder = -10; // o algo menor que la explosión
            sr.enabled = false;
        }
    }

    private GameObject SpawnExplosion(float radius = 0f)
    {
        if (explosionPrefab == null) return null;

        Vector3 spawnPosition = transform.position;

        if (radius > 0f)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * radius;
            spawnPosition += (Vector3)randomOffset;
        }

        return Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
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