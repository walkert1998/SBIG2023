using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FactionTable))]
public class FactionTableEditor : Editor
{
    const float opinionsLabelWidth = 50;
    const float opinionCellSize = 50;
    SerializedProperty factions;
    SerializedProperty opinions;
    int opinionsTableWidth = 10;
    Rect opinionTableRect;

    private void OnEnable()
    {
        factions = serializedObject.FindProperty("factions");
        opinions = serializedObject.FindProperty("opinions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(factions, true);
        if (EditorGUI.EndChangeCheck())
        {
            opinions.arraySize = factions.arraySize * factions.arraySize;
        }

        if (opinions.arraySize > 1)
        {
            DrawOpinions(opinions, factions);
        }
        else
        {
            EditorGUILayout.LabelField("Not enough factions to draw matrix.");
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawOpinions(SerializedProperty opinions, SerializedProperty factions)
    {
        int factionCount = factions.arraySize;

        if (Event.current.type == EventType.Layout)
        {
            opinionsTableWidth = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth);
        }

        Rect rect = GUILayoutUtility.GetRect(opinionsTableWidth, opinionsTableWidth, EditorStyles.inspectorDefaultMargins);

        if (opinionsTableWidth > 0 && Event.current.type == EventType.Repaint)
        {
            opinionTableRect = rect;
        }

        if (opinionTableRect.width > 0)
        {
            float cellWidth = Mathf.Min((opinionTableRect.width - opinionsLabelWidth) / factionCount, opinionCellSize);
            Rect opinionCell = new Rect(opinionTableRect.x + opinionsLabelWidth, opinionTableRect.y + opinionsLabelWidth, cellWidth, cellWidth / 2);
            Matrix4x4 guiMatrix4x4 = GUI.matrix;

            // Draw vertical labels
            for (int i = 1; i <= factionCount; ++i)
            {
                Rect verticalLabelRect = new Rect(opinionTableRect.x + opinionsLabelWidth + i * opinionCell.width, opinionTableRect.y, opinionsLabelWidth, opinionsLabelWidth);
                EditorGUIUtility.RotateAroundPivot(90f, new Vector2(verticalLabelRect.x, verticalLabelRect.y));
                EditorGUI.LabelField(verticalLabelRect, factions.GetArrayElementAtIndex(i - 1).FindPropertyRelative("faction").enumDisplayNames[factions.GetArrayElementAtIndex(i - 1).FindPropertyRelative("faction").enumValueIndex]);
                GUI.matrix = guiMatrix4x4;
            }

            // Draw matrix
            for (int i = 0; i < factionCount; ++i)
            {
                // Draw horizontal labels\
                EditorGUI.LabelField(new Rect(opinionTableRect.x, opinionCell.y, opinionsLabelWidth, opinionCell.height), factions.GetArrayElementAtIndex(i).FindPropertyRelative("faction").enumDisplayNames[factions.GetArrayElementAtIndex(i).FindPropertyRelative("faction").enumValueIndex]);

                for (int j = 0; j < factionCount; ++j)
                {
                    opinionCell.x = opinionTableRect.x + opinionsLabelWidth + j * cellWidth;
                    if (j > i)
                    {
                        SerializedProperty opinion = opinions.GetArrayElementAtIndex(i * factionCount + j);
                        opinion.intValue = EditorGUI.IntField(opinionCell, opinion.intValue);
                    }
                    // Remove following else if Opinion( A, B ) must be different from Opinion( B, A )
                    else
                    {
                        // Put grey box because the matrix is symmetrical
                        EditorGUI.DrawRect(opinionCell, Color.grey);
                    }
                }
                opinionCell.y += cellWidth / 2;
            }
        }
    }
}
