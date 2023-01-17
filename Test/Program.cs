namespace AstarTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Astar;
    using System.Drawing;

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
            TestFloyd();

            //while (true)
            //{
            //    int rows = 0;
            //    while (true)
            //    {
            //        Console.WriteLine("please input rows(greater than 0):");
            //        if (int.TryParse(Console.ReadLine(), out rows) && rows > 0)
            //        {
            //            break;
            //        }
            //    }

            //    int cols = 0;
            //    while (true)
            //    {
            //        Console.WriteLine("please input cols(greater than 0):");
            //        if (int.TryParse(Console.ReadLine(), out cols) && cols > 0)
            //        {
            //            break;
            //        }
            //    }

            //    Map<Cell> map = Util.Runner("generate map", MapGenerateHelper.GenerateRandomMap, rows, cols);
            //    AstarHelper helper = new AstarHelper(map);
            //    helper.Scan(map[0, 0], map[rows - 1, cols - 1]);
            //    map.Create("route", helper.Result);
            //}
        }
    }
}
