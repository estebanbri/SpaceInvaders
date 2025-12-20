using UnityEngine;

public class PickupEscudo : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Nave nave))
        {
            nave.GetEscudo().AddEscudo();
            Destroy(gameObject);
        }
    }
}
