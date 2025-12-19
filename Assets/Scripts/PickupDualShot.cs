using UnityEngine;

public class PickupDualShot : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Nave nave))
        {
            BonusManager.Instance.Activate(BonusType.DualShot);
            Destroy(gameObject);
        }
    }
}
