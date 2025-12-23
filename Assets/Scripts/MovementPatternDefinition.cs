using UnityEngine;

public enum MovementType
{
    Lineal,
    Senoidal,
    ZigZag,
    ApuntaInicial,
    Circular,
    FigureEight,
    NoMove,
    Horizontal,
    LinealUp
}

[CreateAssetMenu(menuName = "Movement Pattern")]
public class MovementPatternDefinition : ScriptableObject
{
    public MovementType type;

    public float speed = 3f;

    // Senoidal / ZigZag / Circular
    public float amplitude = 1f;
    public float frequency = 2f;

    // Factory: crea una instancia runtime
    public MovementPatternRuntime CreateRuntime()
    {
        return new MovementPatternRuntime(this);
    }
}