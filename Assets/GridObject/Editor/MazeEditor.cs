using UnityEngine;
using System;
using System.Collections.Generic;




#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif // UNITY_EDITOR



#if UNITY_EDITOR
[CustomEditor(typeof(MazeTableObject))]

public class MazeEditor : Editor
{
    private MazeTableObject obj;
    private Vector2Int m_size;
    private bool m_isOpen;
    private void OnEnable()
    {
        // 有効になった時に対象を確保しておく
        obj = target as MazeTableObject;
        m_isOpen = false;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        m_size = new(obj.X, obj.Y);

        EditorGUILayout.BeginHorizontal();
        {
            m_size = EditorGUILayout.Vector2IntField("Size:", m_size);
        }
        EditorGUILayout.EndHorizontal();

        m_size.x = Math.Clamp(m_size.x, 5, 500);
        m_size.y = Mathf.Clamp(m_size.y, 5, 500);
        obj.X = m_size.x;
        obj.Y = m_size.y;
        var mazeList = obj.MazeList;
        mazeList ??= new List<Vector2Int>();

        if (GUILayout.Button("CreateMaze"))
        {
            int X = m_size.x;
            int Y = m_size.y;
            MazeFactory mazeFactory = new(ref X, ref Y);
            var maze = mazeFactory.CreateMaze();
            mazeList.Clear();
            mazeList.Capacity = X * Y;
            for (int y = 0; y < Y; ++y)
            {
                for (int x = 0; x < X; ++x)
                {
                    if (maze[x, y] == 1) continue;
                    mazeList.Add(new(x, y));
                }
            }
        }
        if (mazeList.Count > 0)
            if (m_isOpen = EditorGUILayout.Foldout(m_isOpen, "List"))
            {
                foreach (Vector2Int maze in mazeList)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        using (new EditorGUI.DisabledScope(true))
                        {
                            EditorGUILayout.Vector2IntField("GridPoint:", maze);
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                }
            }
        // Dirtyフラグを立てる
        EditorUtility.SetDirty(obj);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif // UNITY_EDITOR