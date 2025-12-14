using UnityEngine;

public class EnemigoVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Enemigo enemigo;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemigo = GetComponentInParent<Enemigo>();
    }

    public void PlayHitEffect()
    {
        spriteRenderer.color = Color.red;
        Invoke(nameof(ResetColor), 0.1f);
    }

    void ResetColor()
    {
        spriteRenderer.color = Color.white;
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
