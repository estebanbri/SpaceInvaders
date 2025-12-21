using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponDefinition weaponDefault;

    private FactionComponent factionComponent;
    private WeaponInstance currentWeapon;

    private void Awake()
    {
      factionComponent = GetComponentInParent<FactionComponent>();
      Equip(weaponDefault);
    }

    public void Equip(WeaponDefinition weaponDefinition)
    {
        WeaponDefinition temp = weaponDefinition ?? weaponDefault;
        currentWeapon = new WeaponInstance(temp, factionComponent.Faction);
    }

    public void Fire()
    {
        if (currentWeapon == null) return;
        // transform: va a capturar la Posicion y Rotacion del gameobject (Gameobject=WeaponController) y segun la rotacion que tenga va a disparar en una direccion u otra
        // Es decir rotacion z=180 el transform.up va a tener una direccion hacia abajo, porque es como si esta patas para arriba.
        currentWeapon.Fire(transform);
    }

    public void ApplyFireRateBonus(float multiplier)
    {
        currentWeapon?.SetFireRateMultiplier(multiplier);
    }
}
