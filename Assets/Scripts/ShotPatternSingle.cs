using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shot Patterns/Single Shot")]
public class ShotPatternSingle : ShotPatternBase
{

    public override void Fire(WeaponAmmo ammoPrefab, Transform firePoint, float ammoSpeed, FactionType ownerFaction)
    {
        WeaponAmmo ammo = Object.Instantiate(ammoPrefab, firePoint.position, Quaternion.identity);
        ammo.Init(firePoint.up, ownerFaction, ammoSpeed);
    }
}