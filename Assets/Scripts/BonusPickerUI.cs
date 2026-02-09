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

    // Escala inicial: comprimido horizontalmente
    [SerializeField] private Vector3 hiddenScale = new Vector3(0f, 0.9f, 1f);

    private System.Action<BonusDefinition> onSelectCallback;
    private Coroutine animCoroutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // Estado inicial oculto
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

        PlayAnim(show: true);
    }

    void OnBonusClicked(BonusDefinition bonus)
    {
        onSelectCallback?.Invoke(bonus);
        Close();
    }

    public void Close()
    {
        PlayAnim(show: false);
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

        if (show)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / animDuration);

            // easing suave
            float eased = Mathf.SmoothStep(0f, 1f, p);

            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);

            //  expansión horizontal desde el centro
            transform.localScale = new Vector3(
                Mathf.Lerp(fromScale.x, toScale.x, eased),
                Mathf.Lerp(fromScale.y, toScale.y, eased),
                1f
            );

            yield return null;
        }

        canvasGroup.alpha = toAlpha;
        transform.localScale = toScale;

        if (!show)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    void ClearButtons()
    {
        for (int i = optionsContainer.childCount - 1; i >= 0; i--)
            Destroy(optionsContainer.GetChild(i).gameObject);
    }
}
