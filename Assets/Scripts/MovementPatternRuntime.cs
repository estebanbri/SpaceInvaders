using UnityEngine;

public class MovementPatternRuntime
{
    private readonly MovementPatternDefinition data;

    private Vector3 direction;
    private float startAngle;

    public MovementPatternRuntime(MovementPatternDefinition data)
    {
        this.data = data;
    }

    public void Init(Vector3 startPos, Vector3 playerPos)
    {
        switch (data.type)
        {
            case MovementType.ApuntaInicial:
                direction = (playerPos - startPos).normalized;
                break;

            case MovementType.LinealUp:
                direction = Vector3.up;
                break;

            default:
                direction = Vector3.down;
                break;
        }

        // Opcional: ángulo inicial aleatorio para circular
        startAngle = Random.Range(0f, Mathf.PI * 2f);
    }

    /// <summary>
    /// Función matemática del patrón (posición relativa)
    /// </summary>
    public Vector3 Evaluate(float time)
    {
        switch (data.type)
        {
            case MovementType.Senoidal:
                return new Vector3(
                    Mathf.Sin(time * data.frequency) * data.amplitude,
                    -data.speed * time,
                    0f
                );

            case MovementType.ZigZag:
                float x = Mathf.PingPong(time * data.frequency, data.amplitude)
                          - data.amplitude / 2f;
                return new Vector3(x, -data.speed * time, 0f);

            case MovementType.Circular:
                float angle = startAngle + time * data.frequency;
                return new Vector3(
                    Mathf.Cos(angle) * data.amplitude,
                    Mathf.Sin(angle) * data.amplitude,
                    0f
                );

            case MovementType.FigureEight:
                float t = time * data.frequency;
                return new Vector3(
                    Mathf.Sin(t) * data.amplitude,
                    Mathf.Sin(2f * t) * data.amplitude,
                    0f
                );

            case MovementType.Horizontal:
                return new Vector3(
                    Mathf.Sin(time * data.frequency) * data.amplitude,
                    0f,
                    0f
                );

            case MovementType.Lineal:
            case MovementType.LinealUp:
            case MovementType.ApuntaInicial:
                return direction * data.speed * time;

            case MovementType.NoMove:
                return Vector3.zero;
        }

        return Vector3.zero;
    }
}