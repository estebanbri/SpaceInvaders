using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponDefinition weaponDefault;
    private WeaponInstance currentWeapon;

    private void Awake()
    {
      Equip(weaponDefault);
    }

    public void Equip(WeaponDefinition weaponConfig)
    {
        currentWeapon = new WeaponInstance(weaponConfig);
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
