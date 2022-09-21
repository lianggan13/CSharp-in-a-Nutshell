using Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            // 反射：在运行时检查并使用元数据和编译代码的操作
            // 例如，反射获得类型和成员列表，使用类型名称字符串实例化一个对象；在运行时构建程序集

            // 19.1 反射和激活类型
            {
                // 19.1.1 反射类型
                {
                    // 获取类型
                    {
                        Type t1 = DateTime.Now.GetType();
                        Type t2 = typeof(DateTime);

                        Type t3 = Assembly.GetExecutingAssembly().GetType("System.String");
                        // mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
                        Type t4 = Type.GetType("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

                        // GetTypeInfo()
                        Type stringType = typeof(string);
                        string name = stringType.Name;
                        string name2 = stringType.GetTypeInfo().Name;
                        Type baseType = stringType.BaseType;

                        Type simpleArrayType = typeof(int).MakeArrayType();
                        Console.WriteLine(simpleArrayType == typeof(int[]));

                        Type e = typeof(int[]).GetElementType();    // e == typeof(int)
                        int rank = typeof(int[,,]).GetArrayRank();  // 3

                        // 获取嵌套类型
                        foreach (Type t in typeof(Environment).GetNestedTypes())
                            Console.WriteLine(t.FullName);

                        Point p = new Point(3, 4);
                        string sqn = p.GetType().AssemblyQualifiedName;
                        Type t5 = Type.GetType(sqn);

                        Type t6 = typeof(Dictionary<,>);
                        Console.WriteLine(t6.Name);
                        Console.WriteLine(t6.FullName);
                        Console.WriteLine(typeof(Dictionary<int, string>).FullName);

                        // ref out 参数类型
                        Type t7 = typeof(bool);
                        Console.WriteLine(t7.Name);
                        Type t8 = typeof(bool).GetMethod("TryParse").GetParameters()[1].ParameterType;
                        Console.WriteLine(t8.Name);
                    }
                    // 类型比较
                    {
                        Type base1 = typeof(System.String).BaseType;
                        foreach (Type iType in typeof(Guid).GetInterfaces())
                            Console.WriteLine(iType.Name);
                        object obj = Guid.NewGuid();
                        Type target = typeof(IFormattable);
                        bool isTrue = obj is IFormattable;
                        bool alsoTrue = target.IsInstanceOfType(obj);
                        Console.WriteLine(target.IsAssignableFrom(obj.GetType()));
                    }
                    // 实例化类型
                    {
                        //  int DateTime
                        int i = (int)Activator.CreateInstance(typeof(int));
                        DateTime dt = (DateTime)Activator.CreateInstance(typeof(DateTime), 2020, 5, 29);

                        // List<> --- 未绑定 List<T> --- 封闭
                        Type closed1 = typeof(List<int>);
                        List<int> list = (List<int>)Activator.CreateInstance(closed1);
                        Type unbound = typeof(List<>);
                        try { object anError = Activator.CreateInstance(unbound); } catch { }
                        Type closed2 = unbound.MakeGenericType(typeof(int));
                        ConstructorInfo ci = typeof(List<int>).GetConstructor(new[] { typeof(int) });
                        object foo = ci.Invoke(new object[] { 3 });

                        // Nuallble<>
                        Type nullable = typeof(bool?);
                        Console.WriteLine(
                            nullable.IsGenericType &&
                            nullable.GetGenericTypeDefinition() == typeof(Nullable<>));
                        Console.WriteLine(closed2.GetGenericArguments()[0]);
                        Console.WriteLine(nullable.GetGenericArguments()[0]);




                        Delegate staticD = Delegate.CreateDelegate(
                                typeof(IntFunc), typeof(Program), "Square");
                        Delegate instanceD = Delegate.CreateDelegate(
                            typeof(IntFunc), new Program(), "Cuble");
                        Console.WriteLine(staticD.DynamicInvoke(3));
                        Console.WriteLine(instanceD.DynamicInvoke(3));
                        IntFunc intFunc = staticD as IntFunc;
                        Console.WriteLine(intFunc(3));
                    }

                }
            }
            // 19.2 反射调用成员
            {
                MemberInfo[] members1 = typeof(Walant).GetMembers();
                foreach (MemberInfo m in members1) Console.WriteLine(m);

                IEnumerable<MemberInfo> members2 = typeof(Walant).GetTypeInfo().DeclaredMembers;

                MemberInfo program = typeof(Program).GetMethod("ToString");
                MemberInfo obj = typeof(object).GetMethod("ToString");
                Console.WriteLine(program.DeclaringType);   // 重写方法 DeclaringType 返回基类型
                Console.WriteLine(obj.DeclaringType);
                Console.WriteLine(program.ReflectedType);   // 重写方法 ReflectedType 返回子类型
                Console.WriteLine(obj.ReflectedType);
                Console.WriteLine(program == obj);
                Console.WriteLine(program.MetadataToken == obj.MetadataToken
                                && program.Module == obj.Module);

                Console.WriteLine(MethodBase.GetCurrentMethod().DeclaringType); // 返回当前正在执行的方法+

                // 获取后端方法
                PropertyInfo pi = typeof(Console).GetProperty("Title");
                MethodInfo getter = pi.GetGetMethod();
                MethodInfo setter = pi.GetSetMethod();
                MethodInfo[] both = pi.GetAccessors();

                // 未绑定类型的泛型类型成员是永远无法动态调用的
                PropertyInfo unbound = typeof(IEnumerator<>).GetProperty("Current");
                PropertyInfo closed = typeof(IEnumerator<int>).GetProperty("Current");
                Console.WriteLine(unbound);
                Console.WriteLine(closed);
                Console.WriteLine(unbound.PropertyType.IsGenericParameter);
                Console.WriteLine(closed.PropertyType.IsGenericParameter);

                PropertyInfo unbound2 = typeof(List<>).GetProperty("Count");
                PropertyInfo closed2 = typeof(List<int>).GetProperty("Count");
                Console.WriteLine(unbound2);
                Console.WriteLine(closed2);

                // 索引并调用泛型方法
                int[] source = { 3, 4, 5, 6, 7, 8 };
                Func<int, bool> predicate = n => n % 2 == 1;
                var sourceExpr = Expression.Constant(source);
                var predicateExpr = Expression.Constant(predicate);
                var callExpression = Expression.Call(
                    typeof(Enumerable), "Where",
                    new[] { typeof(int) },
                    sourceExpr, predicateExpr);

                // 调用未知类型的泛型接口成员
                // Console.WriteLine((new List<int> { 5, 6, 7 }).ToStringEx());
                Console.WriteLine("xyyzzz".GroupBy(c => c).ToStringEx());


                // 动态调用成员
                object s = "Hello";
                PropertyInfo prop = s.GetType().GetProperty("Length");
                int length = (int)prop.GetValue(s, null);

                // 在调用就 GetMethod 时显示指定参数类型可以避免重载方法的二义性
                Type type = typeof(string);
                Type[] paramsType = { typeof(int) }; // 形参类型
                MethodInfo method = type.GetMethod("Substring", paramsType);   // 获取 string 类型的方法 Substring
                object[] paramsValue = { 2 };      // 形参值
                object returnValue = method.Invoke("123456", paramsValue);      // 通过 Invoke 传入参数 执行方法
                ParameterInfo[] paramList = method.GetParameters();
                foreach (ParameterInfo p in paramList)                         // 返回方法 参数类型 的 元信息
                {
                    Console.WriteLine(p.Name);
                    Console.WriteLine(p.ParameterType);
                }

                int x, y = 0;
                bool successfulParse = int.TryParse("12", out x);
                object[] paramsValue2 = { "12", y };
                Type[] paramsType2 = { typeof(string), typeof(int).MakeByRefType() };
                MethodInfo tryParse = typeof(int).GetMethod("TryParse", paramsType2);
                bool successfulParse2 = (bool)tryParse.Invoke(null, paramsValue2);
                Console.WriteLine(successfulParse + " " + paramsValue2[1]);



                // 使用委托提高性能
                MethodInfo trimMethod = typeof(string).GetMethod("Trim", new Type[0]);
                trimMethod.GetParameters().ToList().ForEach(p =>
                {
                    Console.WriteLine(p.Name);
                });
                object[] paramsValue3 = { "23", "12" };
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < 1000000; i++) trimMethod.Invoke("test", null);  // null : 没有参数值
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds * 1.0 / 1000 + " Milliseconds using Invoke");

                var trim = (StringToString)Delegate.CreateDelegate(typeof(StringToString), trimMethod);
                stopwatch.Restart();
                for (int i = 0; i < 1000000; i++) trim("test");
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds * 1.0 / 1000 + " Milliseconds using delegate ");

                // 访问非共有成员
                BindingFlags nonPublicBinding = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                MemberInfo[] members = typeof(object).GetMembers(nonPublicBinding);
            }
            // 19.3 反射程序集
            {
                Assembly exeasb = Assembly.GetExecutingAssembly();
                foreach (Type t in exeasb.GetTypes()) Console.WriteLine(t);
                foreach (Module m in exeasb.GetModules()) Console.WriteLine(m.Name);

            }
            // 19.4 特性反射：位映射特性，自定义特性，伪自定义特性
            {
                // 位映射特性
                TypeAttributes ta = typeof(Console).Attributes;
                MethodAttributes ma = MethodInfo.GetCurrentMethod().Attributes;
                Console.WriteLine(ta + "\r\n" + ma);

                // 自定义特性
                // foreach (MethodInfo mi in typeof(Foo).GetTypeInfo().DeclaredMethods)) { ... }
                foreach (MethodInfo mi in typeof(TestClass).GetMethods()) //运行时检索特性
                {
                    // TestAttribute at =(TestAttribute) mi.GetCustomAttribute(typeof(TestAttribute));
                    TestAttribute attr = (TestAttribute)Attribute.GetCustomAttribute(mi, typeof(TestAttribute));
                    if (attr != null)
                        Console.WriteLine("Method {0} will be tested; reps={1};msg={2}",
                                            mi.Name, attr.Repetitions, attr.FailureMessage);
                }
                // 使用自定义特性
                foreach (MethodInfo mi in typeof(TestClass).GetMethods())
                {
                    TestAttribute attr = (TestAttribute)Attribute.GetCustomAttribute(mi, typeof(TestAttribute));
                    if (attr != null)
                        for (int i = 0; i < attr.Repetitions; i++)
                        {
                            try
                            {
                                mi.Invoke(new TestClass(), null); //  调用类中方法 （不带参数的方法）
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Error: " + attr.FailureMessage, ex);
                            }
                        }
                }
                object[] atts = Attribute.GetCustomAttributes(typeof(TestClass));
                foreach (object att in atts) Console.WriteLine(att);
            }
            // 19.5 使用 DynamicMethod 生成代码 + IL 评估栈 
            {
                // （具体参见 《C# 7.0 核心技术指南 章节19.5）
            }
            // 19.6 生成程序集和类型
            {
                // 19.1.1 程序集 --> 模块.cs --> 类型class 成员
                AppDomain appDomain = AppDomain.CurrentDomain;
                AssemblyName aname = new AssemblyName("MyDynamicAssembly");
                AssemblyBuilder assemblyBuilder =
                        appDomain.DefineDynamicAssembly(aname, AssemblyBuilderAccess.RunAndCollect);
                ModuleBuilder modBuilder = assemblyBuilder.DefineDynamicModule("DynModule");
                TypeBuilder tb = modBuilder.DefineType("Widget", TypeAttributes.Public); // public class Widget
                MethodBuilder methBuilder = tb.DefineMethod("SayHello", MethodAttributes.Public, null, null); // public void SayHello(){ Console.WriteLine("Hello world!");return;}
                ILGenerator gen = methBuilder.GetILGenerator();
                gen.EmitWriteLine("Hello world!");
                gen.Emit(OpCodes.Ret);

                Type t = tb.CreateType();  // 类型一旦 创建，其定义就确定下来，无法对类型再进行添加或更改操作
                object o = Activator.CreateInstance(t);      // 反射获取信息，执行动态绑定
                t.GetMethod("SayHello").Invoke(o, null);
                // tb.GetMethod("SayHello").Invoke(null, null); // 静态成员的Invoke 第一个参数 才可以 为 null
                
                // 19.6.2 Reflection.Emit 对象模型
                             
            }
            // 19.7 生成类型和成员
            {
                // 19.7.1 生成方法(静态、实例、ref out参数传递、实例、重写)

                // 19.7.2 生成字段和属性

                // 19.7.3 生成构造器 调用构造器

                // 19.7.4 附加特性

            }
            // 19.8 生成泛型方法和类型
            { 
                // 19.8.1 定义泛型方法

                // 19.8.2 定义泛型类型
            }
            // 19.9 复杂的生成目标
            {
                // 19.9.1 生成含有封闭泛型类型的方法
            }

            Console.Read();

        }
        delegate int IntFunc(int x);
        delegate string StringToString(string s);

        static int Square(int x) { return x * x; }
        int Cuble(int x) { return x * x * x; }

    }
    class Walant
    {
        private bool cracked;
        public void Crack() { cracked = true; }

        public override string ToString()
        {
            return cracked.ToString();
        }
    }

    /// <summary>
    /// 单元测试特性
    /// </summary>  
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class TestAttribute : Attribute
    {
        /// <summary>
        /// 重复次数
        /// </summary>
        public int Repetitions;
        /// <summary>
        /// 失败信息
        /// </summary>
        public string FailureMessage;
        public TestAttribute() : this(1) { }
        public TestAttribute(int repetitions) { Repetitions = repetitions; }
    }

    /// <summary>
    /// 附加单元测试特性
    /// </summary>
    [Serializable, Obsolete]
    public class TestClass
    {
        [TestAttribute]
        public void Method1() { Console.WriteLine("Method1"); }
        [TestAttribute(13)]
        public void Method2() { Console.WriteLine("Method2"); }
        [TestAttribute(20, FailureMessage = "I'm a loser!")]
        public void Method3() { Console.WriteLine("Method3"); }
    }
}
