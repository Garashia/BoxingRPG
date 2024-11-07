#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
using UnityEditorInternal;
using UnityEngine;
#endif // UNITY_EDITOR

#if UNITY_EDITOR

[CustomEditor(typeof(EnemyParameter))]
public class EnemyParameterEditor : Editor
{
    private EnemyParameter obj;
    private ReorderableList _reorderableList;


    private void OnEnable()
    {
        // 有効になった時に対象を確保しておく
        obj = target as EnemyParameter;
    }

    [System.Obsolete]
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();


        var conditionList = obj.TransitionConditionList;
        // ReorderableListを作る
        if (_reorderableList == null)
        {
            _reorderableList = new ReorderableList(conditionList, typeof(EnemyParameter.EnemyState));
            // 並び替え可能か
            _reorderableList.draggable = true;

            // タイトル描画時のコールバック
            // 上書きしてEditorGUIを使えばタイトル部分を自由にレイアウトできる
            _reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "推移条件");

            // 要素の描画時のコールバック
            // 上書きしてEditorGUIを使えば自由にレイアウトできる
            _reorderableList.drawElementCallback += DrawElement;
            void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                var height = EditorGUIUtility.singleLineHeight + 5;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 5;
                rect.x += 10;
                var condition = obj.TransitionConditionList[index];
                condition.IsFold = EditorGUI.Foldout(rect, condition.IsFold, new GUIContent("要素" + index.ToString()));

                if (condition.IsFold)
                {
                    rect.x -= 10;

                    rect.y += height;

                    condition.StateName = EditorGUI.TextField(rect, "遷移名", condition.StateName);
                    rect.y += height;
                    condition.EnemyMotion = EditorGUI.ObjectField(rect, "Motion", condition.EnemyMotion, typeof(Motion), true) as Motion;
                    rect.y += height;
                    EditorGUI.BeginChangeCheck();
                    condition.ConditionList.ExplanatoryIndex = EditorGUI.Popup(rect, condition.ConditionList.ExplanatoryIndex, condition.ConditionList.ExplanatoryNoteList.ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {
                        condition.Condition = condition.ConditionList.TransitionConditionsList[condition.ConditionList.ExplanatoryIndex];
                        // Debug.Log(condition.Condition);
                    }
                    if (condition.Condition != null)
                    {
                        condition.Condition.Editor(() =>
                        {
                            rect.y += height;
                            return (rect, index, isActive, isFocused);
                        });
                    }
                    rect.y += height;
                    EditorGUI.BeginChangeCheck();
                    condition.SelectPunch = (EnemyParameter.ID)EditorGUI.EnumMaskField(rect, "Select", condition.SelectPunch);
                    // condition.HitTerms ??= new();
                    condition.Math = 1;
                    rect.y += height;
                    EditorGUI.LabelField(rect, "反撃可能フレーム");
                    rect.y += height;
                    foreach (var i in EnemyParameter.ID_LIST)
                    {
                        if ((i & condition.SelectPunch) != i) continue;
                        condition.HitTerms.TryAdd(i, new());
                        var o = condition.HitTerms[i];
                        {
                            EditorGUI.LabelField(rect, i.ToString());
                            rect.y += height;

                            //横に並べたい項目
                            var FloatRect = rect;
                            FloatRect.width *= 0.4f;
                            o.Start = EditorGUI.FloatField(FloatRect, o.Start);
                            var x = FloatRect.x + FloatRect.width * 1.25f;
                            FloatRect.x = Mathf.Lerp(FloatRect.x + FloatRect.width * 0.9f, x, 0.5f);
                            EditorGUI.LabelField(FloatRect, "～");

                            FloatRect.x = x;
                            o.End = EditorGUI.FloatField(FloatRect, o.End);
                            if (o.End < o.Start)
                                o.End = o.Start;
                        }
                        condition.HitTerms[i] = o;
                        rect.y += height;
                        condition.Math += 2;
                    }

                    condition.Block = (EnemyParameter.BlockID)EditorGUI.EnumPopup(rect, condition.Block);
                    if (condition.Block != EnemyParameter.BlockID.None)
                    {
                        rect.y += height;
                        condition.Damage = EditorGUI.FloatField(rect, "ダメージ", condition.Damage);
                        rect.y += height;

                        EditorGUI.LabelField(rect, "攻撃フレーム");
                        rect.y += height;

                        //横に並べたい項目
                        var FloatRect = rect;
                        FloatRect.width *= 0.4f;
                        condition.Start = EditorGUI.FloatField(FloatRect, condition.Start);
                        var x = FloatRect.x + FloatRect.width * 1.25f;
                        FloatRect.x = Mathf.Lerp(FloatRect.x + FloatRect.width * 0.9f, x, 0.5f);
                        EditorGUI.LabelField(FloatRect, "～");

                        FloatRect.x = x;
                        condition.End = EditorGUI.FloatField(FloatRect, condition.End);
                        if (condition.End < condition.Start)
                            condition.End = condition.Start;

                    }

                }

                obj.TransitionConditionList[index] = condition;
            };

            // プロパティの高さを指定
            _reorderableList.elementHeightCallback = index =>
            {
                var condition = obj.TransitionConditionList[index];
                if (!condition.IsFold)
                    return 25;

                if (condition.Condition != null)
                    return condition.Condition.Height(condition.Math);
                return 9 * 25;
            };

            // +ボタンが押された時のコールバック
            _reorderableList.onAddCallback += Add;
            void Add(ReorderableList list)
            {
                Debug.Log("+ clicked.");
                conditionList.Add(new());//現在の要素の個数を文字列で追加する
            }
            //-ボタンを押した時に要素を削除出来るか判定する処理を設定
            _reorderableList.onCanRemoveCallback += CanRemove;
            bool CanRemove(ReorderableList list)
            {
                return conditionList.Count >= 1;//1個以上の時しか削除できないように
            }

        }
        // 描画
        _reorderableList.DoLayoutList();

        // Dirtyフラグを立てる
        EditorUtility.SetDirty(obj);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif // UNITY_EDITOR
