using UnityEngine;

public class EnemigoManager : MonoBehaviour
{
    public static EnemigoManager Instance;

    public int ActiveEnemies { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterEnemy()
    {
        ActiveEnemies++;
    }

    public void UnregisterEnemy()
    {
        ActiveEnemies--;
    }
}