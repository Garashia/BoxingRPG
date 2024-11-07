using UnityEngine;
using static MultipleMazeTableObject;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif // UNITY_EDITOR



#if UNITY_EDITOR
[CustomEditor(typeof(MultipleMazeTableObject))]
public class MultipleMazeEditor : Editor
{
    private MultipleMazeTableObject obj;
    private bool m_isOpen1;
    private bool m_isOpen2;

    private Vector2 m_scrollPosition = Vector2.zero;


    private void OnEnable()
    {
        // 有効になった時に対象を確保しておく
        obj = target as MultipleMazeTableObject;
        m_isOpen1 = false;
        m_isOpen2 = false;
        m_scrollPosition = Vector2.zero;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var multipleArea = obj.MultipleArea;
        bool isError = false;
        int count = multipleArea.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; ++i)
            {

                var area = multipleArea[i];
                if (area.Open = EditorGUILayout.Foldout(area.Open, "list" + (i + 1).ToString()))
                {
                    area.AreaType = (MazeArea.Type)EditorGUILayout.EnumPopup("タイプ", area.AreaType);
                    switch (area.AreaType)
                    {
                        case MazeArea.Type.Area:
                        case MazeArea.Type.Maze:

                            EditorGUILayout.BeginHorizontal();
                            {
                                area.Max = EditorGUILayout.Vector2IntField("Max:", area.Max);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            {
                                area.Min = EditorGUILayout.Vector2IntField("Min:", area.Min);
                            }
                            EditorGUILayout.EndHorizontal();

                            int sa =
                                (area.AreaType == MazeArea.Type.Area) ? 1 : 4;

                            if (area.Min.x > area.Max.x || area.Min.y > area.Max.y)
                            {
                                EditorGUILayout.HelpBox("最小値が最大値を上回っています", MessageType.Error);
                                isError = true;
                            }
                            else if (((area.Max.x - area.Min.x) < sa) || (area.Max.y - area.Min.y) < sa)
                            {
                                EditorGUILayout.HelpBox("範囲が足りません", MessageType.Error);
                                isError = true;

                            }
                            else if (area.AreaType == MazeArea.Type.Maze)
                            {
                                if (((area.Max.x - area.Min.x) % 2 == 1) ||
                                    ((area.Max.y - area.Min.y) % 2 == 1))
                                {
                                    EditorGUILayout.HelpBox("Mazeの場合、minとmaxの差を偶数に" +
                                        "してください。", MessageType.Error);
                                    isError = true;

                                }
                            }
                            break;
                        case MazeArea.Type.Line:
                            EditorGUILayout.BeginHorizontal();
                            {
                                area.FirstPoint = EditorGUILayout.Vector2IntField("FirstPoint:", area.FirstPoint);
                            }
                            EditorGUILayout.EndHorizontal();
                            area.Directed = (MazeArea.Direction)EditorGUILayout.EnumPopup("向き", area.Directed);
                            area.LineLength = EditorGUILayout.IntField("長さ", area.LineLength);
                            area.LineLength = Mathf.Clamp(area.LineLength, 1, 100);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        var mazeList = obj.MazeList;

        if (!isError)
        {
            mazeList ??= new List<Vector2Int>();


            if (GUILayout.Button("Add"))
            {
                MazeArea mazeArea = new MazeArea();
                multipleArea.Add(mazeArea);
            }
            else if (multipleArea.Count > 0)
            {
                if (GUILayout.Button("Delete"))
                    multipleArea.RemoveAt(multipleArea.Count - 1);
            }
            if (GUILayout.Button("CreateMaze"))
            {
                mazeList.Clear();
                for (int i = 0; i < count; ++i)
                {
                    var area = multipleArea[i];

                    switch (area.AreaType)
                    {
                        case MazeArea.Type.Area:
                            for (int x = area.Min.x; x <= area.Max.x; ++x)
                            {
                                for (int y = area.Min.y; y <= area.Max.y; ++y)
                                {
                                    if (IsListed(x, y, mazeList)) continue;

                                    mazeList.Add(new Vector2Int(x, y));
                                }
                            }
                            break;
                        case MazeArea.Type.Maze:
                            int X = area.Max.x - area.Min.x + 3;
                            int Y = area.Max.y - area.Min.y + 3;
                            MazeFactory mazeFactory = new(ref X, ref Y);
                            var maze = mazeFactory.CreateMaze();
                            for (int y = 0; y < Y; ++y)
                            {
                                for (int x = 0; x < X; ++x)
                                {
                                    if (maze[x, y] == 1) continue;
                                    if (IsListed(x + area.Min.x, y + area.Min.y, mazeList)) continue;

                                    mazeList.Add(new(x + area.Min.x - 1, y + area.Min.y - 1));
                                }
                            }
                            break;
                        case MazeArea.Type.Line:
                            Vector2Int vector2Int = Vector2Int.zero;
                            switch (area.Directed)
                            {
                                case MazeArea.Direction.Right:
                                    vector2Int = Vector2Int.right;
                                    break;
                                case MazeArea.Direction.Left:
                                    vector2Int = Vector2Int.left;

                                    break;
                                case MazeArea.Direction.Front:
                                    vector2Int = Vector2Int.up;

                                    break;
                                case MazeArea.Direction.Back:
                                    vector2Int = Vector2Int.down;

                                    break;
                            }
                            for (int j = 0; j < area.LineLength; ++j)
                            {
                                if (IsListed(area.FirstPoint + j * vector2Int, mazeList)) continue;
                                mazeList.Add(area.FirstPoint + j * vector2Int);
                            }
                            break;
                        default:
                            break;
                    }

                }
            }
        }
        if (mazeList.Count > 0)
        {
            if (m_isOpen1 = EditorGUILayout.Foldout(m_isOpen1, "List"))
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
            if (m_isOpen2 = EditorGUILayout.Foldout(m_isOpen2, "Tile"))
            {
                Vector2Int max = Vector2Int.zero;
                Vector2Int min = Vector2Int.one * 1000;

                m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
                foreach (Vector2Int maze in mazeList)
                {
                    min.x = Mathf.Min(min.x, maze.x);
                    min.y = Mathf.Min(min.y, maze.y);
                    max.x = Mathf.Max(max.x, maze.x);
                    max.y = Mathf.Max(max.y, maze.y);

                }
                for (int y = max.y; y >= min.y; --y)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        for (int x = min.x; x <= max.x; ++x)
                        {
                            using (new EditorGUI.DisabledScope(true))
                            {
                                using (new EditorGUILayout.VerticalScope())
                                {
                                    //縦に並べたい項目
                                    EditorGUILayout.LabelField("("
                                        + x.ToString() +
                                        ", " +
                                        y.ToString() +
                                        ")");
                                    EditorGUILayout.Toggle(IsListed(x, y, mazeList));

                                }
                            }

                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                //スクロール箇所終了
                EditorGUILayout.EndScrollView();
            }
        }
        // Dirtyフラグを立てる
        EditorUtility.SetDirty(obj);

        serializedObject.ApplyModifiedProperties();
    }

    private bool IsListed(Vector2Int vector2Int, List<Vector2Int> list)
    {
        foreach (var area in list)
        {
            if (area == vector2Int)
            {
                return true;
            }
        }
        return false;

    }
    private bool IsListed(int x, int y, List<Vector2Int> list)
    {

        return IsListed(new(x, y), list);

    }

}
#endif // UNITY_EDITOR
