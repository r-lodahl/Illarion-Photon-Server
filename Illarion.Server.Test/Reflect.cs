using System.Linq.Expressions;
using System.Reflection;

public delegate void Action<A, B, C, D, E>
(A a, B b, C c, D d, E e);

public delegate void Action<A, B, C, D, E, F>
(A a, B b, C c, D d, E e, F f);

public delegate void Action<A, B, C, D, E, F, G>
(A a, B b, C c, D d, E e, F f, G g);

public delegate void Action<A, B, C, D, E, F, G, H>
(A a, B b, C c, D d, E e, F f, G g, H h);

public delegate void Action<A, B, C, D, E, F, G, H, I>
(A a, B b, C c, D d, E e, F f, G g, H h, I i);

public static partial class Reflect
{
    static FieldInfo GetFieldInfo(LambdaExpression lambda) => (FieldInfo)GetMemberInfo(lambda);

    static PropertyInfo GetPropertyInfo(LambdaExpression lambda) => (PropertyInfo)GetMemberInfo(lambda);

    static MemberInfo GetMemberInfo(LambdaExpression lambda) => ((MemberExpression)lambda.Body).Member;

    static MethodInfo GetMethodInfo(LambdaExpression lambda) => ((MethodCallExpression)lambda.Body).Method;
}