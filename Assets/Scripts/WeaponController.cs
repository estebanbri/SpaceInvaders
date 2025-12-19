using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get; private set; }

    [SerializeField] private List<Weapon> weapons;
    private Dictionary<WeaponType, Weapon> weaponMap;
    private WeaponType currentWeapon;

    private void Awake()
    {
        Instance = this;
        weaponMap = new Dictionary<WeaponType, Weapon>();
        foreach (var weapon in weapons)
            weaponMap[weapon.type] = weapon;
    }

    public void SetWeapon(WeaponType type)
    {
        currentWeapon = type;
    }

    public void Fire()
    {
        if (!weaponMap.TryGetValue(currentWeapon, out var weapon)) return;

        weapon.Fire(transform);
    }
}
