using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WeaponAmmo : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
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

    public FactionType GetFaction() => ownerFaction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ammoState != AmmoState.Flying)
            return;

        // Verificamos si colisiona con algo que implemente IDamageable
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            // Verificamos facción si tiene
            bool isEnemy = true;
            if (collision.TryGetComponent(out FactionComponent factionComp))
            {
                isEnemy = factionComp.Faction != ownerFaction;
            }

            if (isEnemy)
            {
                // Aplicamos daño
                damageable.TakeDamage(damageAmount, transform.position);

                // Impacto
                SetState(AmmoState.Impact);
            }
        }
    }

    public int GetDamageAmount() => damageAmount;

    private void SetState(AmmoState newState)
    {
        ammoState = newState;
        visual?.UpdateSpriteByState(newState);

        switch (ammoState)
        {
            case AmmoState.Impact:
                if (col != null) col.enabled = false;
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

public enum AmmoState
{
    Flying,
    Impact
}
