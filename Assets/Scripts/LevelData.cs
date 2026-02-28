using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    public List<SpawnEvent> spawnEvents;
    public GameObject bossPrefab;
}