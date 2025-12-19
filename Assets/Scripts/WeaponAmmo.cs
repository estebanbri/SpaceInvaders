using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{

    [SerializeField] private int damageAmount;
    private float ammoSpeed = 1;

    void Update()
    {
        transform.position += transform.up * ammoSpeed * Time.deltaTime;
    }

    public void SetAmmoSpeed(float ammoSpeed) {
        this.ammoSpeed = ammoSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damageAmount);
            Destroy(gameObject);
        }
    }

   
}
