using UnityEngine;

public class PickupBonus : MonoBehaviour
{
    [SerializeField] private BonusDefinition bonusDefinition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Nave nave))
        {
            BonusManager.Instance.ApplyBonus(bonusDefinition);
            Destroy(gameObject);
        }
    }

    public BonusDefinition GetBonusDefinition() { 
        return bonusDefinition;
    }
}
