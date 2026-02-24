using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void OnExplosionEnd()
    {
        Destroy(gameObject);
    }
}