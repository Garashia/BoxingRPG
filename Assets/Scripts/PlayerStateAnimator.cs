using UnityEngine;

public class PlayerStateAnimator : StateMachineBehaviour
{
    public enum PlayerState
    {
        None = 0,
        RightP = (1 << 0),
        LeftP = (1 << 1),

        RightB = (1 << 2),
        LeftB = (1 << 3),
        [InspectorName("")]
        B = LeftB | RightB,
        [InspectorName("")]
        RBRP = (1 << 4),
        [InspectorName("")]
        LBRP = (1 << 5),
        [InspectorName("")]
        RBLP = (1 << 6),
        [InspectorName("")]
        LBLP = (1 << 7),
        [InspectorName("")]
        P = RightP | LeftP | RBRP | LBRP | RBLP | LBLP,
    }

    [SerializeField]
    private PlayerState m_name;
    public PlayerState Name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    [SerializeField]
    private float m_first, m_end;
    public float First
    {
        set { m_first = value; }
        get { return m_first; }
    }
    public float End
    {
        set { m_end = value; }
        get { return m_end; }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerObserver.OnStateEnter(m_name);
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float timer = stateInfo.length * stateInfo.normalizedTime;
        if (timer > m_first && timer < m_end)
            PlayerObserver.OnEnterTime(m_name, timer);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerObserver.OnStateExit(m_name);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
