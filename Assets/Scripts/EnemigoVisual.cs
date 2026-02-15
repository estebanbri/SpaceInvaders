using System.Collections;
using UnityEngine;

public class EnemigoVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Enemigo enemigo;

    private Material material;
    private Coroutine hitCoroutine;
    private float flashDuration = 0.03f;
    private Vector3 knockbackOffset;

    [Header("Knockback")]
    [SerializeField] private float knockbackRecoverSpeed = 8f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemigo = GetComponentInParent<Enemigo>();
        material = spriteRenderer.material;

    }

    private void Update()
    {
        // Recuperación suave hacia cero
        knockbackOffset = Vector3.Lerp(knockbackOffset,Vector3.zero,Time.deltaTime * knockbackRecoverSpeed);
        transform.localPosition = knockbackOffset;
    }

    public void AddKnockback(Vector3 direction, float force)
    {
        knockbackOffset += direction.normalized * force;
        Debug.Log("knockbackOffset: " + knockbackOffset);
    }

    public Vector3 GetKnockbackOffset()
    {
        return knockbackOffset;
    }


    /// <summary>
    /// Reproduce el efecto de recibir daño: flash y activación de trigger.
    /// </summary>
    public void PlayHitEffect(Vector3 hitDir)
    {
        AddKnockback(hitDir, 0.6f);

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        // Flash blanco
        material.SetFloat("_Flash", 1f);
        yield return new WaitForSeconds(flashDuration);
        // Vuelve a normal
        material.SetFloat("_Flash", 0f);
    }

    public void PlayDeath()
    {
        animator.SetBool("IsDead", true);
    }

    public void OnDeathAnimationFinished()
    {
        enemigo.OnDeathAnimationFinished();
    }
}
