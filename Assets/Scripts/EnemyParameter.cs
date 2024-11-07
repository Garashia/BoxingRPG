// using System.Collections;
// using System.Collections.Generic;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif // UNITY_EDITOR

using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy")]
public class EnemyParameter : ScriptableObject
{
    public enum ID
    {
        Left = (1 << 0),
        Right = (1 << 1),
        RBlockRPunch = (1 << 2),
        RBlockLPunch = (1 << 3),
        LBlockRPunch = (1 << 4),
        LBlockLPunch = (1 << 5),
    }

    public enum BlockID
    {
        LeftBlock = (1 << 0),      // 右避け
        RightBlock = (1 << 1),     // 左避け
        Block = LeftBlock | RightBlock,          // 右or左避け
        None = 0,           // 挑発など
    }

    public static readonly ID[] ID_LIST = new ID[6] {
    ID.Left, ID.Right, ID.RBlockLPunch, ID.LBlockLPunch,
    ID.RBlockRPunch, ID.LBlockRPunch};

    [System.Serializable]
    public class HitTerm
    {
        [SerializeField, HideInInspector]
        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        [SerializeField, HideInInspector]
        private ID battleID;

        public ID BattleID
        {
            get { return battleID; }
            set { battleID = value; }
        }

        [SerializeField, HideInInspector]
        private float start, end;

        public float Start
        {
            get { return start; }
            set { start = value; }
        }

        public float End
        {
            get { return end; }
            set { end = value; }
        }
    }

    [System.Serializable]
    public struct EnemyState
    {
        [SerializeField]
        private string stateName;

        public string StateName
        {
            set { stateName = value; }
            get { return stateName; }
        }

        [SerializeField]
        private Motion enemyMotion;

        public Motion EnemyMotion
        {
            set { enemyMotion = value; }
            get { return enemyMotion; }
        }

        [SerializeField, HideInInspector]
        private TransitionConditionList transitionConditionList;

        // エディタ用、触らない事
        public TransitionConditionList ConditionList
        {
            get
            {
                return transitionConditionList;
            }
            set { transitionConditionList = value; }
        }

        [SerializeField, HideInInspector]
        private TransitionCondition transitionCondition;

        public TransitionCondition Condition
        {
            get { return transitionCondition; }
            set { transitionCondition = value; }
        }

        [SerializeField, HideInInspector]
        private bool isFold;

        public bool IsFold
        {
            get { return isFold; }
            set { isFold = value; }
        }

        [SerializeField, HideInInspector]
        private ID selectPunch;

        public ID SelectPunch
        {
            set { selectPunch = value; }
            get { return selectPunch; }
        }

        [SerializeField, HideInInspector]
        private Dictionary<ID, HitTerm> hitTerms;

        public Dictionary<ID, HitTerm> HitTerms
        {
            set { hitTerms = value; }
            get
            {
                hitTerms ??= GetHitTerms();
                return hitTerms;
            }
        }

        [SerializeField, HideInInspector]
        private HitTerm hitTerms_1, hitTerms_2, hitTerms_3, hitTerms_4, hitTerms_5, hitTerms_6;

        private Dictionary<ID, HitTerm> GetHitTerms()
        {
            return new Dictionary<ID, HitTerm>()
            {
                [ID.Left] = hitTerms_1,
                [ID.Right] = hitTerms_2,
                [ID.RBlockRPunch] = hitTerms_3,
                [ID.LBlockRPunch] = hitTerms_4,
                [ID.LBlockLPunch] = hitTerms_5,
                [ID.RBlockLPunch] = hitTerms_6,
            };
        }

        [SerializeField, HideInInspector]
        private BlockID blockID;

        public BlockID Block
        {
            get { return blockID; }
            set { blockID = value; }
        }

        [SerializeField, HideInInspector]
        private float damage;

        public float Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        [SerializeField, HideInInspector]
        private float start, end;

        public float Start
        {
            get { return start; }
            set { start = value; }
        }

        public float End
        {
            get { return end; }
            set { end = value; }
        }

        [SerializeField, HideInInspector]
        private int math;

        public int Math
        {
            get { return math; }
            set { math = value; }
        }
    }

    [SerializeField, Range(1, 10)]
    private int m_MaxHP;

    public int MaxMP
    {
        get { return m_MaxHP; }
        set { m_MaxHP = value; }
    }

    [SerializeField]
    private float m_Power;

    [SerializeField]
    private float m_Guard;

    [SerializeField]
    private Avatar m_enemyAvatar;

    public Avatar EnemyAvatar
    {
        get { return m_enemyAvatar; }
        set { m_enemyAvatar = value; }
    }

    [SerializeField]
    private Motion m_enemyIdleMotion;

    public Motion EnemyIdleMotion
    {
        get { return m_enemyIdleMotion; }
        set { m_enemyIdleMotion = value; }
    }

    [SerializeField]
    private Motion m_enemyHitMotion;

    public Motion EnemyHitMotion
    {
        get { return m_enemyHitMotion; }
        set { m_enemyHitMotion = value; }
    }

    [SerializeField]
    private Motion m_enemyDownMotion;

    public Motion EnemyDownMotion
    {
        get { return m_enemyDownMotion; }
        set { m_enemyDownMotion = value; }
    }


    [SerializeField, HideInInspector]
    private List<EnemyState> m_transitionConditionList;

    public List<EnemyState> TransitionConditionList
    {
        set { m_transitionConditionList = value; }
        get { return m_transitionConditionList; }
    }
}

[System.Serializable]
public class TransitionConditionList
{
    [SerializeField, HideInInspector]
    private TransitionCondition m_conditions;

    public TransitionCondition Condition
    {
        get
        {
            m_conditions ??= new TransitionCondition();
            return m_conditions;
        }
        set { m_conditions = value; }
    }

    [SerializeField, HideInInspector]
    private HPTransitionCondition m_HPConditions;

    public HPTransitionCondition HPCondition
    {
        get
        {
            m_HPConditions ??= new HPTransitionCondition();
            return m_HPConditions;
        }
        set { m_HPConditions = value; }
    }

    [SerializeField, HideInInspector]
    private List<TransitionCondition> m_transitionConditionsList;

    public List<TransitionCondition> TransitionConditionsList
    {
        get
        {
            if (!(m_transitionConditionsList?.Count > 0) || m_transitionConditionsList != GetTransitionConditionsList())
                m_transitionConditionsList = GetTransitionConditionsList();
            return m_transitionConditionsList;
        }
    }

    [SerializeField, HideInInspector]
    private int m_explanatoryIndex = 0;

    public int ExplanatoryIndex
    {
        set { m_explanatoryIndex = value; }
        get { return m_explanatoryIndex; }
    }

    [SerializeField, HideInInspector]
    private List<string> m_explanatoryNoteList;

    public List<string> ExplanatoryNoteList
    {
        get
        {
            if (!(m_explanatoryNoteList?.Count > 0) || m_explanatoryNoteList != GetExplanatoryNoteList())
                m_explanatoryNoteList = GetExplanatoryNoteList();
            return m_explanatoryNoteList;
        }
    }

    // 初期化用
    private List<TransitionCondition> GetTransitionConditionsList()
    {
        List<TransitionCondition> conditions = new List<TransitionCondition> { Condition, HPCondition };
        return conditions;
    }

    private List<string> GetExplanatoryNoteList()
    {
        List<string> keyValuePairs = new List<string>();
        List<TransitionCondition> transitions = TransitionConditionsList;
        foreach (TransitionCondition transition in transitions)
        {
            keyValuePairs.Add(transition.ExplanatoryNote());
        }
        return keyValuePairs;
    }
}

[System.Serializable]
public class TransitionCondition
{
    public virtual string ExplanatoryNote()
    {
        return "リストから条件を指定:";
    }

    private EnemyController owner;

    public EnemyController Owner
    {
        set { owner = value; }
        get { return owner; }
    }

    public virtual bool IsTransitionCondition()
    {
        return true;
    }

    public delegate (Rect, int, bool, bool) editor();

    public virtual void Editor(editor editorGUI)
    {
        // なし
    }

    public virtual int Height(int index = 0)
    {
        return 25 * (9 + index);
    }
}

[System.Serializable]
public class HPTransitionCondition : TransitionCondition
{
    [SerializeField, HideInInspector]
    private int m_transitionHP;

    public override string ExplanatoryNote()
    {
        return "遷移条件：HP";
    }

    public override bool IsTransitionCondition()
    {
        return m_transitionHP >= Owner.HP;
    }

#if UNITY_EDITOR

    public override void Editor(editor editor)
    {
        (Rect rect, int index, bool isActive, bool isFocused) = editor();
        m_transitionHP = EditorGUI.IntField(rect, "条件(hp)", m_transitionHP);
    }

    public override int Height(int index = 0)
    {
        return base.Height(index) + 25;
    }

#endif
}