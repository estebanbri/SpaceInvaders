using UnityEngine;

public class DestroyOutsideCamera : MonoBehaviour
{
    [SerializeField] private float bottomMargin = 2f;
    private Camera mainCamera;

    public enum OutsideCameraBehavior
    {
        DestroyOnly,
        DestroyAndRespawn
    }

    [SerializeField] private OutsideCameraBehavior behavior = OutsideCameraBehavior.DestroyOnly;

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
            NotifyExit();
            Destroy(gameObject);
        }
    }

    private void NotifyExit()
    {
        if (!TryGetComponent(out FactionComponent faction) ||
            faction.Faction != FactionType.Enemy)
            return;

        LevelManager.Instance.OnEnemyExitedCamera(gameObject);
    }

    public OutsideCameraBehavior GetBehavior() {
        return behavior;
    }


}
