namespace Astar
{
    using System;
    using System.Collections.Generic;
using System.Collections.ObjectModel;
    using System.Text;

    public enum NeighbourMode
    {
        Eight,
        Four,
    }

    public class AstarHelper
    {
        private readonly IMap m_Map;
        private readonly Dictionary<Vector, Node> m_Nodes;
        private readonly Dictionary<Vector, Node> m_ClosedList;
        private readonly MinHeap<Node> m_OpenList;
        private readonly NeighbourMode m_NeighbourMode;
        private readonly List<ICell> m_Result;

        private Node m_Start;
        private Node m_End;
        private Vector m_EndPos;

        public bool Searched { get; private set; }

        public List<ICell> Result
        {
            get
            {
                m_Result.Clear();
                if (Searched)
                {
                    Node node = m_End;
                    while (node != null)
                    {
                        m_Result.Add(node.cell);
                        node = node.Parent;
                    }
                    m_Result.Reverse();
                }
                else
                {
                    m_Result.Add(m_Start.cell);
                    m_Result.Add(m_End.cell);
                }
                return m_Result;
            }
        }

        public AstarHelper(IMap map, NeighbourMode mode = NeighbourMode.Eight, int n = 2)
        {
            m_Map = map;
            m_NeighbourMode = mode;
            m_OpenList = new MinHeap<Node>(n, true);
            int count = map.Rows * map.Cols;
            m_Nodes = new Dictionary<Vector, Node>(count);
            m_ClosedList = new Dictionary<Vector, Node>(count);
            m_Result = new List<ICell>(count);
        }

        private Node GetNode(ICell cell, float g = float.MaxValue)
        {
            if (!m_Nodes.TryGetValue(cell.pos, out Node node))
            {
                node = new Node(cell)
                {
                    G = g,
                    H = Heuristic(cell.pos),
                };
                m_Nodes.Add(cell.pos, node);
            }

            return node;
        }

        private float Heuristic(Vector pos)
        {
            return Vector.Chebyshev(pos, m_EndPos);
        }

        private void Start()
        {
            m_OpenList.Push(m_Start);
            Node current = null;
            while(m_OpenList.Count > 0)
            {
                if(!m_OpenList.TryPop(ref current))
                {
                    break;
                }
                m_ClosedList[current.cell.pos] = current;
                if(current == m_End)
                {
                    Searched = true;
                    break;
                }

                foreach (var next in current.GetNeighbours(m_Map, m_NeighbourMode))
                {
                    if (m_ClosedList.ContainsKey(next.cell.pos))
                    {
                        continue;
                    }

                    float g = current.G + next.cost;
                    Node node = GetNode(next.cell);
                    if (g < node.G)
                    {
                        node.G = g;
                        node.F = node.G + node.H;
                        node.Parent = current;
                        m_OpenList.Push(node);
                    }
                }
            }
        }

        public void Scan(ICell start, ICell end)
        {
            if (start == null || !start.walkable)
            {
                throw new Exception(string.Format("Astar Search Error. {0} is not walkable", start));
            }

            if (end == null || !end.walkable)
            {
                throw new Exception(string.Format("Astar Search Error. {0} is not walkable", end));
            }

            m_EndPos = end.pos;

            m_OpenList.Clear();
            m_ClosedList.Clear();
            m_Nodes.Clear();
            m_Result.Clear();

            Searched = false;

            m_Start = GetNode(start, 0);
            m_End = GetNode(end);

            Util.Runner(string.Format("scan from {0} to {1}", start.pos, end.pos), Start);
        }
    }
}
