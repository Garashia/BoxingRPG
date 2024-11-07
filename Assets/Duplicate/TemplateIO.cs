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

    // UTF-8�G���R�[�f�B���O���w�肵�ăe���v���[�g��ǂݍ���
    public static TemplateCollection LoadTemplates()
    {
        if (!File.Exists(templateFilePath)) return new TemplateCollection();

        string json = File.ReadAllText(templateFilePath, Encoding.UTF8);  // UTF-8�œǂݍ���
        return JsonUtility.FromJson<TemplateCollection>(json);
    }

    // UTF-8�G���R�[�f�B���O���w�肵�ăe���v���[�g��ۑ�����
    public static void SaveTemplates(TemplateCollection templates)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(templateFilePath));
        string json = JsonUtility.ToJson(templates, true);
        File.WriteAllText(templateFilePath, json, Encoding.UTF8);  // UTF-8�ŕۑ�
    }
}
