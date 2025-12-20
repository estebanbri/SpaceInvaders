using UnityEngine;

public class PickupDualShot : MonoBehaviour
{
    [SerializeField] private WeaponDefinition weapon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Nave nave))
        {
            nave.GetWeaponController().Equip(weapon);
            Destroy(gameObject);
        }
    }
}
