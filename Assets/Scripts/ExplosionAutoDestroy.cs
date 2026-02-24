using UnityEngine;

public class ExplosionAutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}