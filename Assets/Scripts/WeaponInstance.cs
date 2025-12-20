using UnityEngine;

public class WeaponInstance
{
    private WeaponDefinition weaponDefinition;
    private float nextFireTime;
    private float fireRateMultiplier = 1f;

    public WeaponInstance(WeaponDefinition weaponDefinition)
    {
        this.weaponDefinition = weaponDefinition;
    }

    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + weaponDefinition.fireRate * fireRateMultiplier;

        weaponDefinition.shotPattern.Fire(weaponDefinition.ammoPrefab, firePoint.position, weaponDefinition.ammoSpeed);
    }

    public void SetFireRateMultiplier(float multiplier)
    {
        this.fireRateMultiplier = multiplier;
    }
}