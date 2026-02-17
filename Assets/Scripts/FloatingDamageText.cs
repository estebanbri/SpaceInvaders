using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float duration = 1f;

    public void SetDamage(int damage, bool isCritical = false)
    {
        text.text = damage.ToString();
        text.color = isCritical ? Color.yellow : Color.white;
        Destroy(gameObject, duration);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}
