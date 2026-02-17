using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Enemy Progression")]
    [SerializeField] private GameObject baseEnemy;
    [SerializeField] private List<GameObject> advancedEnemies;

    [Header("Formation")]
    [SerializeField] private FormationController formationPrefab;

    [Header("MiniBoss")]
    [SerializeField] private GameObject miniBossPrefab;
    [SerializeField] private BossHealthBarUI bossHealthBar;
    [SerializeField] private int wavesPerCycle = 3;

    [Header("Parallax")]
    [SerializeField] private VerticalParallax parallax;

    [Header("Bonus")]
    [SerializeField] private BonusBoxSpawner bonusBoxSpawner;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Grid Limits")]
    [SerializeField] private float horizontalSpacing = 1.5f;
    [SerializeField] private float verticalSpacing = 1.3f;

    private int currentWave = 1;
    private FormationController activeFormation;

    private float screenLimitMinX = -6f;
    private float screenLimitMaxX = 6f;
    private float spawnPositionY = 7f;

    [Header("Asteroid InterWave")]
    [SerializeField] private AsteroidSpawner asteroidSpawner;
    [SerializeField] private float asteroidDuration = 15f;
    [SerializeField] private float asteroidChance = 0.35f;

    private bool lastWasAsteroids = false;
    private int wavesSinceAsteroids = 0;

    [Header("Asteroid Warning UI")]
    [SerializeField] private TextMeshProUGUI asteroidWarningText;

    [SerializeField] private Image darkOverlay;

    [SerializeField] float targetDarknessAsteroidBackground = 0.4f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartWave();
    }

    private void StartWave()
    {
        UpdateWaveUI();

        if (activeFormation != null)
        {
            activeFormation.OnFormationCleared = null;
            Destroy(activeFormation.gameObject);
            activeFormation = null;
        }

        if (IsMiniBossWave())
            SpawnMiniBoss();
        else
            SpawnProceduralFormation();
    }

    private bool IsMiniBossWave()
    {
        return currentWave % (wavesPerCycle + 1) == 0;
    }

    private void SpawnProceduralFormation()
    {
        Vector3 formationPos = new Vector3(0, 4f, 0);
        activeFormation = Instantiate(formationPrefab, formationPos, Quaternion.identity);

        ProceduralWaveData waveData = GenerateWaveData();
        activeFormation.InitializeProcedural(waveData);

        activeFormation.OnFormationCleared += OnFormationCleared;
    }

    private void OnFormationCleared()
    {
        if (activeFormation != null)
            activeFormation.OnFormationCleared -= OnFormationCleared;

        StartCoroutine(HandlePostWave());
    }

    private IEnumerator HandlePostWave()
    {
        yield return new WaitForSeconds(1f);

        bool forceAsteroids = wavesSinceAsteroids >= 3;
        bool spawnAsteroids = false;

        if (!lastWasAsteroids)
        {
            if (forceAsteroids)
                spawnAsteroids = true;
            else
                spawnAsteroids = Random.value < asteroidChance;
        }

        if (spawnAsteroids)
        {
            lastWasAsteroids = true;
            wavesSinceAsteroids = 0;

            yield return StartCoroutine(StartAsteroidInterWave());
        }
        else
        {
            lastWasAsteroids = false;
            wavesSinceAsteroids++;
        }

        currentWave++;
        StartWave();
    }

    private IEnumerator StartAsteroidInterWave()
    {
        // 🔥 1️⃣ Shake
        yield return StartCoroutine(ScreenShake(0.8f, 0.15f));

        // 🔇 2️⃣ Silencio dramático
        yield return new WaitForSeconds(0.2f);

        // 🌑 3️⃣ Oscurecer fondo
        yield return StartCoroutine(FadeOverlay(targetDarknessAsteroidBackground, 0.4f));

        // ⚠️ 4️⃣ Mostrar texto con flicker
        yield return StartCoroutine(ShowAsteroidWarning());

        // ⏳ 5️⃣ Pequeña pausa antes de empezar tormenta
        yield return new WaitForSeconds(0.3f);

        asteroidSpawner.StartSpawning();

        yield return new WaitForSeconds(asteroidDuration);

        asteroidSpawner.StopSpawning();

        // 🌑 6️⃣ Quitar oscuridad
        yield return StartCoroutine(FadeOverlay(0f, 0.6f));
    }

    private IEnumerator FadeOverlay(float to, float duration)
    {
        float timer = 0f;
        float startAlpha = darkOverlay.color.a;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, to, timer / duration);

            Color c = darkOverlay.color;
            darkOverlay.color = new Color(c.r, c.g, c.b, alpha);

            yield return null;
        }

        Color final = darkOverlay.color;
        darkOverlay.color = new Color(final.r, final.g, final.b, to);
    }

    private IEnumerator ShowAsteroidWarning()
    {
        asteroidWarningText.gameObject.SetActive(true);

        Color baseColor = asteroidWarningText.color;
        asteroidWarningText.color = baseColor;

        float fadeDuration = 0.5f;
        float displayTime = 1.5f;

        // 🔹 Fade In con leve escala
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeDuration;

            float alpha = Mathf.Lerp(0f, 1f, progress);


            // 🎛 Flicker sutil
            float flicker = Mathf.Sin(Time.time * 80f) * 0.8f;
            float finalAlpha = Mathf.Clamp01(alpha + flicker);

            asteroidWarningText.color = new Color(baseColor.r, baseColor.g, baseColor.b, finalAlpha);


            // Escala leve
            float scale = Mathf.Lerp(0.9f, 1f, progress);
            asteroidWarningText.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        // 🔹 Fade Out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeDuration;

            float alpha = Mathf.Lerp(1f, 0f, progress);

            float finalAlpha = Mathf.Clamp01(alpha);

            asteroidWarningText.color = new Color(baseColor.r, baseColor.g, baseColor.b, finalAlpha);

            yield return null;
        }

        asteroidWarningText.gameObject.SetActive(false);
    }

    private ProceduralWaveData GenerateWaveData()
    {
        ProceduralWaveData data = new ProceduralWaveData();
        data.enemyPrefabs = new List<GameObject>();

        int rows = 4;
        int columns = 7;
        int totalSlots = rows * columns;

        data.fixedRows = rows;
        data.fixedColumns = columns;
        data.spacingX = horizontalSpacing;
        data.spacingY = verticalSpacing;
        data.invertShape = currentWave % 2 == 0;

        int unlockedTypes = Mathf.Clamp((currentWave - 2) / 3 + 1, 0, advancedEnemies.Count);
        int specialCount = Mathf.Clamp(currentWave - 1, 0, totalSlots);

        Vector2 center = new Vector2(columns / 2f, rows / 2f);
        List<(int index, float distance)> positions = new List<(int, float)>();
        for (int i = 0; i < totalSlots; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float dist = Vector2.Distance(new Vector2(col, row), center);
            positions.Add((i, dist));
        }
        positions.Sort((a, b) => a.distance.CompareTo(b.distance));

        GameObject[] finalArray = new GameObject[totalSlots];
        for (int i = 0; i < totalSlots; i++)
            finalArray[i] = baseEnemy;

        for (int i = 0; i < specialCount && i < positions.Count; i++)
        {
            if (unlockedTypes <= 0) break;
            int posIndex = positions[i].index;
            int randomType = Random.Range(0, unlockedTypes);
            finalArray[posIndex] = advancedEnemies[randomType];
        }

        data.enemyPrefabs.AddRange(finalArray);
        data.moveSpeed = 2f + currentWave * 0.15f;
        data.healthMultiplier = 1f + currentWave * 0.1f;
        data.fireRateMultiplier = 1f + currentWave * 0.05f;
        data.cycle = currentWave;

        return data;
    }

    private void SpawnMiniBoss()
    {
        GameObject bossGO = Instantiate(miniBossPrefab, new Vector3(0, 7f, 0), Quaternion.identity); // fuera de pantalla
        Enemigo boss = bossGO.GetComponent<Enemigo>();

        if (boss != null)
        {
            boss.ConfigureByCycle(currentWave);
            boss.OnEnemyDied += OnMiniBossDied;
            // Lanzamos la entrada épica
            StartCoroutine(BossEntranceSequence(boss));
        }
    }

    private ParticleSystem CreateBossAura(Transform parent)
    {
        GameObject psGO = new GameObject("BossAura");
        psGO.transform.SetParent(parent);
        psGO.transform.localPosition = Vector3.zero;
        psGO.transform.localRotation = Quaternion.identity;

        ParticleSystem ps = psGO.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = 0f;
        main.startSize = 1f;
        main.loop = true;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 20f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.yellow, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.6f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));

        return ps;
    }

    private IEnumerator BossEntranceSequence(Enemigo boss)
    {
        Vector3 startPos = new Vector3(0, 7f, 0);
        Vector3 targetPos = new Vector3(0, 4f, 0);

        float enterDuration = 3f;
        float timer = 0f;

        // Partículas opcionales
        ParticleSystem[] particles = boss.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            ps.Play();
        }

        while (timer < enterDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / enterDuration);

            // Movimiento vertical con oscilación leve
            float oscillation = Mathf.Sin(timer * 3f) * 0.2f;
            boss.transform.position =
                Vector3.Lerp(startPos, targetPos, t) + Vector3.up * oscillation;

            yield return null;
        }

        boss.transform.position = targetPos;

        // Pausa dramática
        yield return new WaitForSeconds(1f);

        boss.SetState(EnemyState.Idle);

        if (bossHealthBar != null)
            bossHealthBar.Bind(boss);
    }




    private void OnMiniBossDied()
    {
        if (bonusBoxSpawner != null)
            bonusBoxSpawner.Spawn(GetBonusSpawnPosition());

        currentWave++;
        StartWave();
    }

    private void UpdateWaveUI()
    {
        waveText.text = "WAVE " + currentWave;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    private Vector3 GetBonusSpawnPosition()
    {
        float x = Random.value < 0.5f ? screenLimitMinX : screenLimitMaxX;
        return new Vector3(x, spawnPositionY, 0f);
    }

    private IEnumerator ScreenShake(float duration, float maxMagnitude)
    {
        Camera cam = Camera.main;
        Vector3 originalPos = cam.transform.position;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float progress = timer / duration;
            float currentMagnitude = Mathf.Lerp(0f, maxMagnitude, progress);

            float offsetX = Random.Range(-1f, 1f) * currentMagnitude;
            float offsetY = Random.Range(-1f, 1f) * currentMagnitude;

            cam.transform.position = originalPos + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        cam.transform.position = originalPos;
    }

}
