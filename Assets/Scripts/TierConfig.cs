using System.Collections.Generic;

[System.Serializable]
public class TierConfig
{
    public List<EnemyCount> enemies;
    public FormationPattern formationPattern = FormationPattern.Grid;
}