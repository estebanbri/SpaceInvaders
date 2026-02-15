using UnityEngine;

public class EntryAnimation : MonoBehaviour
{
    public bool HasFinished { get;  set; } = false;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float duration;

    private float elapsed = 0f;

    // Parámetros para la animación orgánica
    private float arcHeight;
    private float lateralAmplitude;
    private float lateralSpeed;
    private float phaseOffset;

    // Offset inicial para subgrupo
    private Vector3 subGroupOffset;

    public void Initialize(Vector3 start, Vector3 target, float durationSeconds)
    {
        startPos = start;
        targetPos = target;
        duration = durationSeconds;

        // Valores aleatorios para sensación orgánica
        arcHeight = Random.Range(0.8f, 1.5f);
        lateralAmplitude = Random.Range(0.2f, 0.5f);
        lateralSpeed = Random.Range(3f, 6f);
        phaseOffset = Random.Range(0f, Mathf.PI * 2f);

        // Offset del subgrupo para que los enemigos no entren exactamente en línea
        subGroupOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.3f, 0.3f), 0f);

        HasFinished = false;
    }

    void Update()
    {
        if (HasFinished) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Convertimos a posición local relativa al padre
        Vector3 localStart = transform.parent.InverseTransformPoint(startPos);
        Vector3 localTarget = transform.parent.InverseTransformPoint(targetPos);

        // Movimiento principal interpolado
        Vector3 pos = Vector3.Lerp(localStart, localTarget, t);

        // Determinar eje perpendicular al movimiento principal
        Vector3 moveDir = (localTarget - localStart).normalized;
        Vector3 perpendicular = Vector3.Cross(moveDir, Vector3.forward); // perpendicular en XY

        // Aplicar arco en el eje perpendicular
        pos += perpendicular * Mathf.Sin(t * Mathf.PI) * arcHeight * 3f;

        // Oscilación lateral orgánica (sutil)
        pos += perpendicular * Mathf.Sin(Time.time * lateralSpeed + phaseOffset) * lateralAmplitude * (1f - t);

        // Offset de subgrupo que se disuelve al llegar
        pos += subGroupOffset * (1f - t);

        transform.localPosition = pos;

        if (t >= 1f) HasFinished = true;
    }
}
