using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/Vida")]
public class BonusVida : BonusDefinition
{
    public override void Apply()
    {
        // GameManager.Instance.AddVida();
    }

    public override void Remove()
    {

    }
}