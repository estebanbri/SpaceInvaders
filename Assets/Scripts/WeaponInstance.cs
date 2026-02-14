using UnityEngine;

public class WeaponInstance
{
    private WeaponDefinition weaponDef;
    private ShotPatternBase shotPattern;
    private FactionType faction;

    public float AmmoSpeed { get; private set; }
    public float FireRate { get; private set; }
    public int BulletCount { get; private set; }

    private float nextFireTime;

    public WeaponInstance(WeaponDefinition def, int cycle, FactionType faction, bool applyTierScaling = true)
    {
        weaponDef = def;
        shotPattern = def.shotPattern;
        this.faction = faction;

        if (applyTierScaling && shotPattern != null)
        {
            int baseBullets = Mathf.Max(1, Mathf.RoundToInt(def.bulletCount)); // tier 0 como base
            shotPattern.ApplyTierScaling(cycle, def.ammoSpeed, def.fireRate, baseBullets,
                out float scaledAmmoSpeed, out float scaledFireRate, out int scaledBulletCount);

            AmmoSpeed = scaledAmmoSpeed;
            FireRate = scaledFireRate;
            BulletCount = scaledBulletCount;
            Debug.Log($"[WeaponInstance] {def.name} | Cycle: {cycle} | AmmoSpeed: {AmmoSpeed:F2} | FireRate: {FireRate:F2} | Bullets: {BulletCount}");
        }
        else
        {
            AmmoSpeed = def.ammoSpeed;
            FireRate = def.fireRate;
            BulletCount = Mathf.Max(1, Mathf.RoundToInt(def.bulletCount));
        }
    }

    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + FireRate;
        shotPattern.Fire(weaponDef.ammoPrefab, firePoint, AmmoSpeed, faction);
    }

    public void Fire(Quaternion rotation, Vector3 position)
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + FireRate;

        GameObject temp = new GameObject("TempFirePoint");
        temp.transform.position = position;
        temp.transform.rotation = rotation;

        shotPattern.Fire(weaponDef.ammoPrefab, temp.transform, AmmoSpeed, faction);
        GameObject.Destroy(temp);
    }
}
