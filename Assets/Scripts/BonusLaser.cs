using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/Laser")]
public class BonusLaser : BonusConfig
{
    public override void Enable()
    {
        Nave.Instance.GetWeaponController().SetWeapon(WeaponType.Laser);
    }

    public override void Disable()
    {
        Nave.Instance.GetWeaponController().SetWeapon(WeaponType.Bullet);
    }
}