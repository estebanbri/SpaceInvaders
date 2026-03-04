using System;
using System.Collections;
using UnityEngine;

public class NaveVisual : MonoBehaviour
{
    [SerializeField] private float invulnerableTime = 2f;

    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private GameObject explosionPrefab;
    private SpriteRenderer spriteRendererComponent;
    private SpriteRenderer[] spriteRenderers;
    private Animator animator;

    private Nave nave;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        animator = GetComponent<Animator>();
        spriteRendererComponent = GetComponent<SpriteRenderer>();
        nave = GetComponentInParent<Nave>();
    }

    public IEnumerator BlinkSpriteDuringInvulnerabilityCoroutine()
    {
        float elapsed = 0f;
        Color color = spriteRendererComponent.color;
        float blinkInterval = 0.2f;
        while (elapsed < invulnerableTime)
        {
            color.a = (color.a == 1f) ? 0.1f : 1f;
            spriteRendererComponent.color = color;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Estado final consistente
        color.a = 1f;
        spriteRendererComponent.color = color;
    }

    public void AddHorizontalMoveVisual(float moveX)
    {
        UpdateNaveSprite(moveX);
    }

    private void UpdateNaveSprite(float moveX)
    {
        if (moveX < 0)
        {
            spriteRendererComponent.sprite = leftSprite;
        }
        else if (moveX > 0)
        {
            spriteRendererComponent.sprite = rightSprite;
        }
        else
        {
            spriteRendererComponent.sprite = idleSprite;
        }
    }

    public void playDeathEffect()
    {
       StartCoroutine(SpawnExplosionCorutine());
       Destroy(transform.root.gameObject);
    }

    private IEnumerator SpawnExplosionCorutine()
    {
        // Desactivar sprite inmediatamente
        foreach (var sr in spriteRenderers)
            sr.enabled = false;

        SpawnExplosion();

        yield return new WaitForSeconds(0.3f);
    }

    private GameObject SpawnExplosion(float radius = 0f)
    {
        if (explosionPrefab == null) return null;

        Vector3 spawnPosition = transform.position;

        if (radius > 0f)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * radius;
            spawnPosition += (Vector3)randomOffset;
        }

        return Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
    }
}
