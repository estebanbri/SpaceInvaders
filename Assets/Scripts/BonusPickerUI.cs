using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPickerUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private BonusItem buttonPrefab;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.25f;
    [SerializeField] private Vector3 hiddenScale = new Vector3(0.7f, 0.7f, 1f);


    private System.Action<BonusDefinition> onSelectCallback;
    private Coroutine animCoroutine;


    public void Open(
        List<BonusDefinition> bonuses,
        System.Action<BonusDefinition> onSelect
    )
    {
        gameObject.SetActive(true);

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
        bool paid = GameManager.Instance.SpendScore(bonus.starCost);
        if (!paid)
            return;

        onSelectCallback?.Invoke(bonus);

        Close();
        LevelManager.Instance.ContinueAfterBonus();
    }

    public void Close()
    {
        PlayAnim(false);
        gameObject.SetActive(false);
    }

    void PlayAnim(bool show)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animCoroutine = StartCoroutine(AnimatePanel(show));
    }

    IEnumerator AnimatePanel(bool show)
    {

        //  MUY IMPORTANTE
        // esperar 1 frame para que EventSystem recalcule hover
        yield return null;

        float t = 0f;


        Vector3 fromScale = transform.localScale;
        Vector3 toScale = show ? Vector3.one : hiddenScale;

        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / animDuration);
            float eased = Mathf.SmoothStep(0, 1, p);

            transform.localScale = Vector3.Lerp(fromScale, toScale, eased);

            yield return null;
        }

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
