namespace Astar
{
    using System;
    using System.Collections.Generic;


    public interface IMap
    {
        int rows { get; }
        int cols { get; }
        BaseCell this[int x, int y] { get; }
        BaseCell this[Vector pos] { get; }
    }

    public struct Cluster
    {
        public int index;
        /// <summary>
        /// 质心点
        /// </summary>
        public Vector centroid;
        public List<Vector> points;
        /// <summary>
        /// 障碍率
        /// </summary>
        public float po;
        /// <summary>
        /// 直通率
        /// </summary>
        public float pt;

        private Vector m_Sum;
        private Vector m_Min;
        private Vector m_Max;

        public Cluster(int index, Vector centroid)
        {
            this.index = index;
            this.centroid = centroid;

            points = new List<Vector>();
            po = 0f;
            pt = 0f;
            m_Sum = Vector.zero;
            m_Min = new Vector(int.MaxValue, int.MaxValue);
            m_Max = Vector.zero;
        }

        public void Add(Vector p)
        {
            points.Add(p);
            m_Sum += p;
            if (p.x < m_Min.x)
            {
                m_Min.x = p.x;
            }
            if (p.x > m_Max.x)
            {
                m_Max.x = p.x;
            }

            if (p.y < m_Min.y)
            {
                m_Min.y = p.y;
            }
            if (p.y > m_Max.y)
            {
                m_Max.y = p.y;
            }
        }

        public bool Update()
        {
            Vector c = m_Sum / points.Count;
            if (c != centroid)
            {
                centroid = c;
                return true;
            }
            return false;
        }

        public void Finish(IMap map)
        {
            BaseCell cell;
            int n0 = 0;
            int n1 = 0;
            int rows = m_Max.x - m_Min.x + 1;
            int cols = m_Max.y - m_Min.y + 1;
            int[,] all = new int[rows, cols];

            int CalculateLD(bool isL, Vector min)
            {
                bool start = false;
                bool iscontinue = false;
                int result = 0;
                int temp = 0;
                int len0 = isL ? rows : cols;
                int len1 = isL ? cols : rows;
                for (int i = 0; i < len0; i++)
                {
                    for (int j = 0; j < len1; j++)
                    {
                        temp = isL ? all[i, j] : all[j, i];
                        if (!start && temp == 1)
                        {
                            start = true;
                            result++;
                            iscontinue = true;
                            continue;
                        }
                        if (start)
                        {
                            cell = isL ? map[i + min.x, j + min.y] : map[j + min.x, i + min.y];
                            if (!cell.walkable)
                            {
                                iscontinue = false;
                            }
                            if (temp == 0)
                            {
                                start = false;
                                if (iscontinue)
                                {
                                    n1++;
                                }
                            }
                        }
                    }
                }
                return result;
            }

            for (int i = 0; i < points.Count; i++)
            {
                cell = map[points[i]];
                if (cell != null)
                {
                    cell.cluster = this;
                    if (!cell.walkable)
                    {
                        n0++;
                    }
                    all[points[i].x - m_Min.x, points[i].y - m_Min.y] = 1;
                }
            }

            int l = CalculateLD(true, m_Min);
            int d = CalculateLD(false, m_Min);

            po = (float)n0 / points.Count;
            pt = (float)n1 / (l + d);
        }

        public void Reset()
        {
            points.Clear();
            m_Sum = Vector.zero;
        }
    }

    public class Map<T> : IMap where T : BaseCell
    {
        public int rows { get; private set; }
        public int cols { get; private set; }

        private T[,] m_Cells;

        public BaseCell this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= rows || y < 0 || y >= cols)
                {
                    return null;
                }
                return m_Cells[x, y];
            }
            set
            {
                if (x < 0 || x >= rows || y < 0 || y >= cols)
                {
                    return;
                }
                m_Cells[x, y] = (T)value;
            }
        }

        public BaseCell this[Vector pos]
        {
            get
            {
                return this[pos.x, pos.y];
            }
            set
            {
                this[pos.x, pos.y] = value;
            }
        }
        private Clusters m_Clusters;

        public Map(T[,] cells, int rows, int cols)
        {
            m_Cells = cells;
            this.rows = rows;
            this.cols = cols;
        }

        public void Init(int k, bool piercing, NeighbourMode mode = NeighbourMode.Eight)
        {
            m_Clusters = new Clusters(this, k);
            CalculateNeighbours(piercing, mode);
        }

        public void CalculateNeighbours(bool piercing, NeighbourMode mode = NeighbourMode.Eight)
        {
            BaseCell current;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    current = this[i, j];
                    if(current == null || !current.walkable)
                    {
                        continue;
                    }
                    current.CalculateNeighbours(this, mode, piercing);
                }
            }
        }

        private class Clusters
        {
            private const int N = 128;
            private int m_Count;
            private Cluster[] m_Items;

            public Clusters(IMap map, int count)
            {
                m_Count = Initialize(map, count);

                KMeansPartition(map, 1);

                for (int i = 0; i < m_Count; i++)
                {
                    m_Items[i].Finish(map);
                }
            }

            private int Initialize(IMap map, int count)
            {
                m_Items = new Astar.Cluster[count];

                int index = 0;
                BaseCell cell;
                for (int i = 0; i < map.rows; i++)
                {
                    for (int j = 0; j < map.cols; j++)
                    {
                        cell = map[i, j];
                        if (cell == null || !cell.walkable)
                        {
                            m_Items[index] = new Astar.Cluster(index, new Vector(i, j));
                            index++;
                        }

                        if (index == count)
                        {
                            return index;
                        }
                    }
                }
                return index;
            }

            private void KMeansPartition(IMap map, int depth)
            {
                for (int i = 0; i < m_Count; i++)
                {
                    m_Items[i].Reset();
                }

                Vector v = Vector.zero;
                float min, d;
                int index;
                for (int x = 0; x < map.rows; x++)
                {
                    v.x = x;
                    for (int y = 0; y < map.cols; y++)
                    {
                        v.y = y;
                        min = (v - m_Items[0].centroid).sqrMagnitude;
                        index = 0;
                        for (int i = 1; i < m_Count; i++)
                        {
                            d = (v - m_Items[i].centroid).sqrMagnitude;
                            if (d < min)
                            {
                                min = d;
                                index = i;
                            }
                        }

                        m_Items[index].Add(v);
                    }
                }

                bool changed = false;
                for (int i = 0; i < m_Count; i++)
                {
                    if (m_Items[i].Update())
                    {
                        changed = true;
                    }
                }

                if (changed && depth < N)
                {
                    return;
                }

                KMeansPartition(map, depth + 1);
            }
        }
    }
}
