using UnityEngine;

public class WeaponInstance
{
    private WeaponDefinition weaponDef;
    private ShotPatternBase shotPattern;
    private FactionType faction;
    private WeaponMuzzleFlash muzzleFlash;

    public float AmmoSpeed { get; private set; }
    public float FireRate { get; private set; }
    public int BulletCount { get; private set; }

    private float nextFireTime;

    // Parámetro opcional: cuánto variar el FireRate en porcentaje (0.2 = ±20%)
    private const float fireRateVariation = 0.2f;

    public WeaponInstance(WeaponDefinition def, int cycle, FactionType faction, WeaponMuzzleFlash muzzleFlashPrefab, bool applyTierScaling = true) 
    {
        weaponDef = def;
        shotPattern = def.shotPattern;
        this.faction = faction;
        muzzleFlash = muzzleFlashPrefab;
        if (applyTierScaling && shotPattern != null)
        {
            int baseBullets = Mathf.Max(1, Mathf.RoundToInt(def.bulletCount));
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

        // Primer disparo aleatorio para que no todos disparen sincronizados
        nextFireTime = Time.time + Random.Range(0f, FireRate);
    }

    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;

        // 🔹 Aplicar random al FireRate actual
        float randomizedFireRate = FireRate * Random.Range(1f - fireRateVariation, 1f + fireRateVariation);

        // 🔹 Log para depuración
        Debug.Log($"[WeaponInstance] Disparo en {Time.time:F2}s | FireRate base: {FireRate:F2} | FireRate random: {randomizedFireRate:F2}");

        nextFireTime = Time.time + randomizedFireRate;

        shotPattern.Fire(weaponDef.ammoPrefab, firePoint, AmmoSpeed, faction);
        // 🔥 Muzzle exacto en el momento real del disparo
        muzzleFlash?.Play();

    }

    public void Fire(Quaternion rotation, Vector3 position)
    {
        if (Time.time < nextFireTime) return;

        float randomizedFireRate = FireRate * Random.Range(1f - fireRateVariation, 1f + fireRateVariation);
        // 🔹 Log para depuración
        Debug.Log($"[WeaponInstance] Disparo en {Time.time:F2}s | FireRate base: {FireRate:F2} | FireRate random: {randomizedFireRate:F2}");
        nextFireTime = Time.time + randomizedFireRate;

        GameObject temp = new GameObject("TempFirePoint");
        temp.transform.position = position;
        temp.transform.rotation = rotation;

        shotPattern.Fire(weaponDef.ammoPrefab, temp.transform, AmmoSpeed, faction);
        // 🔥 Muzzle exacto en el momento real del disparo
        muzzleFlash?.Play();
        GameObject.Destroy(temp);
    }

    public void FireImmediate(Transform firePoint)
    {
        // 🔹 Ignora nextFireTime
        shotPattern.Fire(weaponDef.ammoPrefab, firePoint, AmmoSpeed, faction);
        // 🔥 Muzzle exacto en el momento real del disparo
        muzzleFlash?.Play();
    }
    public void FireImmediate(Quaternion rotation, Vector3 position)
    {
        GameObject temp = new GameObject("TempFirePoint");
        temp.transform.position = position;
        temp.transform.rotation = rotation;

        shotPattern.Fire(weaponDef.ammoPrefab, temp.transform, AmmoSpeed, faction);
        // 🔥 Muzzle exacto en el momento real del disparo
        muzzleFlash?.Play();
        GameObject.Destroy(temp);
    }


}
