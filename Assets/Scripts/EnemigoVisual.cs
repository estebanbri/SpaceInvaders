using System.Collections;
using UnityEngine;

public class EnemigoVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Enemigo enemigo;

    private Material material;
    private Coroutine hitCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemigo = GetComponentInParent<Enemigo>();

        // IMPORTANTE: instanciamos el material
        material = spriteRenderer.material;
    }

    public void PlayHitEffect()
    {
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        // Flash blanco
        material.SetFloat("_Flash", 1f);
        yield return new WaitForSeconds(0.03f);

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
