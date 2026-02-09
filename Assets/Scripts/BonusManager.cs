using System;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    private Dictionary<BonusDefinition, BonusRuntime> activeBonuses =
        new Dictionary<BonusDefinition, BonusRuntime>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        UpdateBonuses();
    }

    // ============================
    // PUBLIC API
    // ============================

    public void ApplyBonus(BonusDefinition bonus)
    {
        // Bonus instantáneo (vida, score, etc)
        if (bonus.isTemporary)
        {
            if (activeBonuses.TryGetValue(bonus, out BonusRuntime runtime))
            {
                runtime.remainingTime = bonus.duration;
                return;
            }
        }

        // Bonus nuevo
        bonus.Apply();
        if (!IsBonusActive(bonus)) {
            activeBonuses.Add(bonus, new BonusRuntime(bonus));
        }
    }

    public bool IsBonusActive(BonusDefinition bonus)
    {
        return activeBonuses.ContainsKey(bonus);
    }

    public void RemoveBonus(BonusDefinition bonus)
    {
        if (!activeBonuses.TryGetValue(bonus, out BonusRuntime runtime))
            return;

        bonus.Remove();
        activeBonuses.Remove(bonus);
       
    }

    // ============================
    // INTERNAL
    // ============================

    private void UpdateBonuses()
    {
        if (activeBonuses.Count == 0) return;

        List<BonusDefinition> toRemove = new List<BonusDefinition>();

        foreach (var pair in activeBonuses)
        {
            pair.Value.remainingTime -= Time.deltaTime;

            if (pair.Key.isTemporary && pair.Value.remainingTime <= 0f)
                toRemove.Add(pair.Key);
        }

        foreach (var bonus in toRemove)
            RemoveBonus(bonus);
    }

    public IReadOnlyCollection<BonusDefinition> GetActiveBonuses()
    {
        return activeBonuses.Keys;
    }

    public List<BonusDefinition> GetPickableBonuses(
    IReadOnlyCollection<BonusDefinition> pool,
    int amount
)
    {
        List<BonusDefinition> candidates = new List<BonusDefinition>();

        foreach (var bonus in pool)
        {
            if (!bonus.canAppearInPicker)
                continue;

            if (bonus.isTemporary && IsBonusActive(bonus))
                continue;

            candidates.Add(bonus);
        }

        // Shuffle simple
        for (int i = 0; i < candidates.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, candidates.Count);
            (candidates[i], candidates[rand]) = (candidates[rand], candidates[i]);
        }

        return candidates.GetRange(0, Mathf.Min(amount, candidates.Count));
    }
}
