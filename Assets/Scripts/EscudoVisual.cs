using UnityEngine;

public class EscudoVisual : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HideEscudo()
    {
        gameObject.SetActive(false);
    }

    public void ShowEscudo()
    {
        gameObject.SetActive(true);
    }

    public void OnDestroyedEffect()
    {
        animator.SetBool("IsDestroyed", true);
    }

    public void OnEscudoDestroyedAnimationFinished()
    {
        Nave.Instance.OnEscudoAnimationDestroyedFinished();
    }
}
