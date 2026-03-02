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
    PathComponent originalPath,
    Vector2 formationOffset,
    bool reverse)
    {
        GameObject enemy = Instantiate(prefab);

        var follower = enemy.GetComponent<EnemigoPathFollower>();
        follower.Initialize(originalPath, formationOffset, reverse);

        EnemigoManager.Instance.RegisterEnemy();
    }
}