using System.Collections;
using UnityEngine;
using System;

public class EnemigoVisual : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private Animator animator;

    [SerializeField] private Enemigo enemigo;

    public Action OnAttackFrame;

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
        StartCoroutine(BossDeathSequence());
    }

    private IEnumerator BossDeathSequence()
    {
        // 1️⃣ Mini freeze dramático
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.08f);
        Time.timeScale = 1f;

        // 2️⃣ Flash fuerte
        SetFlash(1f);
        yield return new WaitForSeconds(0.15f);
        SetFlash(0f);

        // 3️⃣ Desactivar animator si existe
        if (animator != null)
            animator.enabled = false;

        // 4️⃣ Explosiones pequeñas distribuidas
        for (int i = 0; i < smallExplosionCount; i++)
        {
            SpawnSmallExplosion();
            yield return new WaitForSeconds(0.15f);
        }

        // 5️⃣ Apagar partes visuales
        foreach (var sr in spriteRenderers)
            sr.enabled = false;

        // 6️⃣ Explosión grande final
        if (bigExplosionPrefab != null)
            Instantiate(bigExplosionPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.8f);

        // 7️⃣ Destruir enemigo real
        if (enemigo != null)
            Destroy(enemigo.gameObject);
    }

    private void SpawnSmallExplosion()
    {
        if (smallExplosionPrefab == null) return;

        Vector3 randomOffset = UnityEngine.Random.insideUnitCircle * 2f;
        Instantiate(smallExplosionPrefab,
                    transform.position + randomOffset,
                    Quaternion.identity);
    }

    #endregion

    public void PerformAttack()
    {
        OnAttackFrame?.Invoke();
    }
}
