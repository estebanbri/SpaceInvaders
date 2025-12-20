using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private float stopY = 5f;
    [SerializeField] private float limitX = 5f;
    [SerializeField] private float entrySpeed = 3f;
    [SerializeField] private float horizontalSpeed = 2f;

    private bool arrived;
    private int horizontalDir = 1; // 1 = derecha, -1 = izquierda
    private Enemigo enemigo;

    private void Awake()
    {
        enemigo = GetComponent<Enemigo>();
    }

    void Update()
    {
        if (!arrived)
        {
            transform.position += Vector3.down * entrySpeed * Time.deltaTime;

            if (transform.position.y <= stopY)
            {
                arrived = true;
                enemigo.StopMovement();
                enemigo.EnableCombat();
            }

            return;
        }

        transform.position += Vector3.right * horizontalDir * horizontalSpeed * Time.deltaTime;

        if (Mathf.Abs(transform.position.x) >= limitX)
        {
            // Invertimos la direccion si era derecha entonces: 1 * -1 = nueva direccion izq pero si ya era izquierda entonces -1 * -1 = nueva direccion derecha
            horizontalDir = horizontalDir * - 1;  
        }
    }
}