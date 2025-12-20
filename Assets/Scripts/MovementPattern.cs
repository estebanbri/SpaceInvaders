using UnityEngine;

public enum MovementType
{
    Lineal,
    Senoidal,
    ZigZag,
    ApuntaInicial
}

[System.Serializable]
public class MovementPattern
{
    public MovementType type;

    public float speed = 3f;

    // Senoidal / ZigZag
    public float amplitude = 1f;
    public float frequency = 2f;

    private Vector3 direction;
    private Vector3 startPos;

    public void Init(Vector3 enemyPos, Vector3 playerPos)
    {
        startPos = enemyPos;

        if (type == MovementType.ApuntaInicial)
        {
            // Calcular la dirección desde la posición del enemigo hacia la posición del jugador
            direction = (playerPos - enemyPos).normalized;
        }
        else
        {
            direction = Vector3.down;
        }
    }

    public Vector3 Evaluate(float time)
    {
        switch (type)
        {
            case MovementType.Senoidal:
                return new Vector3(
                    Mathf.Sin(time * frequency) * amplitude, -speed * time, 0);

            case MovementType.ZigZag:
                float x = Mathf.PingPong(time * frequency, amplitude) - amplitude / 2f;
                return new Vector3(x, -speed * time, 0);

            case MovementType.ApuntaInicial:
            case MovementType.Lineal:
                return direction * speed * time;
        }

        return Vector3.zero;
    }
}