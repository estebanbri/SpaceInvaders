using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponDefinition weaponDefault;
    [SerializeField] private WeaponMuzzleFlash muzzleFlash;
    [SerializeField] private bool autoAimAtPlayer = false;

    private FactionComponent factionComponent;
    private WeaponInstance currentWeapon;
    private Nave playerNave;

    private void Awake()
    {
        factionComponent = GetComponentInParent<FactionComponent>();
        playerNave = FindFirstObjectByType<Nave>();
        Equip(weaponDefault, 0);
    }

    public void Equip(WeaponDefinition weaponDefinition, int tier = 0)
    {
        WeaponDefinition temp = weaponDefinition ?? weaponDefault;

        if (temp == null)
        {
            Debug.LogWarning("[WeaponController] No weapon to equip!");
            return;
        }

        // Aplica tier solo si hay ShotPattern
        if (tier > 0 && temp.shotPattern != null)
        {
            currentWeapon = new WeaponInstance(temp, tier, factionComponent?.Faction ?? FactionType.Player);
        }
        else
        {
            currentWeapon = new WeaponInstance(temp, 0, factionComponent?.Faction ?? FactionType.Player, applyTierScaling: false);
        }
    }

    public void Fire()
    {
        if (currentWeapon == null) return;

        if (autoAimAtPlayer && playerNave != null)
        {
            Vector3 dir = (playerNave.transform.position - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);
            currentWeapon.Fire(rot, transform.position);
        }
        else
        {
            currentWeapon.Fire(transform);
        }
    }

    // Nuevo: llamado desde Enemigo para respetar FireRate
    public void TryFire()
    {
        Fire();
    }

    public WeaponDefinition GetWeaponDefault() => weaponDefault;
}
