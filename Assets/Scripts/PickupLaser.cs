using UnityEngine;

public class PickupLaser : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Nave nave))
        {
            BonusManager.Instance.Activate(BonusType.Laser);
            Destroy(gameObject);
        }
    }
}
