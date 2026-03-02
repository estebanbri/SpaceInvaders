using UnityEngine;

public class DestroyOutsideCamera : MonoBehaviour
{
    private Camera mainCamera;
    private float halfWidth;
    private float halfHeight;
   
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;
        halfWidth = sr.bounds.extents.x;
        halfHeight = sr.bounds.extents.y;
    }

    private void Update()
    {
        if (sr == null) return;
        CheckIfOutOfBounds();
    }

    private void CheckIfOutOfBounds()
    {
        Camera cam = Camera.main;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        Vector3 pos = transform.position;

        //  Caso superior
        if (pos.y - halfHeight > max.y)
        {
            Destroy(gameObject);
            return;
        }

        //  Caso inferior
        if (pos.y + halfHeight < min.y)
        {
            Destroy(gameObject);
            return;
        }

        //  Caso izquierda (completamente fuera)
        if (pos.x + halfWidth < min.x)
        {
            Destroy(gameObject);
            return;
        }

        //  Caso derecha (completamente fuera)
        if (pos.x - halfWidth > max.x)
        {
            Destroy(gameObject);
            return;
        }
    }

}
