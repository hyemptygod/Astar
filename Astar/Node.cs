namespace Astar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public abstract class Cell
    {
        public Vector pos { get; set; }
        public bool walkable { get; set; }
        public abstract int cost { get; }
        public Cluster cluster { get; set; }
    }

    public class Node : IComparable<Node>
    {
        public static uint UID = 1;
        public Cell cell { get; set; }
        
        public uint uid { get; set; }

        private float m_F;
        private float m_G;
        private float m_H;

        private float m_R1;
        private float m_R2;

        public Node parent { get; set; }

        public Node(Cell cell, float g, float h, Vector start, Vector end, float dst)
        {
            this.cell = cell;
            m_G = g;
            m_H = h;

            float dsn = (cell.pos - start).magnitude;
            float dnt = (end - cell.pos).magnitude;
            m_R1 = cell.cluster.pt + dsn / dst;
            m_R2 = cell.cluster.po + (float)Math.Pow(Math.E, dnt / dst);

            uid = UID++;
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
            m_F = m_R1 * m_G + m_R2 * m_H;
        }

        public int CompareTo(Node other)
        {
            return m_F.CompareTo(other.m_F);
        }
    }
}
