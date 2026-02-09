using UnityEngine;

public class BonusBoxVisual : MonoBehaviour
{
    private Material material;
    private float t;

    void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        t += Time.deltaTime * 2f;
        float pulse = (Mathf.Sin(t) + 1f) / 2f; // 0..1

        material.SetFloat("_FlashAmount", Mathf.Lerp(0.15f, 0.35f, pulse));
    }
}
