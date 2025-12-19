using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    public float fireRate;          // tiempo entre disparos
    public float ammoSpeed;
    public WeaponAmmo ammoPrefab;
    public ShotPatternBase shotPattern;
}

public enum ShotType
{
    Single,
    Dual,
    Triple,
    Spread
}