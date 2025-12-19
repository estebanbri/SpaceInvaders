using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/Escudo")]
public class BonusEscudo : BonusConfig
{
    public override void Apply()
    {
        Escudo.Instance.AddEscudo();
    }

    public override void Remove()
    {
        Escudo.Instance.RemoveEscudo();
    }
}