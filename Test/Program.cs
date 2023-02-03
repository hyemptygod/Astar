namespace AstarTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Astar;
    using System.Drawing;
    using System.Threading;
    using System.Runtime.CompilerServices;

    class Program
    {
        private class FloydNode : IFloydNode
        {
            public int index { get; set; }

            public float GetDistance(IFloydNode node)
            {
                return _data[index, node.index];
            }
        }

        private static float _f = float.MaxValue;
        private static float[,] _data = new float[,] {
                { 0, 7, _f, 5, _f, _f, _f },
                { 7, 0, 8, 9, 7, _f, _f},
                {_f, 8, 0, _f, 5, _f, _f},
                {5, 9, _f, 0, 15, 6, _f},
                {_f, 7, 5, 15, 0, 8, 9},
                {_f, _f, _f, 6, 8, 0, 11},
                {_f, _f, _f, _f, 9, 11, 0},};

        private static void TestFloyd()
        {
            FloydNode[] nodes = new FloydNode[_data.GetLength(0)];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new FloydNode()
                {
                    index = i,
                };
            }
            int[,] path = nodes.Floyd();
            List<int> result = new List<int>();
            while (true)
            {
                int start = 0;
                while (true)
                {
                    Console.WriteLine("please input start(greater than -1):");
                    if (int.TryParse(Console.ReadLine(), out start) && start >= 0)
                    {
                        break;
                    }
                }

                int end = 0;
                while (true)
                {
                    Console.WriteLine("please input end(greater than -1):");
                    if (int.TryParse(Console.ReadLine(), out end) && end >= 0)
                    {
                        break;
                    }
                }
                result.Clear();
                if(path.TryGetFloydPath(start, end, ref result))
                {
                    Console.Write(result[0]);
                    for (int i = 1; i < result.Count; i++)
                    {
                        Console.Write("->" + result[i]);
                    }
                    Console.WriteLine("");
                }
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                int[] temp = GetUInt("行(>0),列(>0)", 2);
                int rows = temp[0];
                int cols = temp[1];
                while (true)
                {
                    Util.Runner("create map", MapGenerateHelper.GenerateRandomMap, out Map<Cell> map, rows, cols);

                    while(true)
                    {
                        temp = GetUInt("分区数(>0), 穿帮优化(1|2), 移动模式(1|2)", 3);
                        map.Init(temp[0], temp[1] == 1, (NeighbourMode)temp[2]);
                        Search(map, 2, out EventResult e1);
                        //Search(map, 3, out EventResult e2);

                        //Console.Write("优化效率:(2 -> 3):");
                        //OutputInfo("耗时", e1.elapsedTime, e2.elapsedTime);
                        //OutputInfo("比较", e1.compareTimes, e2.compareTimes);
                    }
                    //map.Create("route" + index, e.route);
                }
            }
        }

        static int[] GetUInt(string str, int count = 1)
        {
            
            int[] k = new int[count];
            while (true)
            {
                Console.WriteLine(str + ":");
                string[] temp = Console.ReadLine().Split(',');
                if (temp.Length < count)
                {
                    continue;
                }
                else
                {
                    bool success = true;
                    for (int i = 0; i < count; i++)
                    {
                        if (!int.TryParse(temp[i], out int t) || t <= 0)
                        {
                            success = false;
                            break;
                        }
                        k[i] = t;
                    }
                    if(success)
                    {
                        break;
                    }
                }
                
            }
            return k;
        }

        static void Search(IMap map, int n, out EventResult e)
        {
            PathSearchHelper.UpdateN(n);
            var task = map.AstarSearch(map[0, 0], map[map.Rows - 1, map.Cols - 1]);
            while (!task.IsCompleted)
            {
                Thread.Sleep(20);
            }
            e = task.GetResult();
            if (e.searched)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("(Success)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("(Failed)");
            }
            Console.ForegroundColor = e.elapsedTime <= 20 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(string.Format(" 耗时:{0}ms", e.elapsedTime));
            Console.ResetColor();
            Console.WriteLine();
        }

        static void OutputInfo(string str, float e2, float e8)
        {
            Console.Write(str + "(");
            if (e2 > e8)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("↑");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("↓");
            }
            Console.Write(((e2 - e8) / (float)e2).ToString("P2"));
            Console.ResetColor();
            Console.Write(")");
        }

        static TaskAwaiter<int> Run()
        {
            return Print().GetAwaiter();
        }

        static async Task<int> Print()
        {
            var r = await Task.Run(() =>
            {
                Console.WriteLine("start");
                int index = 0;
                float x = 0;
                while (index < 100000)
                {
                    //Console.WriteLine(index);
                    index++;
                    //Thread.Sleep(100);
                    x += 0.1f;
                }
                Console.WriteLine(x);
                return index;
            });
            return r;
        }
       
    }
}
