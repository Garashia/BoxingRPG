using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    struct AnimatorTrigger
    {
        public string name;
        public float time;
        public bool flag;
    }

    private Animator m_animator;
    AnimatorTrigger m_animatorTrigger;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_animatorTrigger.name = "";
        m_animatorTrigger.time = 0.0f;
        m_animatorTrigger.flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_animatorTrigger.time >= 0.3f)
        {
            SetTrigger("");
        }
        if (m_animatorTrigger.flag)
            m_animatorTrigger.time += Time.deltaTime;
    }

    public void SetTrigger(string triggerName)
    {
        if (m_animatorTrigger.name == "")
        {
            m_animatorTrigger.flag = true;
        }
        else
            m_animator.ResetTrigger(m_animatorTrigger.name);

        m_animator.SetTrigger(triggerName);
        m_animatorTrigger.name = triggerName;
        m_animatorTrigger.time = 0.0f;
    }

}
