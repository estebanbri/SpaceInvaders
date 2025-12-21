using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shot Patterns/Triple Shot")]
public class ShotPatternTriple : ShotPatternBase
{
    [SerializeField] private float offsetX = 0.3f;

    public override void Fire(WeaponAmmo ammoPrefab, Transform firePoint, float ammoSpeed, FactionType ownerFaction)
    {
        Vector3 left = firePoint.position + Vector3.left * offsetX;
        Vector3 right = firePoint.position + Vector3.right * offsetX;

        WeaponAmmo ammo1 = Object.Instantiate(ammoPrefab, firePoint.position, Quaternion.identity);
        WeaponAmmo ammo2 = Object.Instantiate(ammoPrefab, left, Quaternion.identity);
        WeaponAmmo ammo3 = Object.Instantiate(ammoPrefab, right, Quaternion.identity);

        ammo1.Init(firePoint.up, ownerFaction, ammoSpeed);
        ammo2.Init(firePoint.up, ownerFaction, ammoSpeed);
        ammo3.Init(firePoint.up, ownerFaction, ammoSpeed);
    }
}