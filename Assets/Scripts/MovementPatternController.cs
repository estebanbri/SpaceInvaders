using System;
using System.Collections.Generic;
using UnityEngine;

// FIJA POSICION ABSOLUTA
public class MovementPatternController : MonoBehaviour
{
    [SerializeField] private List<MovementPatternDefinition> movementPatterns;
    [SerializeField] private float timeBetweenPatterns = 3f;
    public event Action<int> OnPatternChanged;

    private int currentIndex;
    private float elapsedTime;
    private float patternTimer;

    private Vector3 startPosition;
    // FIJA LA POSICION RELATIVA
    private MovementPatternRuntime currentPattern;

    private void Start()
    {
        ActivatePattern(0);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        patternTimer += Time.deltaTime;

        // POSICION ACTUAL + POSICION RELATIVA
        transform.position = startPosition + currentPattern.Evaluate(elapsedTime);

        if (patternTimer >= timeBetweenPatterns)
        {
            patternTimer = 0f;

            if (currentIndex < movementPatterns.Count - 1)
                ActivatePattern(currentIndex + 1);
        }
    }

    private void ActivatePattern(int index)
    {
        currentIndex = index;
        elapsedTime = 0f;

        // Guardamos posición inicial del patrón
        startPosition = transform.position;

        currentPattern = movementPatterns[index].CreateRuntime();
        currentPattern.Init(startPosition, Nave.Instance.transform.position);
        OnPatternChanged?.Invoke(index);
    }

}