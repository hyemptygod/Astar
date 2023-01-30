namespace Astar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public abstract class BaseCell
    {
        public Vector pos { get; set; }
        public bool walkable { get; set; }
        internal Cluster cluster { get; set; }
        public Dictionary<BaseCell,float> neighbours { get; set; }

        public virtual float Cost(Vector offset)
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

        public Node parent { get; set; }

        public Node(BaseCell cell, Vector start, Vector end, float dst)
        {
            this.cell = cell;
            m_G = cell.pos == start ? 0 : float.MaxValue;
            m_H = Heuristic(cell.pos, end);

            float dsn = (cell.pos - start).magnitude;
            float dnt = (end - cell.pos).magnitude;
            m_R1 = cell.cluster.pt + dsn / dst;
            m_R2 = (1- cell.cluster.po) + (float)Math.Pow(Math.E, dnt / dst);
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
