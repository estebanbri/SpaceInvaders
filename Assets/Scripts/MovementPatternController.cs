using System.Collections.Generic;
using UnityEngine;

public class MovementPatternController : MonoBehaviour
{
    [SerializeField] private List<MovementPatternDefinition> movementPatterns;
    [SerializeField] private float timeBetweenPatterns = 3f;

    private int currentIndex;
    private float elapsedTime;
    private float patternTimer;

    private MovementPatternRuntime currentPattern;

    private void Start()
    {
        ActivatePattern(0);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        patternTimer += Time.deltaTime;

        Vector3 delta = currentPattern.EvaluateDelta(elapsedTime);
        transform.position += delta;

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

        currentPattern = movementPatterns[index].CreateRuntime();
        currentPattern.Init(transform.position, Nave.Instance.transform.position);
    }
}