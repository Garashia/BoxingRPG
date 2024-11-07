using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DuplicateToolConfig))]
public class DuplicateToolEditor : Editor
{
    [SerializeField] private bool isRender = false;
    [SerializeField] private bool drawDebugLines = false;

    private DuplicateToolConfig config;
    private string errorMessage = string.Empty;
    private List<GameObject> previewObjects = new List<GameObject>();
    private TemplateCollection templateCollection;

    private void OnEnable()
    {
        config = (DuplicateToolConfig)target;
        templateCollection = TemplateIO.LoadTemplates();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("数式構文について", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "数式テンプレートの使用例:\n" +
            "- 定数: PI, C, #（インデックス）\n" +
            "- 演算子: +, -, *, / \n" +
            "- 三項演算子, 関数: sin, cos, sqrt, pow, floor, ceil, round, R(min, max)\n" +
            "- 括弧で優先度指定\n",
            MessageType.Info);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }

        drawDebugLines = EditorGUILayout.Toggle("Draw Debug Lines", drawDebugLines);

        if (GUILayout.Button("Toggle Preview Clone Positions"))
        {
            if (isRender)
            {
                ClearPreview();
            }
            else
            {
                StartPreview();
            }
            isRender = !isRender;
        }

        if (GUILayout.Button("Execute Cloning"))
        {
            ExecuteCloning();
        }

        // テンプレート一覧をボタンで表示
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("配置テンプレート", EditorStyles.boldLabel);

        foreach (var template in templateCollection.templates)
        {
            if (GUILayout.Button(template.name))
            {
                ApplyTemplate(template);
            }
        }

        EditorGUILayout.Space();
    }

    private void ApplyTemplate(TemplateConfig template)
    {
        config.positionOffset.X = template.positionOffset.X;
        config.positionOffset.Y = template.positionOffset.Y;
        config.positionOffset.Z = template.positionOffset.Z;
        config.rotationOffset.X = template.rotationOffset.X;
        config.rotationOffset.Y = template.rotationOffset.Y;
        config.rotationOffset.Z = template.rotationOffset.Z;
    }

    private void StartPreview()
    {
        ClearPreview();

        try
        {
            for (int i = 1; i <= config.cloneCount; i++)
            {
                Vector3 position = new Vector3(
                    TransformCalculator.EvaluateSingleExpression(config.positionOffset.X, i, config.cloneCount),
                    TransformCalculator.EvaluateSingleExpression(config.positionOffset.Y, i, config.cloneCount),
                    TransformCalculator.EvaluateSingleExpression(config.positionOffset.Z, i, config.cloneCount)
                );
                Quaternion rotation = Quaternion.Euler(
                    TransformCalculator.EvaluateSingleExpression(config.rotationOffset.X, i, config.cloneCount),
                    TransformCalculator.EvaluateSingleExpression(config.rotationOffset.Y, i, config.cloneCount),
                    TransformCalculator.EvaluateSingleExpression(config.rotationOffset.Z, i, config.cloneCount)
                );

                Vector3 scale = new Vector3(
                    TransformCalculator.EvaluateSingleExpression(config.scaleOffset.X, i, config.cloneCount),
                    TransformCalculator.EvaluateSingleExpression(config.scaleOffset.Y, i, config.cloneCount),
                    TransformCalculator.EvaluateSingleExpression(config.scaleOffset.Z, i, config.cloneCount)
                );

                GameObject previewObject = Instantiate(config.targetObject, position, rotation);
                previewObject.transform.localScale = scale;
                previewObject.name = config.targetObject.name + "_Preview_" + i;
                previewObject.hideFlags = HideFlags.HideAndDontSave;
                previewObjects.Add(previewObject);

                if (drawDebugLines && i > 1)
                {
                    Vector3 previousPosition = previewObjects[i - 2].transform.position;
                    Debug.DrawLine(previousPosition, position, Color.green, 0.1f);
                }
            }
            errorMessage = string.Empty;
        }
        catch (ArgumentException ex)
        {
            errorMessage = ex.Message;
        }
    }

    private void ClearPreview()
    {
        foreach (GameObject previewObject in previewObjects)
        {
            if (previewObject != null)
            {
                DestroyImmediate(previewObject);
            }
        }
        previewObjects.Clear();
    }

    private void ExecuteCloning()
    {
        ClearPreview();

        List<GameObject> spawns = new List<GameObject>();
        for (int i = 1; i <= config.cloneCount; i++)
        {
            Vector3 position = new Vector3(
                TransformCalculator.EvaluateSingleExpression(config.positionOffset.X, i, config.cloneCount),
                TransformCalculator.EvaluateSingleExpression(config.positionOffset.Y, i, config.cloneCount),
                TransformCalculator.EvaluateSingleExpression(config.positionOffset.Z, i, config.cloneCount)
            );
            Quaternion rotation = Quaternion.Euler(
                TransformCalculator.EvaluateSingleExpression(config.rotationOffset.X, i, config.cloneCount),
                TransformCalculator.EvaluateSingleExpression(config.rotationOffset.Y, i, config.cloneCount),
                TransformCalculator.EvaluateSingleExpression(config.rotationOffset.Z, i, config.cloneCount)
            );

            Vector3 scale = new Vector3(
                TransformCalculator.EvaluateSingleExpression(config.scaleOffset.X, i, config.cloneCount),
                TransformCalculator.EvaluateSingleExpression(config.scaleOffset.Y, i, config.cloneCount),
                TransformCalculator.EvaluateSingleExpression(config.scaleOffset.Z, i, config.cloneCount)
            );

            GameObject clone = Instantiate(config.targetObject, position, rotation);
            clone.transform.localScale = scale;
            clone.name = config.targetObject.name + "_Clone_" + i;
            spawns.Add(clone);
        }

        if (config.isParent)
        {
            GameObject parent = new GameObject("SpawnObject");
            foreach (var spawn in spawns)
            {
                spawn.transform.parent = parent.transform;
            }
        }

        Debug.Log($"{config.cloneCount} clones created from {config.targetObject.name}.");
    }

    private void OnDisable()
    {
        ClearPreview();
    }
}
