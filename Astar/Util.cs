namespace Astar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;

    public interface IFloydNode
    {
        int index { get; }
        float GetDistance(IFloydNode node);
    }

    public static class Util
    {

        public static float Clamp01(this float x)
        {
            return x.Clamp(0f, 1f);
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        /// <summary>
        /// Floyd算法求最短路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="getDistance"></param>
        /// <returns></returns>
        public static int[,] Floyd<T>(this T[] datas) where T : IFloydNode
        {
            int[,] result = new int[datas.Length, datas.Length];
            float[,] cost = new float[datas.Length, datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                for (int j = i; j < datas.Length; j++)
                {
                    cost[i, j] = datas[i].GetDistance(datas[j]);
                    cost[j, i] = cost[i, j];
                    result[i, j] = i;
                    result[j, i] = j;
                }
            }

            for (int k = 0; k < datas.Length; k++)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    for (int j = 0; j < datas.Length; j++)
                    {
                        float d = cost[i, k] + cost[k, j];
                        if (d < cost[i, j])
                        {
                            cost[i, j] = d;
                            result[i, j] = k;
                        }
                    }
                }
            }
            return result;
        }

        public static bool TryGetFloydPath(this int[,] m_Path, int start, int end, ref List<int> route)
        {
            if(start < 0 || start >= m_Path.GetLength(0))
            {
                return false;
            }

            if (end < 0 || end >= m_Path.GetLength(0))
            {
                return false;
            }

            if (start != end)
            {
                if(!m_Path.TryGetFloydPath(start, m_Path[start, end], ref route))
                {
                    return false;
                }
            }
            route.Add(end);
            return true;
        }

        public static void StartThread(Action callback, bool isback = true)
        {
            Thread t = new Thread(o =>
            {
                callback();
            })
            {
                IsBackground = isback,
            };

            t.Start();
        }

        public static void StartThread<T>(Action<T> callback, T paramater, bool isback = true)
        {
            Thread t = new Thread(o =>
            {
                callback((T)o);
            })
            {
                IsBackground = isback,
            };

            t.Start(paramater);
        }

        //if (!string.IsNullOrEmpty(des))\n\t\t\t{\n\t\t\t\tConsole.WriteLine(string.Format("{0} elapsed time {1}ms", des,
        // Console.WriteLine\(string.Format\(\"\{0\} elapsed time \{1\}ms\", des, _stopwatch.ElapsedMilliseconds\)\);
        private static Stopwatch _stopwatch = new Stopwatch();
        public static long Runner(string des, Action callback)
        {
            _stopwatch.Restart();
            callback();
            _stopwatch.Stop();
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1>(string des, Action<T1> callback, T1 arg1)
        {
            _stopwatch.Restart();
            callback(arg1);
            _stopwatch.Stop();
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, T2>(string des, Action<T1,T2> callback, T1 arg1, T2 arg2)
        {
            _stopwatch.Restart();
            callback(arg1, arg2);
            _stopwatch.Stop();
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, T2, T3>(string des, Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            _stopwatch.Restart();
            callback(arg1, arg2, arg3);
            _stopwatch.Stop();
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, T2, T3, T4>(string des, Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _stopwatch.Restart();
            callback(arg1, arg2, arg3, arg4);
            _stopwatch.Stop();
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, T2, T3, T4, T5>(string des, Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _stopwatch.Restart();
            callback(arg1, arg2, arg3, arg4, arg5);
            _stopwatch.Stop();
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<TResult>(string des, Func<TResult> callback, out TResult result)
        {
            _stopwatch.Restart();
            TResult ret = callback();
            _stopwatch.Stop();
            result = ret;
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, TResult>(string des, Func<T1, TResult> callback, out TResult result, T1 arg1)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1);
            _stopwatch.Stop();
            result =  ret;
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1,T2,TResult>(string des, Func<T1,T2,TResult> callback, out TResult result, T1 arg1, T2 arg2)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2);
            _stopwatch.Stop();
            result =  ret;
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, T2, T3, TResult>(string des, Func<T1, T2, T3, TResult> callback, out TResult result, T1 arg1, T2 arg2, T3 arg3)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2, arg3);
            _stopwatch.Stop();
            result = ret;
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, T2, T3, T4, TResult>(string des, Func<T1, T2, T3, T4, TResult> callback, out TResult result,T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2, arg3, arg4);
            _stopwatch.Stop();
            result =  ret;
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }

        public static long Runner<T1, T2, T3, T4, T5, TResult>(string des, Func<T1, T2, T3, T4, T5, TResult> callback, out TResult result, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2, arg3, arg4, arg5);
            _stopwatch.Stop();
            result = ret;
            if (!string.IsNullOrEmpty(des))
			{
				Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds)); 
			}
			return _stopwatch.ElapsedMilliseconds;
        }
    }
}
