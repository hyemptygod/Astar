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
        public class Zone : IFloydNode
        {
            public int index { get; set; }
            public Vector start { get; set; }
            public Vector end { get; set; }
            public Vector pos { get; set; }

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

            public float GetDistance(IFloydNode node)
            {
                if(node == this)
                {
                    return 0;
                }

                Vector offset = (node as Zone).pos - pos;
                foreach (var item in Vector.eight)
                {
                    if (item == offset)
                    {
                        foreach (var v in GetBounds(offset))
                        {
                            if (_map[v].walkable)
                            {
                                return 1f;
                            }
                        }
                    }
                }

                return float.MaxValue;
            }
        }

        private static IMap _map;
        private int m_ZoneX;
        private int m_ZoneY;

        private Zone[] m_Zone;

        private int[,] m_Path;

        public List<int> this[int start, int end]
        {
            get
            {
                List<int> result = new List<int>();
                if(m_Path.TryGetFloydPath(start, end, ref result))
                {
                    return result;
                }
                return null;
            }
        }

        public Zones(IMap map, int zonex, int zoney)
        {
            _map = map;

            m_ZoneX = zonex <= 0 ? 1 : zonex;
            m_ZoneY = zoney <= 0 ? 1 : zoney;
            m_Zone = new Zone[m_ZoneX * m_ZoneY];
            m_Path = new int[m_ZoneX, m_ZoneY];
            Vector step = new Vector(map.Rows / m_ZoneX, map.Cols / m_ZoneY);
            Vector start = Vector.zero;
            Vector end = Vector.one;
            for (int i = 0; i < m_ZoneX; i++)
            {
                start.x = end.x - 1;
                end.x = Math.Min(end.x + step.x, map.Rows - 1);
                for (int j = 0; j < m_ZoneY; j++)
                {
                    start.y = end.y - 1;
                    end.y = Math.Min(end.y + step.y, map.Cols - 1);
                    int index = i * m_ZoneY + j;
                    m_Zone[index] = new Zone()
                    {
                        index = index,
                        pos = new Vector(i, j),
                        start = start,
                        end = end,
                    };
                }
            }

            m_Path = m_Zone.Floyd();
        }

        private void GetPath(int start, int end, ref List<int> route)
        {
            if(start != end)
            {
                GetPath(start, m_Path[start, end], ref route);
            }
            route.Add(end);
        }
    }

    public class Map<T> : IMap where T : class, ICell
    {
        public int Rows { get; private set; }
        public int Cols { get; private set; }

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

        public Map(T[,] cells, int rows, int cols, int zonex = 1, int zoney = 1)
        {
            m_Cells = cells;
            
            Rows = rows;
            Cols = cols;

            //分区
            m_Zones = new Zones(this, zonex, zoney);
        }
    }
}
