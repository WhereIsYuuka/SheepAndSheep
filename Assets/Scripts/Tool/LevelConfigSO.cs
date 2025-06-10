using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelConfig
{
    public int layer;
    public int row;
    public int col;
    public List<ExtraCellConfig> extraCellConfigs;
}

[CreateAssetMenu(fileName = "LevelConfig", menuName = "SOs/LevelConfigSO", order = 1)]
public class LevelConfigSO : ScriptableObject
{
    public List<LevelConfig> levelConfigs = new();
}