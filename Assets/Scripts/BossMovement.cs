using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Entrance")]
    [SerializeField] private float enterOffset = 1f;
    [SerializeField] private float enterSpeed = 3f;

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private Vector3 targetPosition;
    private bool entering = true;

    private float leftLimit;
    private float rightLimit;
    private float halfWidth;

    private int direction = 1;
    private Enemigo enemigo;

    void Start()
    {

        enemigo = GetComponent<Enemigo>();
        Camera cam = Camera.main;

        // Obtener borde superior de cámara
        Vector3 top = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, cam.nearClipPlane));

        // Calcular tamaño real del sprite
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        float halfHeight = sr.bounds.extents.y;
        halfWidth = sr.bounds.extents.x;

        // Posición objetivo final (un poco debajo del borde superior)
        targetPosition = new Vector3(
            0,
            top.y - enterOffset - halfHeight,
            0
        );

        // Empieza fuera de pantalla
        transform.position = new Vector3(
            0,
            top.y + halfHeight,
            0
        );

        CalculateHorizontalLimits();
    }

    void Update()
    {
        if (entering)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                enterSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                entering = false;

            return;
        }


        enemigo.SetState(EnemyState.Idle);
        MoveHorizontal();
    }

    void CalculateHorizontalLimits()
    {
        Camera cam = Camera.main;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        leftLimit = min.x + halfWidth;
        rightLimit = max.x - halfWidth;
    }

    void MoveHorizontal()
    {
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        if (transform.position.x >= rightLimit)
        {
            transform.position = new Vector3(rightLimit, transform.position.y, transform.position.z);
            direction = -1;
        }
        else if (transform.position.x <= leftLimit)
        {
            transform.position = new Vector3(leftLimit, transform.position.y, transform.position.z);
            direction = 1;
        }
    }
}
