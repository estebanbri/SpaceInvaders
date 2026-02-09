using UnityEngine;
using System.Collections.Generic;

public class BonusPickerManager : MonoBehaviour
{
    public static BonusPickerManager Instance;

    [Header("Config")]
    public List<BonusDefinition> pickerPool;
    public int optionsAmount = 3;

    [Header("UI")]
    public BonusPickerUI pickerUI;

    void Awake()
    {
        Instance = this;
    }

    public void OpenPicker()
    {
        var options = BonusManager.Instance.GetPickableBonuses(
            pickerPool,
            optionsAmount
        );

        if (options.Count == 0)
            return;

        Time.timeScale = 0f;
        pickerUI.Open(options, OnBonusSelected);
    }

    void OnBonusSelected(BonusDefinition bonus)
    {
        Time.timeScale = 1f;
        BonusManager.Instance.ApplyBonus(bonus);
        LevelManager.Instance.ContinueAfterBonus();
    }
}
