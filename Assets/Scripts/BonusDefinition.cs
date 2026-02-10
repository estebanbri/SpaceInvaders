using UnityEngine;

public abstract class BonusDefinition : ScriptableObject
{
    [Header("UI")]
    public Sprite icon;
    public string displayName;

    [Header("Picker")]
    public int starCost = 1;
    public bool canAppearInPicker = true;

    [Header("Behaviour")]
    public bool isTemporary;
    public float duration;

    public abstract void Apply();
    public abstract void Remove();
}
