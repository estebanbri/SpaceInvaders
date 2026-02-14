using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    private List<GameObject> enemyPrefabs;
    private float spacingX = 1f;
    private float spacingY = 1f;
    private float moveSpeed;

    private FormationPattern formationPattern;

    [Header("Movement")]
    [SerializeField] private float stepDown = 0.5f;
    [SerializeField] private float borderPadding = 0.5f;

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
        spacingX = data.spacingX;
        spacingY = data.spacingY;
        moveSpeed = data.moveSpeed;
        formationPattern = data.formationPattern;

        CalculateBounds();
        GenerateEnemies(data);
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

    void GenerateEnemies(ProceduralWaveData data)
    {
        enemiesAlive = 0;
        int totalEnemies = enemyPrefabs.Count;

        for (int i = 0; i < totalEnemies; i++)
        {
            Vector3 localPos = CalculatePosition(i, totalEnemies, formationPattern);

            GameObject prefab = enemyPrefabs[i];
            if (prefab == null) continue;

            GameObject enemyGO = Instantiate(prefab, transform);
            enemyGO.transform.localPosition = localPos;

            Enemigo enemigo = enemyGO.GetComponent<Enemigo>();
            enemigo.SetFormation(this);
            enemigo.ApplyProceduralScaling(data.healthMultiplier, data.fireRateMultiplier);

            // Aplicar tier al arma si tiene
            if (enemigo.GetWeaponController != null && enemigo.GetWeaponController.GetWeaponDefault() != null)
                enemigo.GetWeaponController.Equip(enemigo.GetWeaponController.GetWeaponDefault(), data.cycle);

            enemiesAlive++;
        }
    }

    Vector3 CalculatePosition(int index, int total, FormationPattern pattern)
    {
        switch (pattern)
        {
            case FormationPattern.Grid:
                int columns = Mathf.CeilToInt(Mathf.Sqrt(total));
                int row = index / columns;
                int col = index % columns;
                float startX = -(columns - 1) * spacingX * 0.5f;
                float startY = row * -spacingY;
                return new Vector3(startX + col * spacingX, startY, 0);

            case FormationPattern.Line:
                float startLineX = -(total - 1) * spacingX * 0.5f;
                return new Vector3(startLineX + index * spacingX, 0, 0);

            case FormationPattern.Circle:
                float radius = Mathf.Max(total * 0.2f, 1f);
                float angleStep = 360f / total;
                float angle = index * angleStep * Mathf.Deg2Rad;
                return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

            case FormationPattern.Star:
                // Patrón estrella simple: alterna entre radio corto y largo
                float radiusOuter = Mathf.Max(total * 0.2f, 1f);
                float radiusInner = radiusOuter * 0.5f;
                float stepAngle = 360f / total;
                float starAngle = index * stepAngle * Mathf.Deg2Rad;
                float r = (index % 2 == 0) ? radiusOuter : radiusInner;
                return new Vector3(Mathf.Cos(starAngle) * r, Mathf.Sin(starAngle) * r, 0);

            default:
                return Vector3.zero;
        }
    }

    IEnumerator EnterAnimation()
    {
        Vector3 start = new Vector3(0, 8f, 0);
        Vector3 target = transform.position;
        transform.position = start;

        float t = 0f;
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

        // Ajustamos bounds según tipo de patrón
        float halfWidth = 0f;

        if (formationPattern == FormationPattern.Grid || formationPattern == FormationPattern.Line)
            halfWidth = (enemyPrefabs.Count - 1) * spacingX * 0.5f;
        else if (formationPattern == FormationPattern.Circle || formationPattern == FormationPattern.Star)
            halfWidth = Mathf.Max(enemyPrefabs.Count * 0.1f, 1f); // radio estimado

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
        Debug.Log("[FORMATION] Enemy killed. Remaining:" + enemiesAlive);

        if (enemiesAlive <= 0)
        {
            Debug.Log("[FORMATION] All enemies cleared!");
            OnFormationCleared?.Invoke();
            Destroy(gameObject);
        }
    }

    void ActivateEnemies()
    {
        Enemigo[] enemies = GetComponentsInChildren<Enemigo>();
        foreach (var enemy in enemies)
            enemy.SetState(EnemyState.Attacking);

        Debug.Log("[FORMATION] Enemies switched to ATTACKING state");
    }
}
