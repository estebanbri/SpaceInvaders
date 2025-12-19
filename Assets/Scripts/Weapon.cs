using UnityEngine;

[System.Serializable]
public class Weapon
{
    public WeaponType type;
    // Tiempo entre disparos en segundos
    public float ammoRate;
    public float ammoSpeed;
    public WeaponAmmo weaponAmmoPrefab;
    private float nextFireTime;

    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + ammoRate;
        WeaponAmmo ammo = Object.Instantiate(weaponAmmoPrefab, firePoint.position, Quaternion.identity);
        ammo.SetAmmoSpeed(ammoSpeed);
    }
}
public enum WeaponType
{
    Bullet,
    Laser
}