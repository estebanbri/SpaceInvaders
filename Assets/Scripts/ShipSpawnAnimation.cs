using UnityEngine;
using System.Collections;

public class ShipSpawnAnimation : MonoBehaviour
{
    [Header("Spawn settings")]
    public float travelDistance = 5f;
    public float travelDuration = 1.2f;
    public AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 targetPosition;
    private Nave playerController;

    void Start()
    {
        targetPosition = transform.position;

        // Aparece desde la izquierda
        Vector3 spawnOffset = Vector3.left * travelDistance;
        transform.position = targetPosition + spawnOffset;

        playerController = GetComponent<Nave>();
        if (playerController != null)
            playerController.enabled = false;

        StartCoroutine(AnimateSpawn());
    }

    IEnumerator AnimateSpawn()
    {
        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime / travelDuration;
            float easedT = easing.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, targetPosition, easedT);
            yield return null;
        }

        transform.position = targetPosition;

        if (playerController != null)
            playerController.enabled = true;
    }
}