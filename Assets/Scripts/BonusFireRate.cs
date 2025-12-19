using System.Collections;
using UnityEngine;

public class FireRateBonus : MonoBehaviour
{
    [SerializeField] private float multiplier = 0.5f;
    [SerializeField] private float duration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out WeaponController weaponController))
        {
            StartCoroutine(ApplyBonus(weaponController));
            Destroy(gameObject);
        }
    }

    private IEnumerator ApplyBonus(WeaponController weaponController)
    {
        weaponController.ApplyFireRateBonus(multiplier);
        yield return new WaitForSeconds(duration);
        weaponController.ApplyFireRateBonus(1f);
    }
}