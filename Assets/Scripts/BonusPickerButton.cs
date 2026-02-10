using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusPickerButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Cost")]
    [SerializeField] private Image starIcon;
    [SerializeField] private TextMeshProUGUI costText;

    [SerializeField] private Button button;

    private BonusDefinition bonus;
    private Action<BonusDefinition> onSelected;

    public void Setup(
        BonusDefinition bonusDefinition,
        Action<BonusDefinition> onSelectedCallback
    )
    {
        bonus = bonusDefinition;
        onSelected = onSelectedCallback;

        icon.sprite = bonus.icon;
        nameText.text = bonus.displayName;
        costText.text = bonus.starCost.ToString();

        bool canAfford = GameManager.Instance.CanAfford(bonus.starCost);

        button.interactable = canAfford;

        costText.color = canAfford ? Color.yellow : Color.red;
        starIcon.color = canAfford
            ? Color.white
            : new Color(1f, 1f, 1f, 0.4f);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        onSelected?.Invoke(bonus);
    }
}
