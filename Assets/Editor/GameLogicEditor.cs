using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameLogic))]
public class GameLogicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameLogic logic = (GameLogic)target;
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