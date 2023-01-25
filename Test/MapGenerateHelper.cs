namespace AstarTest
{
    using Astar;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Diagnostics;

    public class Cell : Astar.Cell
    {
        public override int cost { get { return 1; } }
    }

    public static class MapGenerateHelper
    {
        private const int LINE = 1;
        private const int GRID = 20;

        private readonly static Vector[] _dir =
        {
            Vector.right,
            Vector.right_up,
            Vector.up,
        };
        private readonly static int _dirLen = _dir.Length;

        private static bool RandomMove(Random r, Vector current, Vector start, Vector end, List<Vector> list)
        {
            list.Add(current);
            
            if (current == end)
            {
                return true;
            }

            Vector next;
            for (int i = 0; i < _dirLen; i++)
            {
                next = current + _dir[i];
                if (next == end)
                {
                    list.Add(end);
                    return true;
                }
            }

            int index = r.Next(_dirLen);
            for (int i = index; i < _dirLen; i++)
            {
                next = current + _dir[index % _dirLen];
                if (next.x < start.x || next.x > end.x || next.y < start.y || next.y > end.y)
                {
                    continue;
                }
                if (list.Contains(next))
                {
                    continue;
                }
                if (RandomMove(r, next, start, end, list))
                {
                    return true;
                }
            }

            list.Remove(current);
            return false;
        }

        public static Map<Cell> GenerateRandomMap(int rows, int cols, int k)
        {
            Map<Cell> result = new Map<Cell>(new Cell[rows, cols], rows, cols, k);
            Vector start = new Vector(0, 0);
            Vector end = new Vector(rows - 1, cols - 1);
            List<Vector> list = new List<Vector>();

            int seed = 0;
            Random r = new Random(seed);
            while(true)
            {
                if (RandomMove(r, start, start, end, list))
                {
                    Console.WriteLine(string.Format("generate success for seed is {0}", seed));
                    break;
                }
                seed++;
                r = new Random(seed);
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Vector pos = new Vector(i, j);
                    result[pos] = new Cell()
                    {
                        pos = pos,
                        walkable = list.Contains(pos) || r.Next(2) == 1,
                    };
                }
            }
            return result;
        }

        public static void Create(this IMap map, string name, List<Astar.Cell> points = null)
        {
            Bitmap bitmap = new Bitmap(map.cols * GRID, map.rows * GRID);

            name += ".png";
            Util.Runner(string.Format("draw {0} line", name), bitmap.DrawGridLine, map, Color.Black);
            Util.Runner(string.Format("draw {0} cell", name), bitmap.DrawCell, map);
            if(points != null)
            {
                Util.Runner(string.Format("draw {0} route", name), bitmap.DrawRoute, points);
            }
            Util.Runner(string.Format("save {0}", name), bitmap.Save, name);
        }

        private static void DrawGridLine(this Bitmap bitmap, IMap map, Color color)
        {
            for (int x = 0; x < map.rows; x++)
            {
                for (int i = 0; i < LINE; i++)
                {
                    int newx = x * GRID + i;
                    for (int y = 0; y < bitmap.Width; y++)
                    {
                        bitmap.SetPixel(y, newx, color);
                    }
                }
            }

            for (int y = 0; y < map.cols; y++)
            {
                for (int i = 0; i < LINE; i++)
                {
                    int newy = y * GRID + i;
                    for (int x = 0; x < bitmap.Height; x++)
                    {
                        bitmap.SetPixel(newy, x, color);
                    }
                }
            }
        }

        private static void DrawCell(this Bitmap bitmap, IMap map)
        {
            for (int i = 0; i < map.rows; i++)
            {
                int x = i * GRID;
                for (int j = 0; j < map.cols; j++)
                {
                    SetPixel(bitmap, x, j * GRID, map[i, j].walkable ? Color.Green : Color.Red);
                }
            }
        }

        private static void SetPixel(Bitmap bitmap, int x, int y, Color color)
        {
            for (int i = LINE; i < GRID; i++)
            {
                for (int j = LINE; j < GRID; j++)
                {
                    bitmap.SetPixel(y + j, x + i, color);
                }
            }
        }

        private static void DrawRoute(this Bitmap bitmap, List<Astar.Cell> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector from = points[i].pos;
                Vector to = points[i + 1].pos;

                int dirx = to.x - from.x;
                float diry = to.y - from.y;
                int len = 0;
                if (dirx == 0)
                {
                    diry = 1;
                    len = Math.Abs(from.y - to.y);
                }
                else
                { 
                    diry /= (float)dirx;
                    dirx = 1;
                    len = Math.Abs(from.x - to.x);
                }

                int startx = (int)((from.x + 0.5f) * GRID);
                int starty = (int)((from.y + 0.5f) * GRID);

                for (int j = 0; j < len * GRID; j++)
                {
                    int x = startx + dirx * j;
                    int y = (int)Math.Round(starty + diry * j);
                    bitmap.SetPixel(y, x, Color.Blue);
                    //bitmap.SetPixel(y+1, x+1, Color.Blue);
                }
            }
        }
    }
}
