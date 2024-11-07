using System.Collections.Generic;
using UnityEngine;



#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif // UNITY_EDITOR



#if UNITY_EDITOR
[CustomEditor(typeof(GridObject))]
public class GridObjectEditor : Editor
{
    private GridObject obj;

    private void OnEnable()
    {
        // 有効になった時に対象を確保しておく
        obj = target as GridObject;

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Transform transform = obj.transform;
        Vector2Int point = obj.GridPoint;
        GridManager gridManager = obj.GetGridManager;
        if (gridManager == null) return;
        List<GridObject> grids = gridManager.Grids;

        obj.GridRender = EditorGUILayout.Toggle("DebugString", obj.GridRender);


        EditorGUILayout.BeginHorizontal();
        {
            transform.localPosition = EditorGUILayout.Vector3Field("position:", transform.localPosition);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        {
            using (new EditorGUI.DisabledScope(true))
            {
                obj.GridPoint = EditorGUILayout.Vector2IntField("GridPoint:", obj.GridPoint);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            transform.localEulerAngles = EditorGUILayout.Vector3Field("rotation:", transform.localEulerAngles);
        }
        EditorGUILayout.EndHorizontal();
        Vector2Int rightPoint = point + Vector2Int.right;
        EditorGUILayout.BeginHorizontal();
        using (new EditorGUI.DisabledScope(gridManager.IsAdjacent(point, Vector2Int.right)))
        {
            if (GUILayout.Button("Right"))
            {
                AddObject
                    (
                    grids,
                    point,
                    Vector2Int.right,
                    Vector3.right,
                    transform,
                    gridManager
                    );
            }
        }
        using (new EditorGUI.DisabledScope(gridManager.IsAdjacent(point, Vector2Int.left)))
        {
            if (GUILayout.Button("Left"))
            {
                AddObject
                    (
                    grids,
                    point,
                    Vector2Int.left,
                    Vector3.left,
                    transform,
                    gridManager
                    );
            }
        }
        using (new EditorGUI.DisabledScope(gridManager.IsAdjacent(point, Vector2Int.up)))
        {
            if (GUILayout.Button("Front"))
            {
                AddObject
                    (
                    grids,
                    point,
                    Vector2Int.up,
                    Vector3.forward,
                    transform,
                    gridManager
                    );
            }
        }
        using (new EditorGUI.DisabledScope(gridManager.IsAdjacent(point, Vector2Int.down)))
        {
            if (GUILayout.Button("Back"))
            {
                AddObject
                    (
                    grids,
                    point,
                    Vector2Int.down,
                    Vector3.back,
                    transform,
                    gridManager
                    );
            }
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Destroy"))
        {
            gridManager.DestroyedObject(obj.gameObject);
        }
        else if (GUILayout.Button("GridDown"))
        {
            obj.GridDown();
        }
        // Dirtyフラグを立てる
        if (obj != null)
        {
            EditorUtility.SetDirty(obj);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void AddObject
(
    List<GridObject> list,
    Vector2Int point,
    Vector2Int direct2,
    Vector3 direct3,
    Transform transform,
    GridManager gridManager)
    {
        GameObject games = new GameObject();
        Vector3 position = transform.localPosition;
        Vector3 moving = new Vector3(
            direct3.x * transform.localScale.x,
            0.0f,
            direct3.z * transform.localScale.z
            );
        position += transform.rotation * (moving * 2.0f);

        games.transform.localPosition = position;
        games.transform.rotation = transform.rotation;
        GridObject newGrid = games.AddComponent<GridObject>();
        list.Add(newGrid);
        games.transform.localScale = transform.localScale;

        games.transform.parent = gridManager.transform;
        newGrid.GridPoint = point + direct2;
        games.name = newGrid.GridPoint.ToString();

        newGrid.SetGridManager = gridManager;
    }

}
#endif