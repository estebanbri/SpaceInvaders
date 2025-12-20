using UnityEngine;

public class WeaponInstance
{
    private WeaponDefinition config;
    private float nextFireTime;
    private float fireRateMultiplier = 1f;

    public WeaponInstance(WeaponDefinition config)
    {
        this.config = config;
    }

    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + config.fireRate * fireRateMultiplier;

        config.shotPattern.Fire(config.ammoPrefab, firePoint.position,  config.ammoSpeed);
    }

    public void SetFireRateMultiplier(float multiplier)
    {
        this.fireRateMultiplier = multiplier;
    }
}