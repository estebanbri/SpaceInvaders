using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shot Patterns/Triple Shot Fan")]
public class ShotPatternFanTriple : ShotPatternBase
{
    [SerializeField] public float spreadAngle = 15f;

    public override void Fire(WeaponAmmo ammoPrefab,Transform firePoint,float ammoSpeed,FactionType ownerFaction)
    {
        FireBullet(ammoPrefab, firePoint, 0f, ammoSpeed, ownerFaction);
        FireBullet(ammoPrefab, firePoint, -spreadAngle, ammoSpeed, ownerFaction);
        FireBullet(ammoPrefab, firePoint, spreadAngle, ammoSpeed, ownerFaction);
    }

    private void FireBullet(WeaponAmmo ammoPrefab, Transform firePoint, float angle, float speed, FactionType ownerFaction)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 direction = rotation * firePoint.up;

        WeaponAmmo ammo = Object.Instantiate(ammoPrefab,firePoint.position,Quaternion.identity);

        ammo.Init(direction.normalized, ownerFaction, speed);
    }
}