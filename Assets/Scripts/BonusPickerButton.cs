using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusPickerButton : MonoBehaviour
{
    [SerializeField] private Image icon;

    private BonusDefinition bonus;
    private System.Action<BonusDefinition> callback;

    public void Setup(
        BonusDefinition bonus,
        System.Action<BonusDefinition> onClick
    )
    {
        this.bonus = bonus;
        this.callback = onClick;

        icon.sprite = bonus.icon;
    }

    public void OnClick()
    {
        callback?.Invoke(bonus);
    }
}