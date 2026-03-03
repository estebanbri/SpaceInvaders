using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    [SerializeField] private float baseCritChance = 0.05f;
    [SerializeField] private float critMinMultiplier = 1.8f;
    [SerializeField] private float critMaxMultiplier = 2.3f;

    private float bonusCritChance = 0f;

    public float CritChance => baseCritChance + bonusCritChance;
    public float CritMinMultiplier => critMinMultiplier;

    public float CritMaxMultiplier => critMaxMultiplier;

    public void AddCritChance(float amount)
    {
        bonusCritChance += amount;
        bonusCritChance = Mathf.Clamp(bonusCritChance, 0f, 0.5f);
    }
}