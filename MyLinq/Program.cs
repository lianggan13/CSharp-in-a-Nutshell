using MyLinq.Extension;
using MyLinq.Model;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyLinq
{
    class Program
    {
        static void Main(string[] args)
        {
            #region linq
            {  // 流式运算符
               // 查询表达式
                {
                    // 查询运算符中Lambda表达式针对的是输入序列的每一个元素，而非输入序列整体
                    // 每一个范围变量 如n,其实例都位于Lambda表达式的私有作用域中 
                    // 混合查询
                    string[] names = { "Tom", "Dick", "Harry", "Mary", "Jay" };
                    // 计算包含字母 “a” 的字符串数目
                    int num = (from n in names where n.Contains("a") select n).Count();

                    // 按照字母顺序获得第一个名字
                    string firstName = (from n in names orderby n select n).First();

                    // 单独使用流式运算符
                    num = names.Where(n => n.Contains("a")).Count();
                    firstName = names.OrderBy(n => n).First();

                    // 延续执行
                    // 原理：实例化一个装饰了输入序列的枚举类
                    // 特点: 当枚举时才会真正执行，同时重复枚举会带来重复执行，使用ToArray 或 ToList 避免重复执行

                    // 
                    Console.WriteLine("***************Linq*****************");
                    //LinqShow show = new LinqShow();
                    //show.Show();
                    int[] seq1 = { 1, 2, 3 };
                    int[] seq2 = { 3, 4, 5 };
                    IEnumerable<int> concat = seq1.Concat(seq2);    // 连接
                    IEnumerable<int> union = seq1.Union(seq2);      // 连接 并去掉重复

                }
                // 子查询
                {
                    // 获取数组中长度最短的元素
                    string[] names = {"Tom","Dick","Harry","Mary","Jay" };
                    IEnumerable<string> query1 =  names.Where(n => n.Length == names.OrderBy(n2 => n2.Length)
                                                    .Select(n2 => n2.Length).First());
                    IEnumerable<string> query2 = from n in names
                                                 where n.Length ==
                                                     (from n2 in names orderby n2.Length select  n2.Length).First()
                                                  select n;
                    IEnumerable<string> query3 = from n in names
                                                 where n.Length == names.OrderBy(n2 => n2.Length).First().Length
                                                 select n;
                    IEnumerable<string> query4 = from n in names
                                                 where n.Length ==
                                                        names.Min(n2 => n2.Length)
                                                 select n;
                    IEnumerable<string> query5 = names.Where(n => n.Length == names.Min(n2 => n2.Length));
                    // 外部查询内嵌子查询的运行效率低：子查询会随外层循环迭代重复进行计算
                    // 分别执行子查询和外部查询避免低效操作
                    int shortest = names.Min(n => n.Length);
                    IEnumerable<string> query6 = from n in names
                                                 where n.Length == shortest
                                                 select n;
                }
                // 查询构造方式
                {
                    // 1.渐进式
                    // 元音字母移除 --> 所有长度大于2的名字按照字母排序
                    string[] names = {"Tom","Dick","Harry","Mary","Jay" };
                    IEnumerable<string> query1 =  names.Select(n => Regex.Replace(n, "[aeiou]", ""))
                        .Where(n => n.Length > 2)
                        .OrderBy(n => n);

                    // 2.into关键字
                    // 触发继续查询 或 触发GroupJoin
                    IEnumerable<string> query2 = from n in names
                                                 select Regex.Replace(n, "[aeiou]", "")
                                                   into n2
                                                 where n2.Length > 2
                                                 orderby n2
                                                 select n2;
                    // 3.包装
                    IEnumerable<string> query3 = from n1 in
                                                    (
                                                        from n2 in names
                                                        select Regex.Replace(n2, "[aeiou]", "")

                                                   )
                                                 where n1.Length > 2
                                                 orderby n1
                                                 select n1;
                  

                }
                // 映射方式
                {
                    // 1.Select new 匿名类型
                    // 元音字母移除 --> 找出所有长度大于2的名字 --> 将原始字母排序
                    string[] names = { "Tom", "Dick", "Harry", "Mary", "Jay" };
                    IEnumerable<string> query1 = from n in names
                                                 select new
                                                 {
                                                     Original = n,
                                                     Vowelles = Regex.Replace(n, "[aeiou]", "")
                                                 }
                                                 into temp
                                                 where temp.Vowelles.Length > 2
                                                 orderby temp.Original
                                                 select temp.Original;
                    // 2.let 在查询中定义新常量，新常量与范围变量 并存
                    IEnumerable<string> query2 = from n in names
                                                 let vowlles = Regex.Replace(n, "[aeiou]", "")
                                                 where vowlles.Length > 2
                                                 orderby n
                                                 select n;
                }
                ////////////////////////////////////////////
                ///
                // 解释型查询
                {
                    // IQuerybale
                    // L2S EF
                    // DataContext 与 ObjectContext 不是线程安全的，在多层应用中，中间层方法应当为每个客户端请求创建一个全新的上下文对象，【分散】多个请求【同时】对数据库服务器的更新
                    
                    {

                        // 1.LINQ to SQL (L2S)
                        DataContext dataContext = new DataContext("连接数据库字符串");
                        Table<Customer> customers = dataContext.GetTable<Customer>();
                        IQueryable<string> query = from c in customers
                                                   where c.Name.Contains("a")
                                                   orderby c.Name.Length
                                                   select c.Name.ToUpper();
                        foreach (string name in query) Console.WriteLine(name);
                        // 综合使用解释型查询 和 本地查询
                        // 在外部得到本地查询，其数据源于内侧的解释型查询
                        IEnumerable<string> query1 = customers.Select(c => c.Name.ToUpper())
                                                    .OrderBy(n => n)
                                                    .Pair()         // 此处开始 本地查询
                                                    .Select((n, i) => "Pari " + i + " = " + n);
                        foreach (string name in query) Console.WriteLine(name);
                        // AsEnumerable<T> 将远程查询 IQueryable<T> 转化为 本地查询 IEnumerable<T>

                        // 提交
                        Customer cust = customers.OrderBy(c => c.ID).First();
                        cust.Name = "lianggan13";
                        dataContext.SubmitChanges();
                        // 关闭上下文追踪
                        dataContext.ObjectTrackingEnabled = false;

                        // 提前设置筛选条件
                        // AssociateWith 筛选特定关系的检索对象
                        DataLoadOptions options = new DataLoadOptions();
                        options.AssociateWith<Customer>(c => c.Purchases.Where(p => p.Price > 1000));
                        dataContext.LoadOptions = options;

                        // 立即加载
                        // LoadWith 在父实体加载时，也会立即加载其上的关系子体
                        options.LoadWith<Customer>(c => c.Purchases);
                        dataContext.LoadOptions = options;
                        foreach (Customer c in customers) // context.Customers
                        {
                            foreach (Purchase p in c.Purchases)
                            {
                                Console.WriteLine(c.Name + " bought a " + p.Desccription);
                            }
                        }
                    }

                    {

                        // 2.EF (Entity Data Model)
                        ObjectContext objectContext = new ObjectContext("实体连接字符串");
                        objectContext.DefaultContainerName = "LgEntities";
                        ObjectSet<Customer> customers = objectContext.CreateObjectSet<Customer>();
                        Customer cust = customers.Single(c => c.ID == 2);
                        // 提交
                        cust.Name = "lianggan13";
                        objectContext.SaveChanges();

                        // 关闭上下文追踪
                        customers.MergeOption = MergeOption.NoTracking;

                        // 立即加载
                        // Include
                        foreach (Customer c in customers.Include("Purchases"))
                        {
                            foreach (Purchase p in c.Purchases)
                            {
                                Console.WriteLine(p.Desccription);
                            }
                        } 

                    }
                    {

                        // 委托与表达式树
                        // 本地查询使用 Enumerable 运算符，接受委托
                        // 解释型查询使用 Queryable 运算符，接受表达式树
                        var dataContext = new DataContext("数据库连接字符串");
                        var products =  dataContext.GetTable<Product>();
                        
                        IQueryable<Product>
                            sqlQuery = products.Where(Product.IsSelling());
                        IEnumerable<Product>
                            localQuery = products.AsEnumerable().Where(Product.IsSelling().Compile());
                    
                        // AsQueryable 将本地集合转为远程序列上执行
                    }


                }

                Console.ReadKey();
            }
            
            #endregion
        }

     
    }
}
