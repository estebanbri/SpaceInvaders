using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/DualAmmo")]
public class BonusDualShot : BonusConfig
{
    [SerializeField] private WeaponDefinition weaponAfterBonus;
    [SerializeField] private WeaponDefinition weaponBonus;

    public override void Apply()
    {
        Nave.Instance.GetWeaponController().Equip(weaponBonus);
    }

    public override void Remove()
    {
        Nave.Instance.GetWeaponController().Equip(weaponAfterBonus);
    }
}