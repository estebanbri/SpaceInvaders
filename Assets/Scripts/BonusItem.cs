using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusItem : MonoBehaviour
{
    [SerializeField] private Image bonusIcon;
    [SerializeField] private TextMeshProUGUI bonusLabel;

    [Header("Buy")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Image buyIcon;
    [SerializeField] private TextMeshProUGUI buyText;

    private BonusDefinition bonus;
    private Action<BonusDefinition> onSelected;

    public void Setup(
        BonusDefinition bonusDefinition,
        Action<BonusDefinition> onSelectedCallback
    )
    {
        bonus = bonusDefinition;
        onSelected = onSelectedCallback;

        bonusIcon.sprite = bonus.icon;
        bonusLabel.text = bonus.displayName.ToUpper(); ;

        buyText.text = "BUY (" + bonus.starCost.ToString() + ")";

        bool canAfford = GameManager.Instance.CanAfford(bonus.starCost);

        buyButton.interactable = canAfford;

        buyText.color = canAfford ? Color.black : Color.red;
        buyIcon.color = canAfford
            ? Color.white
            : new Color(1f, 1f, 1f, 0.4f);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void OnBuyClicked()
    {
        onSelected?.Invoke(bonus);
    }
}
