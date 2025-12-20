using UnityEngine;

public abstract class BonusDefinition : ScriptableObject
{
    [Header("UI")]
    public Sprite icon;

    [Header("Behaviour")]
    public bool isTemporary;
    public float duration;

    public abstract void Apply();
    public abstract void Remove();
}