using System;

namespace Tizsoft.Extensions
{
    public static class FuncExtensions
    {
        public static TResult Raise<TResult>(this Func<TResult> func)
        {
            return func != null ? func() : default(TResult);
        }

        public static TResult Raise<T, TResult>(this Func<T, TResult> func, T arg)
        {
            return func != null ? func(arg) : default(TResult);
        }

        public static TResult Raise<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            return func != null ? func(arg1, arg2) : default(TResult);
        }

        public static TResult Raise<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
        {
            return func != null ? func(arg1, arg2, arg3) : default(TResult);
        }
    }
}
