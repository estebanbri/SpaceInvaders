using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/DualAmmo")]
public class BonusDualShot : BonusConfig
{
    [SerializeField] private WeaponConfig weaponAfterBonus;
    [SerializeField] private WeaponConfig weaponBonus;

    public override void Apply()
    {
        Nave.Instance.GetWeaponController().Equip(weaponBonus);
    }

    public override void Remove()
    {
        Nave.Instance.GetWeaponController().Equip(weaponAfterBonus);
    }
}