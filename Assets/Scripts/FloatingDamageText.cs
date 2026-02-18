using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float scaleMultiplier = 1f; // para crit

    public void SetDamage(int damage, bool isCritical = false)
    {
        text.text = damage.ToString();

        if (isCritical)
        {
            text.color = Color.yellow;
            transform.localScale *= 1.5f * scaleMultiplier; // más grande para crit
        }
        else
        {
            text.color = Color.white;
            transform.localScale *= scaleMultiplier;
        }

        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}