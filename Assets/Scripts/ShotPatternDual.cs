using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shot Patterns/Dual Shot")]
public class DualShotPattern : ShotPatternBase
{
    [SerializeField] private float offsetX = 0.3f;

    public override void Fire(WeaponAmmo ammoPrefab, Vector3 position, float ammoSpeed)
    {
        Vector3 left = position + Vector3.left * offsetX;
        Vector3 right = position + Vector3.right * offsetX;

        WeaponAmmo a1 = Object.Instantiate(ammoPrefab, left, Quaternion.identity);
        WeaponAmmo a2 = Object.Instantiate(ammoPrefab, right, Quaternion.identity);

        a1.SetAmmoSpeed(ammoSpeed);
        a2.SetAmmoSpeed(ammoSpeed);
    }
}