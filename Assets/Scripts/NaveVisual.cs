using System.Collections;
using UnityEngine;
using UnityEngine.Windows;

public class NaveVisual : MonoBehaviour
{
    [SerializeField] private float invulnerableTime = 2f;
    
    [SerializeField] private ParticleSystem leftPropulsorParticleSystem;
    [SerializeField] private ParticleSystem middlePropulsorParticleSystem;
    [SerializeField] private ParticleSystem rightPropulsorParticleSystem;

    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    private SpriteRenderer spriteRendererComponent;

    void Awake()
    {
        spriteRendererComponent = GetComponent<SpriteRenderer>();
        HidePropulsoresParticles();
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
        UpdateHorizontalThrusters(moveX);
    }

    public void AddVerticallMoveVisual(float moveY)
    {
        UpdateVerticalThrusters(moveY);
    }

    public void HidePropulsoresParticles()
    {
        SetEnabledParticleSystem(leftPropulsorParticleSystem, false);
        SetEnabledParticleSystem(middlePropulsorParticleSystem, false);
        SetEnabledParticleSystem(rightPropulsorParticleSystem, false);
    }

    private void UpdateHorizontalThrusters(float moveX)
    {
        if (moveX < 0)
        {
            ShowLeftPropulsorParticles();
        }
        else if (moveX > 0)
        {
            ShowRightPropulsorParticles();
        }
    }


    private void UpdateVerticalThrusters(float moveY)
    {
        if (moveY != 0)
        {
            ShowMiddlePropulsorParticles();
        }
    }

    private void ShowLeftPropulsorParticles()
    {
        SetEnabledParticleSystem(leftPropulsorParticleSystem, true);
    }

    private void ShowRightPropulsorParticles()
    {
        SetEnabledParticleSystem(rightPropulsorParticleSystem, true);
    }

    private void ShowMiddlePropulsorParticles()
    {
        SetEnabledParticleSystem(middlePropulsorParticleSystem, true);
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

    private void SetEnabledParticleSystem(ParticleSystem particleSystem, bool enabled) { 
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission; 
        emissionModule.enabled = enabled; 
    }

}
