using UnityEngine;

// Inmutable data Global Shared
[CreateAssetMenu(menuName = "Weapons/Weapon Definition")]
public class WeaponDefinition : ScriptableObject
{
    public float fireRate;          // tiempo entre disparos
    public float ammoSpeed;
    public WeaponAmmo ammoPrefab;
    public ShotPatternBase shotPattern;
    public float bulletCount = 1;
}