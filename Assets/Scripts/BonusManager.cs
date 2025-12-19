
using System.Collections.Generic;
using UnityEngine;

public class BonusManager: MonoBehaviour
{
    public static BonusManager Instance { get; private set; }

    [SerializeField] private List<BonusConfig> bonusConfigList;

    private Dictionary<BonusType, BonusConfig> configMap = new Dictionary<BonusType, BonusConfig>();
    private Dictionary<BonusType, ActiveBonus> activeBonusesMap = new Dictionary<BonusType, ActiveBonus>();


    private void Awake()
    {
        Instance = this;
        foreach (var bonus in bonusConfigList)
        {
            configMap[bonus.type] = bonus;
        }
    }
    private void Update()
    {
        UpdateTimedBonuses();
    }

    private void UpdateTimedBonuses()
    {
        var toRemove = new List<BonusType>();

        foreach (var pair in activeBonusesMap)
        {
            var inst = pair.Value;

            if (!inst.bonusConfig.isPersistent)
            {
                inst.remainingTime -= Time.deltaTime;
                if (inst.remainingTime <= 0)
                    toRemove.Add(pair.Key);
            }
        }

        foreach (var type in toRemove)
            Remove(type);
    }

    public void Activate(BonusType type)
    {
        if (!configMap.TryGetValue(type, out var bonusConfig)) return;

        // en caso de volver a agarrar el mismo bonus, se resetea el tiempo
        if (activeBonusesMap.TryGetValue(type, out var existing))
        {
            existing.remainingTime = bonusConfig.duration;
            return;
        }

        activeBonusesMap[type] = new ActiveBonus(bonusConfig);

        bonusConfig.Apply();
    }

    public void Remove(BonusType type)
    {
        if (!activeBonusesMap.TryGetValue(type, out var activeBonus)) return;

        activeBonus.bonusConfig.Remove();
        activeBonusesMap.Remove(type);
    }

    public bool IsBonusActive(BonusType type)
    {
        return activeBonusesMap.ContainsKey(type);
    }
}

public enum BonusType
{
    Escudo,
    Laser,
    FireRateUp,
    DualShot
}

public class ActiveBonus
{
    public BonusConfig bonusConfig;
    public float remainingTime;

    public ActiveBonus(BonusConfig config)
    {
        this.bonusConfig = config;
        remainingTime = config.duration;
    }
}

