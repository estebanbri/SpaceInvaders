using UnityEngine;

public abstract class BonusDefinition : ScriptableObject
{
    [Header("UI")]
    public Sprite icon;

    [Header("Behaviour")]
    public bool isTemporary;
    public float duration;

    [Header("Picker")]
    public bool canAppearInPicker = true;

    public abstract void Apply();
    public abstract void Remove();
}