using System.Collections.Generic;
using UnityEngine;

public class MazeTable : ScriptableObject
{

    [SerializeField]
    private List<Vector2Int> m_mazeList = new List<Vector2Int>();
    public List<Vector2Int> MazeList
    {
        get { return m_mazeList; }
        set { m_mazeList = value; }
    }
}
