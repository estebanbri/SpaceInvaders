public class BonusRuntime
{
    public BonusDefinition definition;
    public float remainingTime;

    public BonusRuntime(BonusDefinition definition)
    {
        this.definition = definition;
        this.remainingTime = definition.duration;
    }
}