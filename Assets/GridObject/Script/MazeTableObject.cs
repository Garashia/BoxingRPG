using UnityEngine;

[CreateAssetMenu(fileName = "Maze", menuName = "ScriptableObjects/CreateMaze")]
public class MazeTableObject : MazeTable
{
    //private MazeFactory m_mazeFactory;
    [SerializeField]
    private int x, y;
    public int X
    {
        get { return x; }
        set { x = value; }
    }
    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    //[SerializeField]
    //private List<Vector2Int> m_mazeList = new List<Vector2Int>();
    //public List<Vector2Int> MazeList
    //{
    //    get { return m_mazeList; }
    //    set { m_mazeList = value; }
    //}

}
