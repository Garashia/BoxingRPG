using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[Serializable]
public class TemplateConfig
{
    public string name;
    public PositionOffset positionOffset;
    public RotationOffset rotationOffset;
}

[Serializable]
public class PositionOffset
{
    public string X, Y, Z;
}

[Serializable]
public class RotationOffset
{
    public string X, Y, Z;
}

[Serializable]
public class TemplateCollection
{
    public List<TemplateConfig> templates = new List<TemplateConfig>();
}

public static class TemplateIO
{
    private static readonly string templateFilePath = Path.Combine(Application.dataPath, "Templates", "templateConfig.json");

    // UTF-8エンコーディングを指定してテンプレートを読み込む
    public static TemplateCollection LoadTemplates()
    {
        if (!File.Exists(templateFilePath)) return new TemplateCollection();

        string json = File.ReadAllText(templateFilePath, Encoding.UTF8);  // UTF-8で読み込み
        return JsonUtility.FromJson<TemplateCollection>(json);
    }

    // UTF-8エンコーディングを指定してテンプレートを保存する
    public static void SaveTemplates(TemplateCollection templates)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(templateFilePath));
        string json = JsonUtility.ToJson(templates, true);
        File.WriteAllText(templateFilePath, json, Encoding.UTF8);  // UTF-8で保存
    }
}
