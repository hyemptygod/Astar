namespace Astar
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Runtime.CompilerServices;

    public enum NeighbourMode
    {
        Eight = 1,
        Four = 2,
    }

    public enum SearchStatus
    {
        Idle = 0,
        Running = 1,
        Finished = 2,
        Scanned = 3,
    }

    public abstract class BaseEvent
    {
        protected IMap m_Map;
        protected Vector m_Start;
        protected Vector m_End;
        protected EventResult m_Result;
        protected Node m_StartNode;
        protected Node m_EndNode;
        protected float m_Dst;
        protected readonly Dictionary<Vector, Node> m_Nodes;

        public BaseEvent(IMap map)
        {
            m_Map = map;
            m_Result = new EventResult()
            {
                route = new List<BaseCell>()
            };
            m_Nodes = new Dictionary<Vector, Node>(32);
        }

        protected Node GetNode(BaseCell cell)
        {
            if (!m_Nodes.TryGetValue(cell.pos, out Node node))
            {
                node = new Node(cell, m_Start, m_End, m_Dst);
                m_Nodes.Add(cell.pos, node);
            }
            return node;
        }

        public virtual void Init(BaseCell start, BaseCell end)
        {
            if (start == null || !start.walkable)
            {
                throw new Exception(string.Format("Astar Search Error. {0} is not walkable", start));
            }

            if (end == null || !end.walkable)
            {
                throw new Exception(string.Format("Astar Search Error. {0} is not walkable", end));
            }

            m_Start = start.pos;
            m_End = end.pos;
            m_Dst = (m_End - m_Start).magnitude;
            m_Result.Reset();
            m_Nodes.Clear();
            m_StartNode = GetNode(start);
            m_EndNode = GetNode(end);
        }

        public EventResult Scan()
        {
            m_Result.elapsedTime = Util.Runner("", ScanHandle);

            if(m_Result.searched)
            {
                m_Result.Set(m_StartNode, m_EndNode);
            }
            return m_Result;
        }

        protected abstract void ScanHandle();
    }

    public struct EventResult
    {
        public bool searched;
        public List<BaseCell> route;
        public long elapsedTime;
        public int compareTimes;

        public void Set(Node start, Node end)
        {
            if (searched)
            {
                Node node = end;
                while (node != null)
                {
                    route.Add(node.cell);
                    node = node.parent;
                }
                route.Reverse();
            }
            else
            {
                route.Add(start.cell);
                route.Add(end.cell);
            }
        }

        public void Reset()
        {
            route.Clear();
            searched = false;
        }
    }

    public class AstarEvent : BaseEvent
    {
        public static int n = 2;
        private readonly Dictionary<Vector, Node> m_ClosedList;
        private readonly MinHeap<Node> m_OpenList;

        public AstarEvent(IMap map) : base(map)
        {
            m_OpenList = new MinHeap<Node>(n, true);
            m_ClosedList = new Dictionary<Vector, Node>(32);
        }

        public override void Init(BaseCell start, BaseCell end)
        {
            base.Init(start, end);
            m_OpenList.Clear();
            m_ClosedList.Clear();
        }

        protected override void ScanHandle()
        {
            m_OpenList.Push(m_StartNode);
            Node current = null;
            while (m_OpenList.Count > 0)
            {
                if (!m_OpenList.TryPop(ref current))
                {
                    break;
                }
                m_ClosedList[current.cell.pos] = current;
                if (current == m_EndNode)
                {
                    m_Result.searched = true;
                    break;
                }
                foreach (var it in current.cell.neighbours)
                {
                    if (m_ClosedList.ContainsKey(it.Key.pos))
                    {
                        continue;
                    }
                    Node node = GetNode(it.Key);
                    if (node.UpdateG(current, it.Value))
                    {
                        node.parent = current;
                        m_OpenList.Push(node);
                    }
                }
            }
            m_Result.compareTimes = m_ClosedList.Count;
        }
    }

    public static class PathSearchHelper
    {
        private static Dictionary<IMap, List<AstarEvent>> _pool = new Dictionary<IMap, List<AstarEvent>>();
        public static void UpdateN(int n)
        {
            AstarEvent.n = n;
            _pool.Clear();
        }

        public static TaskAwaiter<EventResult> AstarSearch(this IMap map, BaseCell start, BaseCell end)
        {
            return map.AstarSearchAsync(start, end).GetAwaiter();
        }

        /// <summary>
        /// A*算法搜索路径
        /// </summary>
        /// <param name="map">地图数据</param>
        /// <param name="start">起始点</param>
        /// <param name="end">终点</param>
        /// <param name="n">开放列表才用n叉最小堆</param>
        /// <returns></returns>
        public async static Task<EventResult> AstarSearchAsync(this IMap map, BaseCell start, BaseCell end)
        {
            AstarEvent evt;
            if (_pool.TryGetValue(map, out List<AstarEvent> evts) && evts != null && evts.Count > 0)
            {
                evt = evts[0];
                evts.RemoveAt(0);
            }
            else
            {
                evt = new AstarEvent(map);
            }

            evt.Init(start, end);

            EventResult r = await Task.Run(evt.Scan);

            if (evts == null)
            {
                evts = new List<AstarEvent>();
                _pool[map] = evts;
            }

            evts.Add(evt);

            return r;
        }
    }
}
