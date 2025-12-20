using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shot Patterns/Single Shot")]
public class ShotPatternSingle : ShotPatternBase
{

    public override void Fire(WeaponAmmo ammoPrefab, Vector3 position, float ammoSpeed)
    {
        WeaponAmmo ammo = Object.Instantiate(ammoPrefab, position, Quaternion.identity);
        ammo.SetAmmoSpeed(ammoSpeed);
    }
}