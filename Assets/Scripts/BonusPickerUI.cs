using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPickerUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private BonusPickerButton buttonPrefab;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.25f;
    [SerializeField] private Vector3 hiddenScale = new Vector3(0.7f, 0.7f, 1f);

    private System.Action<BonusDefinition> onSelectCallback;
    private Coroutine animCoroutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // estado inicial oculto
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        transform.localScale = hiddenScale;
    }

    public void Open(
        List<BonusDefinition> bonuses,
        System.Action<BonusDefinition> onSelect
    )
    {
        onSelectCallback = onSelect;

        ClearButtons();

        foreach (var bonus in bonuses)
        {
            var button = Instantiate(buttonPrefab, optionsContainer);
            button.Setup(bonus, OnBonusClicked);
        }

        PlayAnim(true);
    }

    void OnBonusClicked(BonusDefinition bonus)
    {
        // pagar estrellas
        bool paid = GameManager.Instance.SpendScore(bonus.starCost);
        if (!paid)
            return;

        onSelectCallback?.Invoke(bonus);

        Close();

        // continuar waves
        LevelManager.Instance.ContinueAfterBonus();
    }

    public void Close()
    {
        PlayAnim(false);
    }

    void PlayAnim(bool show)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animCoroutine = StartCoroutine(AnimatePanel(show));
    }

    IEnumerator AnimatePanel(bool show)
    {
        float t = 0f;

        float fromAlpha = canvasGroup.alpha;
        float toAlpha = show ? 1f : 0f;

        Vector3 fromScale = transform.localScale;
        Vector3 toScale = show ? Vector3.one : hiddenScale;

        canvasGroup.blocksRaycasts = show;
        canvasGroup.interactable = show;

        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / animDuration);
            float eased = Mathf.SmoothStep(0, 1, p);

            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);
            transform.localScale = Vector3.Lerp(fromScale, toScale, eased);

            yield return null;
        }

        canvasGroup.alpha = toAlpha;
        transform.localScale = toScale;
    }

    void ClearButtons()
    {
        for (int i = optionsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(optionsContainer.GetChild(i).gameObject);
        }
    }
}
