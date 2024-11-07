using UnityEngine;

public class EnemyStateAnimator : StateMachineBehaviour
{
    [SerializeField]
    private string m_name = "";

    [SerializeField]
    private EnemyParameter.EnemyState m_enemyState;


    public void SetStateData(EnemyParameter.EnemyState enemyState)
    {
        m_enemyState = enemyState;
        m_name = m_enemyState.StateName;
        // Debug.Log(m_name);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyObserver.OnStateEnter(m_name);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        EnemyObserver.OnStateExit(m_name);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float timer = stateInfo.length * stateInfo.normalizedTime;
        EnemyObserver.OnEnterTime(m_name, timer, m_enemyState);

        // Debug.Log("On Attack Update :" + m_name.ToString());
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("On Attack Move ");
    }
}