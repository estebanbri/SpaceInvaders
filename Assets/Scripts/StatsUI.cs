using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valuesTextMesh;
    
    void Update()
    {
        valuesTextMesh.text = GameManager.Instance.GetScore() + "\n" +
                                      GameManager.Instance.GetLiveCount();

    }
}
