namespace Astar
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using static Astar.Node;

    public enum NeighbourMode
    {
        Eight,
        Four,
    }

    public class AstarEvent
    {
        private IMap m_Map;
        private NeighbourMode m_NeighbourMode;
        private readonly Dictionary<Vector, Node> m_Nodes;
        private readonly Dictionary<Vector, Node> m_ClosedList;
        private readonly MinHeap<Node> m_OpenList;
        private Node m_Start;
        private Node m_End;
        private Vector m_EndPos;
        private Zones.Zone m_Zone;

        public bool Searched { get; private set; }

        private readonly List<ICell> m_Result;
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

        public AstarEvent(IMap map, NeighbourMode mode, int n = 2)
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

        public void Scan(Zones.Zone zone, ICell start, ICell end)
        {
            if (start == null || !start.walkable)
            {
                throw new Exception(string.Format("Astar Search Error. {0} is not walkable", start));
            }

            if (end == null || !end.walkable)
            {
                throw new Exception(string.Format("Astar Search Error. {0} is not walkable", end));
            }

            m_Zone = zone;
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

        private void Start()
        {
            m_OpenList.Push(m_Start);
            Node current = null;
            while (m_OpenList.Count > 0)
            {
                if (!m_OpenList.TryPop(ref current))
                {
                    break;
                }
                m_ClosedList[current.cell.pos] = current;
                if (current == m_End)
                {
                    Searched = true;
                    break;
                }

                GetNeighbours(current);
            }
        }

        private void GetNeighbours(Node current)
        {
            if (current == null)
            {
                return;
            }

            Vector next;
            ICell cell = null;
            float cost = 0f;
            bool check_cell(Vector offset)
            {
                next = current.cell.pos + offset;
                if (m_ClosedList.ContainsKey(next))
                {
                    return false;
                }
                cell = m_Map[next];
                if (cell == null || !cell.walkable)
                {
                    return false;
                }
                cost = cell.cost * offset.magnitude;
                return true;
            }

            foreach (var offset in Vector.Neighbours(m_NeighbourMode))
            {
                if (check_cell(offset))
                {
                    float g = current.G + cost;
                    Node node = GetNode(cell);
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
    }

    public class AstarHelper
    {
        private readonly IMap m_Map;
        private readonly NeighbourMode m_NeighbourMode;

        public AstarHelper(IMap map, NeighbourMode mode = NeighbourMode.Eight, int n = 2)
        {
            m_Map = map;
            m_NeighbourMode = mode;
        }
    }
}
