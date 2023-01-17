namespace Astar
{
    using System;
    using System.Collections.Generic;
using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

        private static Stopwatch _stopwatch = new Stopwatch();
        public static void Runner(string des, Action callback)
        {
            _stopwatch.Restart();
            callback();
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
        }

        public static void Runner<T1>(string des, Action<T1> callback, T1 arg1)
        {
            _stopwatch.Restart();
            callback(arg1);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
        }

        public static void Runner<T1, T2>(string des, Action<T1,T2> callback, T1 arg1, T2 arg2)
        {
            _stopwatch.Restart();
            callback(arg1, arg2);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
        }

        public static void Runner<T1, T2, T3>(string des, Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            _stopwatch.Restart();
            callback(arg1, arg2, arg3);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
        }

        public static void Runner<T1, T2, T3, T4>(string des, Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _stopwatch.Restart();
            callback(arg1, arg2, arg3, arg4);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
        }

        public static void Runner<T1, T2, T3, T4, T5>(string des, Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _stopwatch.Restart();
            callback(arg1, arg2, arg3, arg4, arg5);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
        }

        public static TResult Runner<TResult>(string des, Func<TResult> callback)
        {
            _stopwatch.Restart();
            TResult ret = callback();
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
            return ret;
        }

        public static TResult Runner<T1, TResult>(string des, Func<T1, TResult> callback, T1 arg1)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
            return ret;
        }

        public static TResult Runner<T1,T2,TResult>(string des, Func<T1,T2,TResult> callback, T1 arg1, T2 arg2)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
            return ret;
        }

        public static TResult Runner<T1, T2, T3, TResult>(string des, Func<T1, T2, T3, TResult> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2, arg3);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
            return ret;
        }

        public static TResult Runner<T1, T2, T3, T4, TResult>(string des, Func<T1, T2, T3, T4, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2, arg3, arg4);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
            return ret;
        }

        public static TResult Runner<T1, T2, T3, T4, T5, TResult>(string des, Func<T1, T2, T3, T4, T5, TResult> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _stopwatch.Restart();
            TResult ret = callback(arg1, arg2, arg3, arg4, arg5);
            _stopwatch.Stop();
            Console.WriteLine(string.Format("{0} elapsed time {1}ms", des, _stopwatch.ElapsedMilliseconds));
            return ret;
        }
    }
}
