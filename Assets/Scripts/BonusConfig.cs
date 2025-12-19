using UnityEngine;

public abstract class BonusConfig : ScriptableObject
{
    public BonusType type;
    public GameObject prefab;
    public bool isPersistent;
    public float duration;
    public abstract void Enable();
    public abstract void Disable();
}