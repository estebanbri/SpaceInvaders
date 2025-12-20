using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/Upgrade Weapon")]
public class BonusUpgradeWeapon : BonusDefinition
{
    [SerializeField] private WeaponDefinition weaponDefinition;

    public override void Apply()
    {
        Nave.Instance.GetWeaponController().Equip(weaponDefinition);
    }

    public override void Remove()
    {
        Nave.Instance.GetWeaponController().Equip(null);
    }
}