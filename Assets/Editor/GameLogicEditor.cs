using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameLogic))]
public class GameLogicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameLogic logic = (GameLogic)target;

        if (GUILayout.Button("Save config"))
        {
            if (logic.levelConfigSO != null)
            {
                var config = new LevelConfig
                {
                    layer = logic.layer,
                    row = logic.row,
                    col = logic.col,
                    extraCellConfigs = new List<ExtraCellConfig>()
                };
                foreach (var item in logic.extraCellConfigs)
                {
                    config.extraCellConfigs.Add(new ExtraCellConfig
                    {
                        direction = item.direction,
                        count = item.count,
                        offset = item.offset,
                        startPosition = item.startPosition
                    });
                }
                logic.levelConfigSO.levelConfigs.Add(config);
                EditorUtility.SetDirty(logic.levelConfigSO);
                Debug.Log("Config saved to LevelConfigSO.");
            }
            else
            {
                Debug.LogWarning("LevelConfigSO is not assigned.");
            }
        }

        int mainCount = 0;
        for (int i = 0; i < logic.layer; i++)
        {
            int curRow = logic.row - i;
            int curCol = logic.col - i;
            if (curRow <= 0 || curCol <= 0)
                break;
            mainCount += curRow * curCol;
        }
        int recommendExtraCount = (3 - mainCount % 3) % 3;
        int totalCount = mainCount;
        foreach (var item in logic.extraCellConfigs)
        {
            if (item != null)
            {
                totalCount += item.count;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Main Count", mainCount.ToString());
        EditorGUILayout.LabelField("Recommend Extra Count", recommendExtraCount.ToString() + " + 3 * n");
        EditorGUILayout.LabelField("Total Count", totalCount.ToString());
        if(totalCount % 3 != 0)
        {
            EditorGUILayout.HelpBox("Total count is not a multiple of 3, pleace adjust the layer, row and col.", MessageType.Warning);
        }
    }
}