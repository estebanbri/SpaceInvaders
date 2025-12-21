using UnityEngine;

public class WeaponInstance
{
    private WeaponDefinition weaponDefinition;
    private float nextFireTime;
    private float fireRateMultiplier = 1f;
    private FactionType ownerFaction;

    public WeaponInstance(WeaponDefinition weaponDefinition, FactionType ownerFaction)
    {
        this.weaponDefinition = weaponDefinition;
        this.ownerFaction = ownerFaction;
    }

    // El parametro Transform firePoint va a venir la data de la rotacion que tenga quien dispara, entonces luego en las 
    // estrategias de disparo al decir transform.up esto no quiere decir que va hacia arriba sino es que va a depender del parametro que reciba aqui
    // porque dentro de el viene la Rotacion, ejemplo con una rotacion z=180 (en criollo es como si lo rotaste y quedo patas para arriba y la cabeza abajo) entonces
    // al decirle transform.up eso va a ir en direccion hacia abajo.
    public void Fire(Transform firePoint)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + weaponDefinition.fireRate * fireRateMultiplier;

        weaponDefinition.shotPattern.Fire(weaponDefinition.ammoPrefab, firePoint, weaponDefinition.ammoSpeed, ownerFaction);
    }

    public void SetFireRateMultiplier(float multiplier)
    {
        this.fireRateMultiplier = multiplier;
    }
}