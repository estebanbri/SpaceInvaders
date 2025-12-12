using UnityEngine;

public class Enemigo : MonoBehaviour, IDamageable
{

    public void TakeDamage()
    {
        // sonido, animación, partículas, etc.
        Destroy(gameObject);
    }
}
