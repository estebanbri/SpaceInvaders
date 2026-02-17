using UnityEngine;

public class AsteroideTurbulence : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.2f;
    [SerializeField] private float frequency = 2.5f;

    private Vector3 camStartPos;
    private bool active = false;

    void Start()
    {
        camStartPos = transform.position;
    }

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
        transform.position = camStartPos;
    }

    void Update()
    {
        if (!active) return;

        float x = Mathf.PerlinNoise(Time.time * frequency, 0f) - 0.5f;
        float y = Mathf.PerlinNoise(0f, Time.time * frequency) - 0.5f;

        transform.position = camStartPos + new Vector3(x, y, 0f) * amplitude;
    }
}

