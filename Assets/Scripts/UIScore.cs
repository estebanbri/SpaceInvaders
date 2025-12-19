using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField]    private TextMeshProUGUI scoreText;


    // Update is called once per frame
    void Update()
    {
        scoreText.text = GameManager.Instance.GetScore().ToString();

    }
}
