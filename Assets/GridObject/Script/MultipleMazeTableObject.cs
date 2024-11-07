using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Maze", menuName = "ScriptableObjects/CreateMultipleMaze")]
public class MultipleMazeTableObject : MazeTable
{
    [System.Serializable]
    public class MazeArea
    {
        public enum Type
        {
            Area,
            Maze,
            Line
        }
        [SerializeField]
        private Type type;
        public Type AreaType
        {
            set { type = value; }
            get { return type; }
        }


        [SerializeField]
        private Vector2Int max;
        public Vector2Int Max
        {
            set { max = value; }

            get { return max; }
        }
        [SerializeField]
        private Vector2Int firstPoint;
        public Vector2Int FirstPoint
        {
            set { firstPoint = value; }

            get { return firstPoint; }
        }
        public Vector2Int Min
        {
            set { firstPoint = value; }

            get { return firstPoint; }
        }

        public enum Direction
        {
            [InspectorName("âE")]
            Right,
            [InspectorName("ç∂")]
            Left,
            [InspectorName("ëO")]
            Front,
            [InspectorName("å„")]
            Back,

        };

        [SerializeField]
        private Direction direction;
        public Direction Directed
        {
            set { direction = value; }
            get { return direction; }
        }

        [SerializeField]
        private int lineLength;
        public int LineLength
        {
            set { lineLength = value; }
            get { return lineLength; }
        }
        private bool open;
        public bool Open
        {
            get { return open; }
            set { open = value; }
        }

    }


    [SerializeField]
    private List<MazeArea> m_area;
    public List<MazeArea> MultipleArea
    {
        get { return m_area; }
        set { m_area = value; }
    }
}
