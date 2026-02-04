using UnityEngine;

public class Nube : MonoBehaviour
{
    [SerializeField] private float speed = 2;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }
}
