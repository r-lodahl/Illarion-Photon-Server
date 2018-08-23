using System;
using System.Linq.Expressions;
using System.Reflection;

public static partial class Reflect
{
    public static class Instance<TClass>
    {
        public static FieldInfo Field<T>
        (Expression<Func<TClass, T>> m) => GetFieldInfo(m);

        public static PropertyInfo Property<T>
        (Expression<Func<TClass, T>> m) => GetPropertyInfo(m);

        public static MethodInfo Method
        (Expression<Action<TClass>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1>
        (Expression<Action<TClass, T1>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1, T2>
        (Expression<Action<TClass, T1, T2>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1, T2, T3>
        (Expression<Action<TClass, T1, T2, T3>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1, T2, T3, T4>
        (Expression<Action<TClass, T1, T2, T3, T4>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1, T2, T3, T4, T5>
        (Expression<Action<TClass, T1, T2, T3, T4, T5>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1, T2, T3, T4, T5, T6>
        (Expression<Action<TClass, T1, T2, T3, T4, T5, T6>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1, T2, T3, T4, T5, T6, T7>
        (Expression<Action<TClass, T1, T2, T3, T4, T5, T6, T7>> m) => GetMethodInfo(m);

        public static MethodInfo Method<T1, T2, T3, T4, T5, T6, T7, T8>
        (Expression<Action<TClass, T1, T2, T3, T4, T5, T6, T7, T8>> m) => GetMethodInfo(m);
    }
}