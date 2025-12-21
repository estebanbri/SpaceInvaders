using UnityEngine;

public class FactionComponent : MonoBehaviour
{
    [SerializeField] private FactionType faction;
    public FactionType Faction => faction;
}

public enum FactionType
{
    Player,
    Enemy
}