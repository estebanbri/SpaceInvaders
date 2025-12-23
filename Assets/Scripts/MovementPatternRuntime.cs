using UnityEngine;

public class MovementPatternRuntime
{
    private readonly MovementPatternDefinition data;

    private Vector3 direction;
    private float startAngle;
    private float lastTime;
    private int horizontalDir = 1;
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
    }

    /// <summary>
    /// Devuelve el desplazamiento incremental desde el último frame
    /// </summary>
    public Vector3 EvaluateDelta(float time)
    {
        Vector3 current = Evaluate(time);
        Vector3 previous = Evaluate(lastTime);

        lastTime = time;

        return current - previous;
    }

    /// <summary>
    /// Función matemática del patrón (posición relativa)
    /// </summary>
    private Vector3 Evaluate(float time)
    {
        switch (data.type)
        {
            case MovementType.Senoidal:
                return new Vector3(
                    Mathf.Sin(time * data.frequency) * data.amplitude,
                    -data.speed * time,
                    0
                );

            case MovementType.ZigZag:
                float x = Mathf.PingPong(time * data.frequency, data.amplitude)
                          - data.amplitude / 2f;
                return new Vector3(x, -data.speed * time, 0);

            case MovementType.Circular:
                float angle = startAngle + time * data.frequency;
                return new Vector3(
                    Mathf.Cos(angle) * data.amplitude,
                    Mathf.Sin(angle) * data.amplitude,
                    0
                );
            case MovementType.FigureEight:  
                    float t = time * data.frequency;
                    float x1 = Mathf.Sin(t);
                    float y = Mathf.Sin(2f * t);

                    return new Vector3(
                        x1 * data.amplitude,
                        y * data.amplitude,
                        0
                    );

            case MovementType.Horizontal:
                float x2 = Mathf.Sin(time * data.frequency) * data.amplitude;
                return new Vector3(x2, 0f, 0f);

            case MovementType.NoMove:
                return Vector3.zero;

            case MovementType.Lineal:
            case MovementType.LinealUp:
            case MovementType.ApuntaInicial:
                return direction * data.speed * time;


        }

        return Vector3.zero;
    }
}