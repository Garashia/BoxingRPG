using UnityEngine;
using static EnemyParameter;
using static PlayerStateAnimator;

[DefaultExecutionOrder(99)]
public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController m_playerController = null;
    public PlayerController SceneInPlayerController
    {
        set { m_playerController = value; }
        get { return m_playerController; }
    }

    [SerializeField]
    private EnemyController m_enemyController = null;
    public EnemyController EnemyController
    {
        set { m_enemyController = value; }
        get { return m_enemyController; }
    }

    private bool m_player = false;
    private bool m_enemy = false;
    private uint m_enemyBlock;
    private float m_enemyAttackDamage;

    private PlayerState m_playerState;
    private PlayerState m_playerMove;

    public uint EnemyBlock
    {
        get { return m_enemyBlock; }
    }

    private uint m_enemyAttack;

    public uint EnemyAttack
    {
        get { return m_enemyAttack; }
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyObserver.SetBattleManager(this);
        PlayerObserver.SetBattleManager(this);
        m_enemyAttackDamage = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_player)
        {
            if ((m_playerMove & PlayerState.P) != PlayerState.None)
                if (!m_enemy || Check())
                {
                    m_enemyController.Hit(m_playerController.Power);
                    m_playerMove = PlayerState.None;
                    m_player = false;
                }
        }
        if (m_enemy)
        {
            //if ((m_playerMove & PlayerState.B) == PlayerState.None)
            //{
            //    m_playerController.Hit(m_enemyAttackDamage);
            //    m_enemy = false;
            //    m_enemyBlock = 0;
            //}
            //else
            if ((m_enemyBlock & (uint)BlockID.LeftBlock) == (uint)BlockID.LeftBlock
                && !((m_playerMove & PlayerState.LeftB) == PlayerState.LeftB))
            {
                m_playerController.Hit(m_enemyAttackDamage);
                m_enemyBlock = 0;
            }
            else if ((m_enemyBlock & (uint)BlockID.RightBlock) == (uint)BlockID.RightBlock
                && !((m_playerMove & PlayerState.RightB) == PlayerState.RightB))
            {
                m_playerController.Hit(m_enemyAttackDamage);
                m_enemyBlock = 0;
            }

        }

        // Debug.Log(((int)(uint)m_playerMove).ToBinaryString());
        m_playerMove = PlayerState.None;

        m_enemyAttackDamage = 0.0f;
    }

    public void OnStateEnterEnemy(string stateName)
    {
        m_enemy = true;
    }


    public void OnStateExitEnemy(string dateName)
    {
        m_enemy = false;
    }

    public void OnStateEnterPlayer(PlayerState stateName)
    {
        m_playerState |= stateName;

        m_player = true;
    }

    public void OnStateEnterPlayer(PlayerState stateName, ref PlayerState state)
    {
        m_playerState |= stateName;
        state = m_playerState;
        m_player = true;
    }


    public void OnStateExitPlayer(PlayerState dateName)
    {
        m_playerState &= ~dateName;

        m_player = false;
    }

    public void OnEnterTimeEnemy(string dateName, float time, EnemyState enemyState)
    {
        m_enemyBlock = 0;
        m_enemyAttack = 0;
        m_enemyAttackDamage = 0.0f;
        if ((enemyState.Start <= time) && (enemyState.End > time) && (enemyState.Block != BlockID.None)
            && m_enemy)
        {
            m_enemyBlock = (uint)enemyState.Block;
        }
        // Debug.Log(time);
        foreach (var (id, name) in enemyState.HitTerms)
        {
            if ((id & enemyState.SelectPunch) != id) continue;
            if (name.Start <= time && name.End > time)
                m_enemyAttack |= (uint)id;
        }
        // Debug.Log(((int)EnemyBlock).ToBinaryString() + ", " + ((int)EnemyAttack).ToBinaryString());
        m_enemyAttackDamage = enemyState.Damage;
    }

    public void OnEnterTimePlayer(PlayerState dateName, float time)
    {
        if (m_player)
            m_playerMove |= dateName;
    }

    private bool Check()
    {
        if ((m_playerMove & PlayerState.LeftP) == PlayerState.LeftP
    && (m_enemyAttack & (uint)ID.Left) == (uint)ID.Left)
            return true;
        else if ((m_playerMove & PlayerState.RightP) == PlayerState.RightP
    && (m_enemyAttack & (uint)ID.Right) == (uint)ID.Right)
            return true;
        else if ((m_playerMove & PlayerState.RBRP) == PlayerState.RBRP
    && (m_enemyAttack & (uint)ID.RBlockRPunch) == (uint)ID.RBlockRPunch)
            return true;
        else if ((m_playerMove & PlayerState.RBLP) == PlayerState.RBLP
    && (m_enemyAttack & (uint)ID.RBlockLPunch) == (uint)ID.RBlockLPunch)
            return true;
        else if ((m_playerMove & PlayerState.LBLP) == PlayerState.LBLP
    && (m_enemyAttack & (uint)ID.LBlockLPunch) == (uint)ID.LBlockLPunch)
            return true;
        else if ((m_playerMove & PlayerState.LBRP) == PlayerState.LBRP
    && (m_enemyAttack & (uint)ID.LBlockRPunch) == (uint)ID.LBlockRPunch)
            return true;


        return false;
    }
}
