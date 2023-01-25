namespace Astar
{
    using System;
using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Threading;

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
        private Vector m_StartPos;
        private Vector m_EndPos;
        private float m_Dst;

        public bool Searched { get; private set; }
        public bool Finished { get; private set; }

        private readonly List<Cell> m_Result;
        public List<Cell> Result
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
                        node = node.parent;
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

            int count = map.rows * map.cols;
            m_Nodes = new Dictionary<Vector, Node>(count);
            m_ClosedList = new Dictionary<Vector, Node>(count);
            m_Result = new List<Cell>(count);
        }

        public void Init(Cell start, Cell end)
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
            m_StartPos = start.pos;
            m_Dst = (m_EndPos - m_StartPos).magnitude;

            m_OpenList.Clear();
            m_ClosedList.Clear();
            m_Nodes.Clear();
            m_Result.Clear();

            Finished = false;
            Searched = false;

            m_Start = GetNode(start, 0);
            m_End = GetNode(end);
        }

        private Node GetNode(Cell cell, float g = float.MaxValue)
        {
            if (!m_Nodes.TryGetValue(cell.pos, out Node node))
            {
                node = new Node(cell, g, Heuristic(cell.pos), m_StartPos, m_EndPos, m_Dst);
                m_Nodes.Add(cell.pos, node);
            }

            return node;
        }

        private float Heuristic(Vector pos)
        {
            return Vector.Chebyshev(pos, m_EndPos);
        }

        public void Scan()
        {
            Util.Runner(string.Format("scan from {0} to {1}", m_StartPos, m_EndPos), Start);
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

            Finished = true;
        }

        private void GetNeighbours(Node current)
        {
            if (current == null)
            {
                return;
            }

            Vector next;
            Cell cell = null;
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
                    Node node = GetNode(cell);
                    if(node.UpdateG(current, cost))
                    {
                        node.parent = current;
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

        private Queue<Thread> m_Thread = new Queue<Thread>();
        private Queue<AstarEvent> _pool = new Queue<AstarEvent>();

        public AstarHelper(IMap map, NeighbourMode mode = NeighbourMode.Eight, int n = 2)
        {
            m_Map = map;
            m_NeighbourMode = mode;
        }

        public AstarEvent Scan(Cell start, Cell end)
        {
            AstarEvent result;
            if (_pool.Count > 0)
            {
                result = _pool.Dequeue();            }
            else
            {
                result = new AstarEvent(m_Map, m_NeighbourMode);
            }

            result.Init(start, end);

            Thread t = new Thread(Scan)
            {
                IsBackground = true
            };
            t.Start(result);

            return result;
        }

        private void Scan(object o)
        {
            if(o != null &&  o is AstarEvent)
            {
                AstarEvent e = o as AstarEvent;
                e.Scan();
            }
        }
    }
}
