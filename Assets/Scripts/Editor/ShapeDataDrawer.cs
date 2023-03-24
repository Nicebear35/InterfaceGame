using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    private const string Box = "box";
    private const string Columns = "Columns";
    private const string Rows = "Rows";
    private const string ClearBoard = "ClearBoard";
    private ShapeData ShapeDataInstance => (ShapeData)target;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();

        EditorGUILayout.Space();

        DrawColumnInputFields();
        EditorGUILayout.Space();

        if (ShapeDataInstance.Board != null && ShapeDataInstance.Columns > 0 && ShapeDataInstance.Rows > 0)
        {
            DrawBoardTable();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(ShapeDataInstance);
        }
    }

    private void ClearBoardButton()
    {
        if (GUILayout.Button(ClearBoard))
        {
            ShapeDataInstance.Clear();
        }
    }

    private void DrawColumnInputFields()
    {
        var columnsTemp = ShapeDataInstance.Columns;
        var rowsTemp = ShapeDataInstance.Rows;

        ShapeDataInstance.Columns = EditorGUILayout.IntField(Columns, ShapeDataInstance.Columns);
        ShapeDataInstance.Rows = EditorGUILayout.IntField(Rows, ShapeDataInstance.Rows);

        if (ShapeDataInstance.Columns != columnsTemp || ShapeDataInstance.Rows != rowsTemp && ShapeDataInstance.Columns > 0 && ShapeDataInstance.Rows > 0)
        {
            ShapeDataInstance.CreateNewBoard();
        }
    }

    private void DrawBoardTable()
    {
        var tableStyle = new GUIStyle(Box);
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;
        headerColumnStyle.alignment = TextAnchor.MiddleCenter;

        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        for (int row = 0; row < ShapeDataInstance.Rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle);

            for (int column = 0; column < ShapeDataInstance.Columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);

                var data = EditorGUILayout.Toggle(ShapeDataInstance.Board[row].Column[column], dataFieldStyle);
                ShapeDataInstance.Board[row].Column[column] = data;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
