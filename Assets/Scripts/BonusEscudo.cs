using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/Escudo")]
public class BonusEscudo : BonusDefinition
{
    public override void Apply()
    {
        Nave.Instance.GetEscudo().AddEscudo();
    }

    public override void Remove()
    {
        Nave.Instance.GetEscudo().RemoveEscudo();
    }
}