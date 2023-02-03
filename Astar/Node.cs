namespace Astar
{
    using System;
    using System.Collections.Generic;

    public abstract class BaseCell
    {
        /// <summary>
        /// 位置信息
        /// </summary>
        public Vector Pos { get; set; }
        /// <summary>
        /// 能否行走
        /// </summary>
        public bool Walkable { get; set; }
        /// <summary>
        /// 簇
        /// </summary>
        internal Cluster Cluster { get; set; }
        /// <summary>
        /// 邻接点对应的消耗
        /// </summary>
        public Dictionary<BaseCell, float> Neighbours { get; set; }

        public int Index { get; private set; }

        /// <summary>
        /// 计算邻接点
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mode"></param>
        /// <param name="piercing"></param>
        public void CalculateNeighbours(IMap map, NeighbourMode mode, bool piercing)
        {
            List<Vector> expects = new List<Vector>();
            Neighbours = new Dictionary<BaseCell, float>();
            BaseCell next;
            if (mode == NeighbourMode.Eight && piercing)
            {
                expects.Clear();
                foreach (var offset in Vector.four)
                {
                    next = map[Pos + offset];
                    if (next == null || !next.Walkable)
                    {
                        expects.Add(offset);
                        if (Vector.dicExcept.TryGetValue(offset, out Vector[] expect))
                        {
                            expects.AddRange(expect);
                        }
                    }
                }
            }

            foreach (var offset in Vector.Neighbours(mode))
            {
                if (expects.Contains(offset))
                {
                    continue;
                }
                next = map[Pos + offset];
                if (next != null && next.Walkable)
                {
                    Neighbours.Add(next, Cost(next, offset));
                }
            }
        }

        /// <summary>
        /// 到邻接点的消耗
        /// </summary>
        /// <param name="offset">方向</param>
        /// <returns></returns>
        protected virtual float Cost(BaseCell next, Vector offset)
        {
            return offset.magnitude;
        }
    }

    public class Node : IComparable<Node>
    {
        public BaseCell cell { get; set; }
        
        private float m_F;
        private float m_G;
        private float m_H;

        private float m_R1;
        private float m_R2;

        public Node Parent { get; set; }

        public int Index { get; set; }

        public Node(BaseCell cell, Vector start, Vector end, float dst)
        {
            this.cell = cell;
            m_G = cell.Pos == start ? 0 : float.MaxValue;
            m_H = Heuristic(cell.Pos, end);

            float dsn = (cell.Pos - start).magnitude;
            float dnt = (end - cell.Pos).magnitude;
            m_R1 = cell.Cluster.pt + dsn / dst;
            m_R2 = (1- cell.Cluster.po) + (float)Math.Pow(Math.E, dnt / dst);
        }

        private float Heuristic(Vector pos, Vector end)
        {
            return Vector.Chebyshev(pos, end);
        }

        public bool UpdateG(Node current, float cost)
        {
            float g = current.m_G + cost;
            if(m_G > g)
            {
                m_G = g;
                CalculateF();
                return true;
            }
            return false;
        }

        private void CalculateF()
        {
            //m_F = m_G + m_H;
            m_F = m_R1 * m_G + m_R2 * m_H;
        }

        public int CompareTo(Node other)
        {
            return m_F.CompareTo(other.m_F);
        }
    }
}
