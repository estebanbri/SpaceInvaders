using UnityEngine;

public abstract class ShotPatternBase : ScriptableObject
{
    public int lanesCount;

    public abstract void Fire(
        WeaponAmmo ammoPrefab,
        Transform firePoint,
        float ammoSpeed,
        FactionType ownerFaction
    );
}
