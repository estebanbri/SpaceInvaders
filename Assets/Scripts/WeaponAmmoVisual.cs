using UnityEngine;

public class WeaponAmmoVisual : MonoBehaviour
{
    [SerializeField] private Sprite flyingSprite;
    [SerializeField] private Sprite impactSprite;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void UpdateSpriteByState(AmmoState state)
    {
        sr.sprite = state == AmmoState.Impact
            ? impactSprite
            : flyingSprite;
    }
}