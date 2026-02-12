using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementPatternController : MonoBehaviour
{
    [SerializeField] private List<MovementPatternDefinition> movementPatterns;
    [SerializeField] private float timeBetweenPatterns = 3f;

    public event Action<int> OnPatternChanged;

    private int currentIndex;
    private float elapsedTime;
    private float patternTimer;

    private Vector3 basePosition;
    private MovementPatternRuntime currentPattern;

    private bool initialized;
    private Transform player;

    private void Update()
    {
        if (!initialized || currentPattern == null)
            return;

        elapsedTime += Time.deltaTime;
        patternTimer += Time.deltaTime;

        Vector3 offset = currentPattern.Evaluate(elapsedTime);

        //  CLAVE: usamos basePosition dinámico
        transform.localPosition = basePosition + offset;

        if (patternTimer >= timeBetweenPatterns)
        {
            patternTimer = 0f;

            if (currentIndex < movementPatterns.Count - 1)
                ActivatePattern(currentIndex + 1);
        }
    }

    public void Initialize(Transform player, Vector3? sharedCenter = null)
{
    this.player = player;

    if (sharedCenter.HasValue)
        basePosition = sharedCenter.Value;
    else
        basePosition = transform.localPosition;

    initialized = true;
    currentIndex = 0;

    ActivatePattern(0);
}


    //  NUEVO: Permite actualizar el centro dinámicamente
    public void SetBasePosition(Vector3 newBase)
    {
        basePosition = newBase;
    }

    private void ActivatePattern(int index)
    {
        currentIndex = index;
        elapsedTime = 0f;
        patternTimer = 0f;

        currentPattern = movementPatterns[index].CreateRuntime();
        currentPattern.Init(basePosition, player.position);

        OnPatternChanged?.Invoke(index);
    }
}
