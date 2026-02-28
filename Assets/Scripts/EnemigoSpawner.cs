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
    PathComponent path)
    {
        GameObject enemy = Instantiate(prefab);

        var follower = enemy.GetComponent<EnemigoPathFollower>();
        follower.Initialize(path);

        EnemigoManager.Instance.RegisterEnemy();
    }
}