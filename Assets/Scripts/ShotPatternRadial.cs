using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shot Patterns/Radial Shot")]
public class ShotPatternRadial : ShotPatternBase
{
    [Header("Radial Settings")]
    [SerializeField] private int baseBulletCount = 8;
    [SerializeField] private float arcAngle = 270f;
    [SerializeField] private float startAngleOffset = 0f;

    [Header("Rotation")]
    [SerializeField] private bool rotateOverTime = false;
    [SerializeField] private float rotationSpeed = 10f;

    private float currentRotation;
    private int bulletCount;

    public override void Fire(
        WeaponAmmo ammoPrefab,
        Transform firePoint,
        float ammoSpeed,
        FactionType ownerFaction)
    {
        if (bulletCount <= 0) return;

        float adjustedSpeed = ammoSpeed;

        float baseOffset = startAngleOffset;
        if (rotateOverTime)
        {
            currentRotation += rotationSpeed * Time.deltaTime; ;
            baseOffset += currentRotation;
        }


        float angleStep = bulletCount > 1 ? arcAngle / (bulletCount - 1) : 0;


        for (int i = 0; i < bulletCount; i++)
        {
            float angle = baseOffset + angleStep * i;
            FireBullet(ammoPrefab, firePoint.position, angle, adjustedSpeed, ownerFaction);
        }

        Debug.Log($"[Radial] Bullets: {bulletCount}, Arc: {arcAngle}, AmmoSpeed: {adjustedSpeed}");
    }

    private void FireBullet(
        WeaponAmmo ammoPrefab,
        Vector3 origin,
        float angle,
        float speed,
        FactionType ownerFaction)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 direction = rotation * Vector3.up;

        WeaponAmmo ammo = Object.Instantiate(ammoPrefab, origin, Quaternion.identity);
        ammo.Init(direction.normalized, ownerFaction, speed);
    }

    public override void ApplyTierScaling(
        int tier,
        float baseAmmoSpeed,
        float baseFireRate,
        int baseBulletCount,
        out float scaledAmmoSpeed,
        out float scaledFireRate,
        out int scaledBulletCount)
    {
        base.ApplyTierScaling(tier, baseAmmoSpeed, baseFireRate, baseBulletCount,
            out scaledAmmoSpeed, out scaledFireRate, out scaledBulletCount);
        bulletCount = scaledBulletCount;
    }

}
