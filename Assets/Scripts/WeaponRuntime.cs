using System;
using UnityEngine;

public class WeaponRuntime
{
    private WeaponConfig config;
    private float nextFireTime;
    private float fireRateMultiplier = 1f;

    public WeaponRuntime(WeaponConfig config)
    {
        this.config = config;
    }

    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + config.fireRate * fireRateMultiplier;

        config.shotPattern.Fire(
        config.ammoPrefab,
        firePoint.position,
        config.ammoSpeed
        );
    }

    public void SetFireRateMultiplier(float multiplier)
    {
        this.fireRateMultiplier = multiplier;
    }
}