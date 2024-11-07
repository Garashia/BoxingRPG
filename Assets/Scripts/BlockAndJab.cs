using UnityEngine;
using static PlayerStateAnimator;

public static class Function
{
    private const int PLAYER_STATES_CHOICE_INDEX = 4;

    static readonly PlayerState[] PLAYER_STATES_CHOICE =
        new PlayerState[PLAYER_STATES_CHOICE_INDEX]
        {
            /*PlayerState.RightB | PlayerState.RightP*/PlayerState.RBRP,
            /*PlayerState.RightB | PlayerState.LeftP*/PlayerState.RBLP,
            /*PlayerState.LeftP | PlayerState.RightP*/PlayerState.LBRP,
            /*PlayerState.LeftP | PlayerState.LeftP*/PlayerState.LBLP,
        };
    static readonly PlayerState[] PLAYER_STATES_CHOICE_CHECK =
    new PlayerState[PLAYER_STATES_CHOICE_INDEX]
    {
            PlayerState.RightB | PlayerState.RightP,
            PlayerState.RightB | PlayerState.LeftP,
            PlayerState.LeftB | PlayerState.RightP,
            PlayerState.LeftB | PlayerState.LeftP,
    };

    public static PlayerState PlayerStates(this PlayerState self, PlayerState second)
    {
        PlayerState nu = self | second;
        for (int i = 0; i < PLAYER_STATES_CHOICE_INDEX; ++i)
        {
            if ((nu & PLAYER_STATES_CHOICE_CHECK[i]) == PLAYER_STATES_CHOICE_CHECK[i])
            {
                // Debug.Log(PLAYER_STATES_CHOICE[i]);
                return PLAYER_STATES_CHOICE[i];
            }
        }
        return PlayerState.None;
    }

}


public class BlockAndJab : PlayerStateAnimator
{
    private const int PLAYER_STATES_INDEX = 2;

    static private readonly PlayerState[] PLAYER_STATES =
        new PlayerState[PLAYER_STATES_INDEX]
    {
        PlayerState.RightB,
        PlayerState.LeftB,
    };



    private PlayerState m_state;
    private PlayerState m_secondPlayerState;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_secondPlayerState = PlayerState.None;
        PlayerObserver.OnStateEnter(Name, ref m_state);
        for (int i = 0; i < PLAYER_STATES_INDEX; ++i)
        {
            if ((PLAYER_STATES[i] & m_state) == PLAYER_STATES[i])
            {
                m_secondPlayerState = PLAYER_STATES[i].PlayerStates(Name);
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float timer = stateInfo.length * stateInfo.normalizedTime;
        if (timer > First && timer < End)
            PlayerObserver.OnEnterTime(m_secondPlayerState, timer);

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerObserver.OnStateExit(Name);
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
