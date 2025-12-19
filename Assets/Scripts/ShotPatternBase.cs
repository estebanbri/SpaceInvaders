using UnityEngine;

public abstract class ShotPatternBase : ScriptableObject
{
    public abstract void Fire(
        WeaponAmmo ammoPrefab,
        Vector3 position,
        float ammoSpeed
    );
}
