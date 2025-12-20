using UnityEngine;

public class EnemigoSpawner : MonoBehaviour
{
    [SerializeField] private float spawnMinX = -5f;
    [SerializeField] private float spawnMaxX = 5f;
    
    private Camera mainCamera;
    [SerializeField] private float spawnMargin = 1.5f;

    [SerializeField] private SpawnEntry[] spawnEntries;

    [SerializeField] private float spawnInterval = 0.1f;
    private float timer;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval) {
            Spawn();
            timer = 0f;
        }
    }

    private void Spawn() {
        // seleccionar que spawnear
        SpawnEntry entry = GetRandomEntry();
        if (entry == null) return;
        // controlar cantidad en escena
        if (entry.aliveCount >= entry.maxAliveCount) return;

        entry.aliveCount += entry.ordaQuantity;

        GameObject objSpawned = null;
        float centerX = Random.Range(spawnMinX, spawnMaxX);
        float spacing = 1.2f; // distancia entre enemigos

        for (int i = 0; i < entry.ordaQuantity; i++) {
            float offset = (i - (entry.ordaQuantity - 1) / 2f) * spacing;
            float cameraTopY = mainCamera.transform.position.y + mainCamera.orthographicSize;
            float spawnY = cameraTopY + spawnMargin;
            objSpawned = Instantiate(entry.prefab, new Vector2(centerX + offset, spawnY), Quaternion.identity);
            // Aviso cuando muere
            SpawnedEntity spawned = objSpawned.AddComponent<SpawnedEntity>();
            spawned.EjecutarAlDestruirse(() => entry.aliveCount--);
        }
    }

    /* 
     * Este método utiliza acumulación de probabilidades para mapear un número aleatorio a un rango específico, 
     * permitiendo seleccionar un elemento de forma proporcional a su peso relativo.
    */
    private SpawnEntry GetRandomEntry()
    {
        // 1) Calcular el peso total sumando todas las probabilidades
        //    Este valor define el rango completo de selección aleatoria
        float totalWeight = 0f;
        foreach (var e in spawnEntries)
            totalWeight += e.enemySpawnProbability;

        // 2) Generar un número aleatorio dentro del rango [0, totalWeight)
        //    Random.value devuelve un valor entre 0 y 1
        //    Al multiplicarlo por totalWeight, se escala al rango total
        float roll = Random.value * totalWeight;

        // 3) Variable acumuladora para recorrer los rangos de probabilidad
        float cumulative = 0f;

        // 4) Recorrer cada entrada sumando su probabilidad al acumulado
        foreach (var e in spawnEntries)
        {
            // Sumar el peso actual al acumulado
            cumulative += e.enemySpawnProbability;

            // 5) Verificar si el valor aleatorio cayó dentro de este rango
            //    Si es así, esta entrada es la seleccionada
            if (roll <= cumulative)
                return e;
        }

        // 6) Fallback de seguridad
        //    En condiciones normales no debería alcanzarse,
        //    pero evita errores si la lista está vacía o mal configurada
        return null;
    }
}
