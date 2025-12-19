using UnityEngine;

public class PïckupVida : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Nave nave))
        {
            GameManager.Instance.AddVida();
            Destroy(gameObject);
        }
    }
}
