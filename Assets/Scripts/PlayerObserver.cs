using static PlayerStateAnimator;

public class PlayerObserver
{
    private static BattleManager m_battleManager = null;

    public static void SetBattleManager(BattleManager battleManager)
    {
        m_battleManager = battleManager;
    }

    public static void OnStateEnter(PlayerState stateName)
    {
        if (m_battleManager != null)
        {
            m_battleManager.OnStateEnterPlayer(stateName);
        }
    }


    public static void OnStateExit(PlayerState stateName)
    {
        if (m_battleManager != null)
        {
            m_battleManager.OnStateExitPlayer(stateName);
        }
    }

    public static void OnEnterTime(PlayerState stateName, float time)
    {
        if (m_battleManager != null)
        {
            m_battleManager.OnEnterTimePlayer(stateName, time);
        }
    }

    public static void OnStateEnter(PlayerState stateName, ref PlayerState state)
    {
        if (m_battleManager != null)
        {
            m_battleManager.OnStateEnterPlayer(stateName, ref state);
        }
    }


}

