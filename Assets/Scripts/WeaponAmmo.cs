using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [SerializeField] private int damageAmount;

    private float ammoSpeed = 1f;
    private Vector3 direction;
    private FactionType ownerFaction;

    public void Init(Vector3 direction, FactionType factionType, float ammoSpeed)
    {
        this.direction = direction.normalized;
        this.ownerFaction = factionType;
        this.ammoSpeed = ammoSpeed;
    }

    void Update()
    {
        transform.position += direction * ammoSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable) &&
            collision.TryGetComponent(out FactionComponent factionComp))
        {
            if (factionComp.Faction != ownerFaction)
            {
                damageable.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }


}
