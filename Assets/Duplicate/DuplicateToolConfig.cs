using UnityEngine;

[CreateAssetMenu(fileName = "DuplicateToolConfig", menuName = "Tools/Duplicate Tool Config")]
public class DuplicateToolConfig : ScriptableObject
{
    [SerializeField]
    public GameObject targetObject; // 複製対象のオブジェクト
    [SerializeField]
    public int cloneCount = 1;      // 複製数
    [SerializeField]
    public bool isParent = false;
    [SerializeField]
    public StringVector3 positionOffset = new StringVector3("2", "0", "0"); // 複製ごとの位置オフセット
    [SerializeField]
    public StringVector3 rotationOffset = new StringVector3("0", "0", "0"); // 複製ごとの回転オフセット
    [SerializeField]
    public StringVector3 scaleOffset = new StringVector3("1", "1", "1");     // 複製ごとのスケールオフセット
}
