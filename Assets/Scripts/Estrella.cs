using UnityEngine;

public class Estrella : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private float destroyY = -6f;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
