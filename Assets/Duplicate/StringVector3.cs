using System;
using UnityEngine;

[Serializable]
public struct StringVector3
{
    [SerializeField] private string x;
    [SerializeField] private string y;
    [SerializeField] private string z;

    // 各プロパティのアクセサ
    public string X
    {
        get => x;
        set => x = value;
    }

    public string Y
    {
        get => y;
        set => y = value;
    }

    public string Z
    {
        get => z;
        set => z = value;
    }

    // コンストラクタ
    public StringVector3(string x, string y, string z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override string ToString()
    {
        return $"{x},{y},{z}";
    }

    public static StringVector3 FromString(string data)
    {
        var values = data.Split(',');
        if (values.Length != 3) throw new ArgumentException("Invalid format for StringVector3");
        return new StringVector3(values[0], values[1], values[2]);
    }
}
