namespace Astar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class FloydHelper
    {
        public static int K = 1;

        public static List<Vector> Floyd(this IMap map, List<Vector> list)
        {
            int len = list.Count;
            if (len > 2)
            {
                Vector v = list[len - 1] - list[len - 2];
                Vector temp;
                for (int i = len - 3; i >= 0;)
                {
                    temp = list[i + 1] - list[i];
                    if (temp != v)
                    {
                        v = temp;
                        i--;
                    }
                    else
                    {
                        list.RemoveAt(i + 1);
                    }
                }
            }

            len = list.Count;
            Console.WriteLine(len);
            int index = 0;
            Vector original, start, end, result = Vector.zero;
            while (index < len)
            {
                original = list[index];
                for (int i = len - 1; i > index + 1; i--)
                {
                    start = list[i - 1];
                    end = list[i];
                    if (map.CheckRouteWalkableStep(original, start, end, ref result))
                    {
                        list.RemoveRange(index + 1, i - 1 - index);
                        list.Insert(index + 1, result);
                        break;
                    }
                }
                index++;
                len = list.Count;
            }
            Console.WriteLine(len);
            return list;
        }

        private static bool CheckRouteWalkableStep(this IMap map, Vector original, Vector start, Vector end, ref Vector result)
        {
            Vector step = end - start;
            float max = step.sqrMagnitude;
            if (step.x == 0 && step.y != 0)
            {
                step.y = step.y / Math.Abs(step.y) * K;
            }
            else if (step.x != 0 && step.y == 0)
            {
                step.x = step.x / Math.Abs(step.x) * K;
            }
            else
            {
                return false;
            }
            Vector now = end;
            while (now != start)
            {
                if (map.CheckRouteWalkable(original, now))
                {
                    result = now;
                    return true;
                }
                now -= step;
                if ((end - now).sqrMagnitude > max)
                {
                    now = start;
                    if (map.CheckRouteWalkable(original, now))
                    {
                        result = now;
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CheckRouteWalkable(this IMap map, Vector start, Vector end)
        {
            Vector step = end - start;
            if (step.y != 0)
            {
                step.y /= Math.Abs(step.y);
            }

            float deltay = 0;
            if (step.x != 0)
            {
                deltay = step.y / Math.Abs(step.x);
                step.x /= Math.Abs(step.x);
            }
            else
            {
                deltay = step.y * 2;
            }

            float nowx = start.x + step.x / 2f;
            float nowy = start.y - step.y / 2f;
            float checky = nowy;
            Vector checkv = start;
            BaseCell cell;
            bool check()
            {
                cell = map[checkv];
                if (cell == null || !cell.Walkable)
                {
                    return false;
                }
                return true;
            }

            while (checkv != end)
            {
                if (!check())
                {
                    Console.WriteLine(checkv);
                    return false;
                }
                nowy += deltay;
                if ((nowy - checky - step.y) * step.y > 0)
                {
                    checky += step.y;
                    checkv.y += step.y;
                    if (!check())
                    {
                        Console.WriteLine(checkv);
                        return false;
                    }
                }
                checkv.x += step.x;
            }

            return true;

        }
    }
}
