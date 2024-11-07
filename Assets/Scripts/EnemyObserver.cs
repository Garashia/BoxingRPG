public class EnemyObserver
{
    private static BattleManager m_battleManager = null;

    public static void SetBattleManager(BattleManager battleManager)
    {
        m_battleManager = battleManager;
    }

    public static void OnStateEnter(string stateName)
    {
        if (m_battleManager != null)
        {
            m_battleManager.OnStateEnterEnemy(stateName);
            m_battleManager.EnemyController.OnStateEnter(stateName);
        }
    }

    public static void OnStateExit(string stateName)
    {
        if (m_battleManager != null)
        {
            m_battleManager.OnStateExitEnemy(stateName);
            m_battleManager.EnemyController.OnStateExit(stateName);

        }
    }

    public static void OnEnterTime(string stateName, float time, EnemyParameter.EnemyState enemyState)
    {
        if (m_battleManager != null)
        {
            m_battleManager.OnEnterTimeEnemy(stateName, time, enemyState);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}