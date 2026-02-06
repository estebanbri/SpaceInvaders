using UnityEngine;

public class ParallaxFollower : MonoBehaviour
{
    public VerticalParallax laneParallax;

    void Update()
    {
        if (laneParallax == null) return;

        // Movemos la torreta exactamente con la velocidad del fondo
        transform.position += Vector3.down * laneParallax.speed * Time.deltaTime;
    }
}