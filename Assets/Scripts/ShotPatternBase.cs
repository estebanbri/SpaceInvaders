using UnityEngine;

public abstract class ShotPatternBase : ScriptableObject
{
    public int lanesCount;

    /// <summary>
    /// Escala stats por tier.
    /// </summary>
    public virtual void ApplyTierScaling(
        int cycle,
        float baseAmmoSpeed,
        float baseFireRate,
        int baseBulletCount,
        out float scaledAmmoSpeed,
        out float scaledFireRate,
        out int scaledBulletCount)
    {
        // Velocidad de proyectil +10% por tier
        scaledAmmoSpeed = baseAmmoSpeed * (1f + cycle * 0.1f);

        // FireRate más rápido con tier
        scaledFireRate = Mathf.Max(0.05f, baseFireRate / (1f + cycle * 0.1f)); // más rápido por tier

        // Cantidad de balas
        scaledBulletCount = Mathf.Min(baseBulletCount + cycle, 12); // Limite opcional
        Debug.Log($"[ShotPattern] {name} | Cycle: {cycle} | AmmoSpeed: {scaledAmmoSpeed:F2} | FireRate: {scaledFireRate:F2} | BulletCount: {scaledBulletCount}");
    }

    public abstract void Fire(
        WeaponAmmo ammoPrefab,
        Transform firePoint,
        float ammoSpeed,
        FactionType ownerFaction
    );

}
