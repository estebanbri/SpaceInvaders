using UnityEngine;

public abstract class BonusConfig : ScriptableObject
{
    public BonusType type;
    public bool isPersistent;
    public float duration;
    public abstract void Apply();
    public abstract void Remove();
}