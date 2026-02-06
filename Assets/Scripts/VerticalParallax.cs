using UnityEngine;

public class VerticalParallax : MonoBehaviour
{
    public float speed = 1f;

    private float height;
    private Transform bg1;
    private Transform bg2;

    void Start()
    {
        bg1 = transform.GetChild(0);
        bg2 = transform.GetChild(1);

        height = bg1.GetComponent<SpriteRenderer>().bounds.size.y;

        // Posición inicial SEGURA
        bg1.localPosition = Vector3.zero;
        bg2.localPosition = Vector3.up * height;
    }

    void Update()
    {
        Vector3 movement = Vector3.down * speed * Time.deltaTime;

        bg1.position += movement;
        bg2.position += movement;

        if (bg1.position.y <= -height)
        {
            bg1.position = new Vector3(
                bg1.position.x,
                bg2.position.y + height,
                bg1.position.z
            );
        }

        if (bg2.position.y <= -height)
        {
            bg2.position = new Vector3(
                bg2.position.x,
                bg1.position.y + height,
                bg2.position.z
            );
        }
    }

}