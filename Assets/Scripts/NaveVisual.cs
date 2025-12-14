using System.Collections;
using UnityEngine;

public class NaveVisual : MonoBehaviour
{
    [SerializeField] private float invulnerableTime = 2f;
    [SerializeField] private ParticleSystem leftPropulsorParticleSystem;
    [SerializeField] private ParticleSystem middlePropulsorParticleSystem;
    [SerializeField] private ParticleSystem rightPropulsorParticleSystem;

    private SpriteRenderer spriteRendererComponent;
    private void Awake()
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

    public void HidePropulsoresParticles()
    {
        SetEnabledParticleSystem(leftPropulsorParticleSystem, false);
        SetEnabledParticleSystem(middlePropulsorParticleSystem, false);
        SetEnabledParticleSystem(rightPropulsorParticleSystem, false);
    }

    public void ShowLeftPropulsorParticles()
    {
        SetEnabledParticleSystem(leftPropulsorParticleSystem, true);
    }

    public void ShowRightPropulsorParticles()
    {
        SetEnabledParticleSystem(rightPropulsorParticleSystem, true);
    }

    public void ShowMiddlePropulsorParticles()
    {
        SetEnabledParticleSystem(middlePropulsorParticleSystem, true);
    }

    public void UpdateHorizontalThrusters(float moveX)
    {
        if (moveX < 0)
        {
            ShowRightPropulsorParticles();
        }
        else if (moveX > 0)
        {
            ShowLeftPropulsorParticles();
        }
    }

    public void UpdateVerticalThrusters(float moveY)
    {
        if (moveY != 0)
        {
            ShowMiddlePropulsorParticles();
        }
    }

    private void SetEnabledParticleSystem(ParticleSystem particleSystem, bool enabled) { 
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission; 
        emissionModule.enabled = enabled; 
    }
}
