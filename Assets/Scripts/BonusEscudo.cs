using UnityEngine;

[CreateAssetMenu(menuName = "Bonus/Escudo")]
public class BonusEscudo : BonusConfig
{
    public override void Enable()
    {
        Escudo.Instance.AddEscudo();
    }

    public override void Disable()
    {
        Escudo.Instance.RemoveEscudo();
    }
}