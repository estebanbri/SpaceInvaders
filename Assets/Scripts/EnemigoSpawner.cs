using UnityEngine;

public class EnemigoSpawner : MonoBehaviour
{
    public static EnemigoSpawner Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnEnemy(
    GameObject prefab,
    PathData path)
    {
        GameObject enemy = Instantiate(prefab);

        var pathFollower = enemy.GetComponent<EnemigoPathFollower>();
        pathFollower.Initialize(path);

        EnemigoManager.Instance.RegisterEnemy();
    }
}