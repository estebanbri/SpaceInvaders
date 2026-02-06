using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponDefinition weaponDefault;
    [SerializeField] private WeaponMuzzleFlash muzzleFlash;

    [SerializeField] private bool autoAimAtPlayer = false;

    private FactionComponent factionComponent;
    private WeaponInstance currentWeapon;

    private Transform playerTransform;

    private Nave playerNave;

    private void Awake()
    {
      factionComponent = GetComponentInParent<FactionComponent>();
      Equip(weaponDefault);
      playerNave = FindFirstObjectByType<Nave>();
    }

    public void Equip(WeaponDefinition weaponDefinition)
    {
        WeaponDefinition temp = weaponDefinition ?? weaponDefault;
        currentWeapon = new WeaponInstance(temp, factionComponent.Faction, muzzleFlash);
    }


    public void Fire()
    {
    if (currentWeapon == null) return;

    if (autoAimAtPlayer)
    {
        if (playerNave == null)
        {
            // Si no hay player, dispara en la dirección actual del arma
            // transform: va a capturar la Posicion y Rotacion del gameobject (Gameobject=WeaponController) y segun la rotacion que tenga va a disparar en una direccion u otra
             // Es decir rotacion z=180 el transform.up va a tener una direccion hacia abajo, porque es como si esta patas para arriba.
            currentWeapon.Fire(transform);
            return;
        }

        Vector3 directionToPlayer = (playerNave.transform.position - transform.position).normalized;
        Quaternion rotationToPlayer = Quaternion.LookRotation(Vector3.forward, directionToPlayer);
        currentWeapon.Fire(rotationToPlayer, transform.position);
    }
    else
    {
        currentWeapon.Fire(transform);
    }
}

    public void ApplyFireRateBonus(float multiplier)
    {
        currentWeapon?.SetFireRateMultiplier(multiplier);
    }
}
