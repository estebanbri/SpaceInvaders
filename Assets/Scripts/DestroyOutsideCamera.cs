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

        //  Caso inferior (ya lo tenías)
        if (pos.y + halfHeight < min.y)
        {
            OnEnemyKilled(gameObject);
            Destroy(gameObject);
            return;
        }

        //  Caso izquierda (completamente fuera)
        if (pos.x + halfWidth < min.x)
        {
            OnEnemyKilled(gameObject);
            Destroy(gameObject);
            return;
        }

        //  Caso derecha (completamente fuera)
        if (pos.x - halfWidth > max.x)
        {
            OnEnemyKilled(gameObject);
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnemyKilled(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out FactionComponent factionComponent))
        {
            if (factionComponent.Faction == FactionType.Enemy)
            {
                Debug.Log("Enemigo: " + gameObject.name + " escapo y sera destruido");
                Enemigo enemy = GetComponent<Enemigo>();
                if (enemy != null)
                {
                    enemy.Die();
                }
            }
        }
    }


}
