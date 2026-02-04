using UnityEngine;

public class WeaponMuzzleFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float flashDuration = 0.05f;

    private float timer;

    private void Awake()
    {
        sr.enabled = false;
    }

    public void Play()
    {
        sr.enabled = true;
        timer = flashDuration;
    }

    private void Update()
    {
        if (!sr.enabled) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            sr.enabled = false;
    }
}