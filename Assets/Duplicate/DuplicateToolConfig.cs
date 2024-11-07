using UnityEngine;

[CreateAssetMenu(fileName = "DuplicateToolConfig", menuName = "Tools/Duplicate Tool Config")]
public class DuplicateToolConfig : ScriptableObject
{
    [SerializeField]
    public GameObject targetObject; // �����Ώۂ̃I�u�W�F�N�g
    [SerializeField]
    public int cloneCount = 1;      // ������
    [SerializeField]
    public bool isParent = false;
    [SerializeField]
    public StringVector3 positionOffset = new StringVector3("2", "0", "0"); // �������Ƃ̈ʒu�I�t�Z�b�g
    [SerializeField]
    public StringVector3 rotationOffset = new StringVector3("0", "0", "0"); // �������Ƃ̉�]�I�t�Z�b�g
    [SerializeField]
    public StringVector3 scaleOffset = new StringVector3("1", "1", "1");     // �������Ƃ̃X�P�[���I�t�Z�b�g
}
