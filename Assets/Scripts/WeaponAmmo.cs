using System.Collections;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [SerializeField] private int damageAmount;
    [SerializeField] private float timeBeforeDestroy = 0.1f;

    private float ammoSpeed;
    private Vector3 direction;
    private FactionType ownerFaction;
    private AmmoState ammoState;

    private WeaponAmmoVisual visual;
    private Collider2D col;

    private void Awake()
    {
        visual = GetComponentInChildren<WeaponAmmoVisual>();
        col = GetComponent<Collider2D>();
    }

    public void Init(Vector3 direction, FactionType factionType, float ammoSpeed)
    {
        this.direction = direction.normalized;
        this.ownerFaction = factionType;
        this.ammoSpeed = ammoSpeed;

        SetState(AmmoState.Flying);
    }

    private void Update()
    {
        if (ammoState != AmmoState.Flying)
            return;

        transform.position += direction * ammoSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ammoState != AmmoState.Flying)
            return;

        if (collision.TryGetComponent(out IDamageable damageable) &&
            collision.TryGetComponent(out FactionComponent factionComp) &&
            factionComp.Faction != ownerFaction)
        {
            damageable.TakeDamage(damageAmount, transform.position);
            SetState(AmmoState.Impact);
        }
    }

    private void SetState(AmmoState newState)
    {
        ammoState = newState;
        visual.UpdateSpriteByState(newState);

        switch (ammoState)
        {
            case AmmoState.Impact:
                col.enabled = false;
                ammoSpeed = 0f;
                StartCoroutine(DestroyRoutine());
                break;
        }
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(timeBeforeDestroy);
        Destroy(gameObject);
    }
}

public enum AmmoState {
    Flying,
    Impact
}
