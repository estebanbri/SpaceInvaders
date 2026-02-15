using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damageAmount, Vector3? attackerPos);
}
