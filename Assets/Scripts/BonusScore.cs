using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/Score")]
public class BonusScore : BonusDefinition
{
    public override void Apply()
    {
        GameManager.Instance.AddScore(10);
    }

    public override void Remove()
    {
        
    }
}