using UnityEngine;

public class Rolling : MonoBehaviour
{
    private Transform m_transform;
    private Quaternion m_rotation;
    // Start is called before the first frame update
    void Start()
    {
        m_transform = transform;
        m_rotation = Quaternion.Euler(new(0.0f, 1.0f, 0.0f));
    }

    // Update is called once per frame
    void Update()
    {
        m_transform.rotation *= m_rotation;
    }
}
