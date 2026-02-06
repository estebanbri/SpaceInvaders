using UnityEngine;

public class ParallaxFollower : MonoBehaviour
{
    public VerticalParallax parallax; // asignar el script que mueve los fondos
    private Transform bg1;
    private Transform bg2;
    private float height;

    private Transform chosenBG;
    private Vector3 lastPos;

    void Start()
    {
        if (parallax == null) return;

        bg1 = parallax.transform.GetChild(0);
        bg2 = parallax.transform.GetChild(1);
        height = bg1.GetComponent<SpriteRenderer>().bounds.size.y;

        // Elegimos aleatoriamente cuál bg seguir
        chosenBG = (Random.value > 0.5f) ? bg1 : bg2;
        lastPos = chosenBG.position;
    }

    void Update()
    {
        Vector3 delta = chosenBG.position - lastPos;
        transform.position += delta;
        lastPos = chosenBG.position;
    }
}
