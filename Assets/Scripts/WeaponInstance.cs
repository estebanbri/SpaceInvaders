using UnityEngine;

public class WeaponInstance
{
    private WeaponDefinition weaponDefinition;
    private float nextFireTime;
    private float fireRateMultiplier = 1f;
    private FactionType ownerFaction;
    private WeaponMuzzleFlash weaponMuzzleFlash;

    public WeaponInstance(WeaponDefinition weaponDefinition, FactionType ownerFaction, WeaponMuzzleFlash weaponMuzzleFlash)
    {
        this.weaponDefinition = weaponDefinition;
        this.ownerFaction = ownerFaction;
        this.weaponMuzzleFlash = weaponMuzzleFlash;
    }

    // El parametro Transform firePoint va a venir la data de la rotacion que tenga quien dispara, entonces luego en las 
    // estrategias de disparo al decir transform.up esto no quiere decir que va hacia arriba sino es que va a depender del parametro que reciba aqui
    // porque dentro de el viene la Rotacion, ejemplo con una rotacion z=180 (en criollo es como si lo rotaste y quedo patas para arriba y la cabeza abajo) entonces
    // al decirle transform.up eso va a ir en direccion hacia abajo.
    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + weaponDefinition.fireRate * fireRateMultiplier;

        if (weaponMuzzleFlash != null)
        {
            weaponMuzzleFlash.Play();
        }

        weaponDefinition.shotPattern.Fire(weaponDefinition.ammoPrefab, firePoint, weaponDefinition.ammoSpeed, ownerFaction);
    }

    // Nuevo método para disparar usando posición y rotación
    public void Fire(Quaternion rotation, Vector3 position)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + weaponDefinition.fireRate * fireRateMultiplier;

        // Creamos un "fake" transform para disparar usando la rotación deseada
        GameObject temp = new GameObject("TempFirePoint");
        temp.transform.position = position;
        temp.transform.rotation = rotation;

        if (weaponMuzzleFlash != null) {
            weaponMuzzleFlash.Play();
        }

        weaponDefinition.shotPattern.Fire(weaponDefinition.ammoPrefab, temp.transform, weaponDefinition.ammoSpeed, ownerFaction);

        GameObject.Destroy(temp); // destruimos el objeto temporal inmediatamente después
    }

    public void SetFireRateMultiplier(float multiplier)
    {
        this.fireRateMultiplier = multiplier;
    }
}