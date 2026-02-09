using System.Collections;
using UnityEngine;

public class TorretaCannonVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Material material;
    private Color originalColor;
    private Coroutine hitCoroutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Cada torreta tiene su propio material
        material = spriteRenderer.material;
        originalColor = spriteRenderer.color;
    }

    public void PlayHitEffect()
    {
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        // Flash blanco (impacto)
        material.SetFloat("_Flash", 1f);
        yield return new WaitForSeconds(0.025f);

        material.SetFloat("_Flash", 0f);

        // Rojo muy breve (daño)
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.04f);

        spriteRenderer.color = originalColor;
    }

    public void PlayDeathEffect()
    {
        animator.SetBool("IsDead", true);
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}