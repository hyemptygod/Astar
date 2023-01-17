namespace Astar
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    public interface IMap
    {
        int Rows { get; }
        int Cols { get; }
        ICell this[int x, int y] { get; }
        ICell this[Vector pos] { get; }
    }

    public class Zones
    {
        public struct Zone
        {
            public Vector start;
            public Vector end;

            public Bound GetBounds(Vector offset)
            {
                Bound bound = new Bound();
                for (int i = 0; i <= 1; i++)
                {
                    if (offset[i] > 0)
                    {
                        bound[i, 0] = end[i];
                        bound[i, 1] = end[i];
                    }
                    else if (offset[i] < 0)
                    {
                        bound[i, 0] = start[i];
                        bound[i, 1] = start[i];
                    }
                    else
                    {
                        bound[i] = new Vector(start[i], end[1]);
                    }
                }
                return bound;
            }
        }

        private Zone[] m_Zone;
        private int m_ZoneX;
        private int m_ZoneY;

        private int[,] m_Path;

        public List<int> this[int start, int end]
        {
            get
            {

            }
        }

        public Zones(IMap map, int rows, int cols, int zonex, int zoney)
        {
            m_ZoneX = zonex;
            m_ZoneY = zoney;
            m_Zone = new Zone[m_ZoneX * m_ZoneY];
            m_Path = new int[m_ZoneX, m_ZoneY];
            Vector step = new Vector(rows / m_ZoneX, cols / m_ZoneY);
            Vector start = Vector.zero;
            Vector end = Vector.one;
            for (int i = 0; i < m_ZoneX; i++)
            {
                start.x = end.x - 1;
                end.x = Math.Min(end.x + step.x, rows - 1);
                for (int j = 0; j < m_ZoneY; j++)
                {
                    start.y = end.y - 1;
                    end.y = Math.Min(end.y + step.y, cols - 1);
                    m_Zone[i * m_ZoneY + j] = new Zone
                    {
                        start = start,
                        end = end,
                    };
                }
            }

            float[,] cost = new float[m_Zone.Length, m_Zone.Length];
            for (int i = 0; i < m_Zone.Length; i++)
            {
                for (int j = 0; j < m_Zone.Length; j++)
                {
                    cost[i, j] = Distance(i, j, map);
                }
            }

            for (int k = 0; k < m_Zone.Length; k++)
            {
                for (int i = 0; i < m_Zone.Length; i++)
                {
                    for (int j = 0; j < m_Zone.Length; j++)
                    {
                        float d = cost[i, k] + cost[k, j];
                        if (d < cost[i, j])
                        {
                            cost[i, j] = d;
                            m_Path[i, j] = k;
                        }
                    }
                }
            }
        }

        private float Distance(int index1, int index2, IMap map)
        {
            if(index1 == index2)
            {
                return 0;
            }

            Vector va = new Vector(index1 / m_ZoneY, index1 % m_ZoneY);
            Vector vb = new Vector(index2 / m_ZoneY, index2 % m_ZoneY);
            Vector offset = vb - va;
            foreach (var item in Vector.eight)
            {
                if (item == offset)
                {
                    foreach (var v in m_Zone[index1].GetBounds(offset))
                    {
                        if (map[v].walkable)
                        {
                            return 1f;
                        }
                    }
                }
            }

            return float.MaxValue;
        }

        private void GetPath(int start, int end)
        {

        }
    }

    public class Map<T> : IMap where T : class, ICell
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        private int m_ZoneX;
        private int m_ZoneY;

        private T[,] m_Cells;

        public ICell this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Rows || y < 0 || y >= Cols)
                {
                    return null;
                }
                return m_Cells[x, y];
            }
            set
            {
                if (x < 0 || x >= Rows || y < 0 || y >= Cols)
                {
                    return;
                }
                m_Cells[x, y] = (T)value;
            }
        }

        public ICell this[Vector pos]
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

        private Zones m_Zones;
        private int[,] m_Costs;

        public Map(T[,] cells, int rows, int cols, int zonex = 1, int zoney = 1)
        {
            m_Cells = cells;
            
            Rows = rows;
            Cols = cols;

            m_ZoneX = zonex <= 0 ? 1 : zonex;
            m_ZoneY = zoney <= 0 ? 1 : zoney;

            //分区
            Partition();
        }

        private void Partition()
        {
            
        }

        private float CalculateZoneCost(Vector startZone, Vector endZone)
        {
            if(startZone == endZone)
            {
                return 0;
            }

            Vector offset = endZone - startZone;
            int startx, endx, starty, endy;
            foreach (var item in Vector.eight)
            {
                if(item == offset)
                {
                    
                }
            }
        }
    }
}
