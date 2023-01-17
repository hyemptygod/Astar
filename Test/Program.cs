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
        static void Main(string[] args)
        {
            while(true)
            {
                int rows = 0;
                while (true)
                {
                    Console.WriteLine("please input rows(greater than 0):");
                    if (int.TryParse(Console.ReadLine(), out rows) && rows > 0)
                    {
                        break;
                    }
                }

                int cols = 0;
                while (true)
                {
                    Console.WriteLine("please input cols(greater than 0):");
                    if (int.TryParse(Console.ReadLine(), out cols) && cols > 0)
                    {
                        break;
                    }
                }

                Map<Cell> map = Util.Runner("generate map", MapGenerateHelper.GenerateRandomMap, rows, cols);
                AstarHelper helper = new AstarHelper(map);
                helper.Scan(map[0, 0], map[rows - 1, cols - 1]);
                map.Create("route", helper.Result);
            }
        }
    }
}
