using UnityEngine;

public class TorretaCannonVisual : MonoBehaviour

{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    public void playDeathEffect()
    {
        animator.SetBool("IsDead", true);
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}
