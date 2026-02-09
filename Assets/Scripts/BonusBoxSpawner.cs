using UnityEngine;

public class BonusBoxSpawner : MonoBehaviour
{
    public GameObject bonusBoxPrefab;

    public void Spawn(Vector3 position)
    {
        Instantiate(bonusBoxPrefab, position, Quaternion.identity);
    }
}