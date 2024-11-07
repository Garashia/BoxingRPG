using System.Collections.Generic;


// using UnityEditor;
using UnityEngine;
// using UnityEditor;
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_gridScale;
    public Vector2 GridScale
    {
        get { return m_gridScale; }
        set { m_gridScale = value; }
    }

    [SerializeField]
    private List<GridObject> m_grids;

    public List<GridObject> Grids
    {
        get { return m_grids; }
        set { m_grids = value; }
    }

    [SerializeField]
    private MazeTable m_mazeTableObject;
    public MazeTable MazeObject
    {
        get { return m_mazeTableObject; }
        set { m_mazeTableObject = value; }
    }

    [SerializeField]
    private GameObject m_floor;
    public GameObject Floor
    {
        get { return m_floor; }
        set { m_floor = value; }
    }
    [SerializeField]
    private GameObject m_wall;
    public GameObject Wall
    {
        get { return m_wall; }
        set { m_wall = value; }
    }
    [SerializeField]
    private GameObject m_corner;
    public GameObject Corner
    {
        set { m_corner = value; }
        get { return m_corner; }
    }

    private readonly Vector2Int[] ORIENTATION = new[]
{
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0)
    };
    private Vector2Int[,] ORIENTATION2 = new Vector2Int[4, 3]
    {
        {
           new Vector2Int(0, 1),
           new Vector2Int(1, 1),
           new Vector2Int(1, 0)

        },
        {
           new Vector2Int(0, 1),
           new Vector2Int(-1, 1),
           new Vector2Int(-1, 0)

        },
        {
           new Vector2Int(0, -1),
           new Vector2Int(1, -1),
           new Vector2Int(1, 0)

        },
        {
           new Vector2Int(0, -1),
           new Vector2Int(-1, -1),
           new Vector2Int(-1, 0)

        },

    };

    static private readonly uint Corner_Up_Right = (1 << 0);
    static private readonly uint Corner_Up_Left = (1 << 1);
    static private readonly uint Corner_Down_Right = (1 << 2);
    static private readonly uint Corner_Down_Left = (1 << 3);
    static private readonly uint Wall_Up = (1 << 0);
    static private readonly uint Wall_Down = (1 << 1);
    static private readonly uint Wall_Right = (1 << 2);
    static private readonly uint Wall_Left = (1 << 3);
    private readonly uint[] WALL_ORIENTATION = new[]
    {
        Wall_Up,
        Wall_Down,
        Wall_Right,
        Wall_Left
    };
    private readonly uint[] CORNER_ORIENTATION = new[]
{
        Corner_Up_Right,
        Corner_Up_Left,
        Corner_Down_Right,
        Corner_Down_Left
    };

    struct Distraction
    {
        private uint corner;
        public uint Corner
        {
            get { return corner; }
            set { corner = value; }
        }



        private uint wall;
        public uint Wall
        {
            get { return wall; }
            set { wall = value; }
        }


    }
    private const float m_by = 2.25f;
    private readonly float m_high = 0.275f * m_by;
    private readonly float m_wallInvert = 0.39f * m_by;
    private readonly float m_cornerInvert = 0.375f * m_by;

    [SerializeField, HideInInspector]
    private List<GameObject> m_spawnObjects;

    public bool IsAdjacent(int x, int y, Vector2Int direct)
    {
        return IsAdjacent(new(x, y), direct);
    }

    public bool IsAdjacent(Vector2Int point, Vector2Int direct)
    {
        Vector2Int vector2Int = point + direct;
        foreach (GridObject obj in m_grids)
        {
            if (obj.GridPoint == vector2Int)
            {
                return true;
            }
        }
        return false;

    }

    public GridObject GetGridObject(Vector2Int point, Vector2Int direct)
    {
        Vector2Int vector2Int = point + direct;
        int count = m_grids.Count;
        for (int i = 0; i < count; ++i)
        {
            if (m_grids[i].GridPoint == vector2Int)
            {
                return m_grids[i];
            }
        }
        return null;

    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2Int[] inter = new Vector2Int[4] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        int count = m_grids.Count;
        for (int i = 0; i < count; ++i)
        {
            GridObject obj = m_grids[i];
            GridObject right = GetGridObject(obj.GridPoint, Vector2Int.right);
            GridObject left = GetGridObject(obj.GridPoint, Vector2Int.left);
            GridObject up = GetGridObject(obj.GridPoint, Vector2Int.up);
            GridObject down = GetGridObject(obj.GridPoint, Vector2Int.down);

            // Debug.Log(i);


            if (right != null && obj.A_Grid.Right == null)
            {
                Debug.Log(right);
                obj.A_Grid.Right = right;
                right.A_Grid.Left = obj;
            }
            if (left != null && obj.A_Grid.Left == null)
            {
                Debug.Log(left);

                obj.A_Grid.Left = left;
                left.A_Grid.Right = obj;
            }
            if (up != null && obj.A_Grid.Front == null)
            {
                Debug.Log(up);
                obj.A_Grid.Front = up;
                up.A_Grid.Back = obj;
            }
            if (down != null && obj.A_Grid.Back == null)
            {
                Debug.Log(down);

                obj.A_Grid.Back = down;
                down.A_Grid.Front = obj;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroyedObject(GameObject obj)
    {
        int count = m_grids.Count;
        for (int i = 0; i < count; ++i)
        {
            if (m_grids[i].gameObject.GetInstanceID() == obj.GetInstanceID())
            {
                DestroyImmediate(m_grids[i].gameObject.gameObject);
                m_grids.RemoveAt(i);
                break;
            }
        }
    }

    public void PushObjectSpawn()
    {
        m_spawnObjects ??= new List<GameObject>();
        if (m_spawnObjects.Count != 0)
        {
            foreach (GameObject spawn in m_spawnObjects)
            {
                if (spawn != null)
                    DestroyImmediate(spawn);
            }
            m_spawnObjects.Clear();
        }

        // âüâ∫éûÇ…é¿çsÇµÇΩÇ¢èàóù

        foreach (GridObject obj in m_grids)
        {
            Transform trans = obj.transform;

            GameObject child = GameObject.Instantiate(m_floor, trans);
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale *= m_by;
            m_spawnObjects.Add(child);

            var distraction = DistractionArea(obj.GridPoint);
            SpawnWall(
                distraction.Wall,
                m_spawnObjects,
                Vector3.zero,
                trans);
            SpawnCorner(
                distraction.Corner,
                m_spawnObjects,
                Vector3.zero,
                trans);

        }
    }

    private Distraction DistractionArea(Vector2Int vector2Int)
    {
        Distraction distraction = new Distraction();
        for (int i = 0; i < ORIENTATION.Length; ++i)
        {
            if (IsAdjacent(vector2Int, ORIENTATION[i]))
            {
                continue;
            }
            distraction.Wall |= WALL_ORIENTATION[i];

        }
        for (int i = 0; i < ORIENTATION2.GetLength(0); ++i)
        {
            // uint m_m = 0;
            var zz = ORIENTATION2[i, 0];
            var zx = ORIENTATION2[i, 1];
            var xx = ORIENTATION2[i, 2];

            //var PXX = vector2Int + xx;
            //var PXY = vector2Int + zx;
            //var PYY = vector2Int + zz;

            if (!IsAdjacent(vector2Int, xx))
            {
                continue;
            }
            else if (!IsAdjacent(vector2Int, zz))
            {
                continue;
            }
            else if (!IsAdjacent(vector2Int, zx))
            {
                distraction.Corner |= CORNER_ORIENTATION[i];
            }

        }

        return distraction;
    }

    private void SpawnWall
    (
    uint walls,
    List<GameObject> gameObjects,
    Vector3 position,
    Transform parent
    )
    {
        if (walls == 0) return;
        if ((walls & Wall_Up) != 0)
        {
            Vector3 pos = new Vector3(0.0f, m_high, m_wallInvert);
            var game = Spawn(pos, parent, m_wall);
            game.transform.localScale *= m_by;
            gameObjects.Add(game);
            // ga.transform.localRotation
        }
        if ((walls & Wall_Down) != 0)
        {
            Vector3 pos = new Vector3(0.0f, m_high, -m_wallInvert);
            var game = Spawn(pos, parent, m_wall);
            game.transform.localScale *= m_by;

            gameObjects.Add(game);
        }
        if ((walls & Wall_Right) != 0)
        {
            Vector3 pos = new Vector3(m_wallInvert, m_high, 0.0f);
            var game = Spawn(pos, parent, m_wall);
            game.transform.localScale *= m_by;

            game.transform.localRotation = Quaternion.AngleAxis(90.0f, Vector3.up);
            gameObjects.Add(game);
        }
        if ((walls & Wall_Left) != 0)
        {
            Vector3 pos = new Vector3(-m_wallInvert, m_high, 0.0f);
            var game = Spawn(pos, parent, m_wall);
            game.transform.localScale *= m_by;
            game.transform.localRotation = Quaternion.AngleAxis(90.0f, Vector3.up);
            gameObjects.Add(game);
        }
    }

    private void SpawnCorner(
        uint corners,
        List<GameObject> gameObjects,
        Vector3 position,
        Transform parent
        )
    {
        if (corners == 0) return;
        if ((corners & Corner_Up_Right) != 0)
        {
            Vector3 pos = new Vector3(m_cornerInvert, m_high, m_cornerInvert);
            var game = Spawn(pos, parent, m_corner);
            game.transform.localScale *= m_by;
            gameObjects.Add(game);
        }
        if ((corners & Corner_Up_Left) != 0)
        {
            Vector3 pos = new Vector3(-m_cornerInvert, m_high, m_cornerInvert);
            var game = Spawn(pos, parent, m_corner);
            game.transform.localScale *= m_by;
            gameObjects.Add(game);
        }
        if ((corners & Corner_Down_Right) != 0)
        {
            Vector3 pos = new Vector3(m_cornerInvert, m_high, -m_cornerInvert);
            var game = Spawn(pos, parent, m_corner);
            game.transform.localScale *= m_by;

            gameObjects.Add(game);
        }
        if ((corners & Corner_Down_Left) != 0)
        {
            Vector3 pos = new Vector3(-m_cornerInvert, m_high, -m_cornerInvert);
            var game = Spawn(pos, parent, m_corner);
            game.transform.localScale *= m_by;
            gameObjects.Add(game);
        }
    }
    private GameObject Spawn(Vector3 pos, Transform parent, GameObject spawn)
    {
        GameObject gamerObject = GameObject.Instantiate(
                spawn, parent);
        gamerObject.transform.localPosition = pos;
        return gamerObject;
    }

}

