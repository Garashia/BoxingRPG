public class SendDamage
{
    static private SendDamage m_sendObject;
    public static SendDamage SendObject
    {
        get
        {
            m_sendObject ??= new SendDamage(); return m_sendObject;
        }
    }
    private EnemyController m_enemyControl;
    public EnemyController EnemyControl
    {
        set { m_enemyControl = value; }
        get { return m_enemyControl; }
    }




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
