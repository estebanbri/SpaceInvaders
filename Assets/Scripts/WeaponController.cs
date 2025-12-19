using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<WeaponConfig> availableWeapons;

    private Dictionary<WeaponConfig, WeaponRuntime> weaponMap;
    private WeaponRuntime currentWeapon;

    private void Awake()
    {
        weaponMap = new Dictionary<WeaponConfig, WeaponRuntime>();

        foreach (var weapon in availableWeapons)
        {
            weaponMap[weapon] = new WeaponRuntime(weapon);
        }

        if (availableWeapons.Count > 0)
            Equip(availableWeapons[0]);
    }

    public void Equip(WeaponConfig weaponConfig)
    {
        if (!weaponMap.ContainsKey(weaponConfig)) return;
        currentWeapon = weaponMap[weaponConfig];
    }

    public void Fire()
    {
        if (currentWeapon == null) return;
        currentWeapon.Fire(transform);
    }

    public void ApplyFireRateBonus(float multiplier)
    {
        currentWeapon?.SetFireRateMultiplier(multiplier);
    }
}
