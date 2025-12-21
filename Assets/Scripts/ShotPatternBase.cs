using UnityEngine;

public abstract class ShotPatternBase : ScriptableObject
{
    public abstract void Fire(
        WeaponAmmo ammoPrefab,
        Transform firePoint,
        float ammoSpeed,
        FactionType ownerFaction
    );
}
