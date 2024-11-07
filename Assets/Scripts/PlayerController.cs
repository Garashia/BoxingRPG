using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float m_power = 5.0f;
    public float Power
    {
        set { m_power = value; }
        get { return m_power; }
    }
    private Animator m_animator;

    public void Hit(float damage)
    {
        m_animator.SetTriggerOneFrame(this, "Hit");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
