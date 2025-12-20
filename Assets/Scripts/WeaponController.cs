using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<WeaponDefinition> availableWeapons;

    private Dictionary<WeaponDefinition, WeaponInstance> weaponMap;
    private WeaponInstance currentWeapon;

    private void Awake()
    {
        weaponMap = new Dictionary<WeaponDefinition, WeaponInstance>();

        foreach (var weapon in availableWeapons)
        {
            weaponMap[weapon] = new WeaponInstance(weapon);
        }

        if (availableWeapons.Count > 0)
            Equip(availableWeapons[0]);
    }

    public void Equip(WeaponDefinition weaponConfig)
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
