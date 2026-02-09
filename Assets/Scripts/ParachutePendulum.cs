using UnityEngine;

public class ParachutePendulum : MonoBehaviour
{
    [Header("Pendulum")]
    public float maxAngle = 10f;
    public float swingSpeed = 1.5f;

    [Header("Wind")]
    public float windStrength = 1f;
    public float windSpeed = 0.5f;

    private float timeOffset;

    void Start()
    {
        timeOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float wind =
            Mathf.Sin(Time.time * windSpeed + timeOffset) * windStrength;

        float angle =
            Mathf.Sin(Time.time * swingSpeed + wind) * maxAngle;

        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}