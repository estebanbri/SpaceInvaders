using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    private List<GameObject> enemyPrefabs;

    private int rows;
    private int columns;
    private float spacingX;
    private float spacingY;

    [Header("Movement")]
    [SerializeField] private float stepDown = 0.5f;
    [SerializeField] private float borderPadding = 0.5f;

    private float moveSpeed;

    private int direction = 1;
    private float leftLimit;
    private float rightLimit;
    private bool isActive = false;
    private int enemiesAlive;

    public System.Action OnFormationCleared;

    #region INITIALIZATION

    public void InitializeProcedural(ProceduralWaveData data)
    {
        enemyPrefabs = data.enemyPrefabs;

        rows = data.rows;
        columns = data.columns;
        spacingX = data.spacingX;
        spacingY = data.spacingY;
        moveSpeed = data.moveSpeed;

        CalculateBounds();
        GenerateGrid(data);
        StartCoroutine(EnterAnimation());
    }

    #endregion

    void CalculateBounds()
    {
        Camera cam = Camera.main;
        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        leftLimit = min.x + borderPadding;
        rightLimit = max.x - borderPadding;
    }

    void GenerateGrid(ProceduralWaveData data)
    {
        enemiesAlive = 0;

        float startX = -(columns - 1) * spacingX * 0.5f;
        float startY = 0f;

        int tier = data.tier; // tier del wave data

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 localPos = new Vector3(
                    startX + c * spacingX,
                    startY - r * spacingY,
                    0f
                );

                GameObject prefab = GetRandomEnemyPrefab();
                if (prefab == null) continue;

                GameObject enemyGO = Instantiate(prefab, transform);
                enemyGO.transform.localPosition = localPos;

                Enemigo enemigo = enemyGO.GetComponent<Enemigo>();

                enemigo.SetFormation(this);
                enemigo.ApplyProceduralScaling(
                    data.healthMultiplier,
                    data.fireRateMultiplier
                );

                // Si el enemigo tiene WeaponController, aplicamos tier
                if (enemigo.GetWeaponController != null && enemigo.GetWeaponController.GetWeaponDefault() != null)
                {
                    enemigo.GetWeaponController.Equip(enemigo.GetWeaponController.GetWeaponDefault(), tier);
                }

                enemiesAlive++;
            }
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("No enemy prefabs assigned for procedural wave");
            return null;
        }

        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }

    IEnumerator EnterAnimation()
    {
        Vector3 start = new Vector3(0, 8f, 0);
        Vector3 target = new Vector3(0, 5f, 0);

        transform.position = start;

        float t = 0;
        float duration = 1.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }

        transform.position = target;
        isActive = true;
        ActivateEnemies();
    }

    void Update()
    {
        if (!isActive) return;
        Move();
    }

    void Move()
    {
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        float halfWidth = (columns - 1) * spacingX * 0.5f;
        float leftEdge = transform.position.x - halfWidth;
        float rightEdge = transform.position.x + halfWidth;

        if (rightEdge >= rightLimit && direction > 0)
            StepDown();
        else if (leftEdge <= leftLimit && direction < 0)
            StepDown();
    }

    void StepDown()
    {
        direction *= -1;
        transform.position += Vector3.down * stepDown;
    }

    public void NotifyEnemyKilled()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            OnFormationCleared?.Invoke();
            Destroy(gameObject);
        }
    }

    void ActivateEnemies()
    {
        Enemigo[] enemies = GetComponentsInChildren<Enemigo>();

        foreach (var enemy in enemies)
        {
            enemy.SetState(EnemyState.Attacking);
        }

        Debug.Log("[FORMATION] Enemies switched to ATTACKING state");
    }
}
