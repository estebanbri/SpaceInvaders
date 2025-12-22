using UnityEngine;

public class DestroyOutsideCamera : MonoBehaviour
{
    [SerializeField] private float bottomMargin = 2f;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        float cameraBottomY =
            mainCamera.transform.position.y - mainCamera.orthographicSize;

        if (transform.position.y < cameraBottomY - bottomMargin)
        {
            if (gameObject.TryGetComponent(out FactionComponent factionComponent)) {
                if (factionComponent.Faction == FactionType.Enemy) {
                    LevelManager.Instance.OnEnemyKilled();
                }
            }
            Destroy(gameObject);
        }
    }
}
