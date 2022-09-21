using LinqDBModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyLiinqOperators
{
    class Program
    {
        static void Main(string[] args)
        {
            // 序列 --> 序列
            {
                // 筛选运算符
                // where Take TakeWhile Skip SkipWhile Distinct
                {
                    string[] names = { "Tom", "Dick", "Harry", "Mary", "Jay" };

                    var query = from n in names
                                where n.Length > 3
                                let u = n.ToUpper() // let:引入变量作用域
                                where u.EndsWith("Y")
                                select u;

                    var query2 = names.Where((n, i) => i % 2 == 0); // 索引筛选 仅适用本地查询 

                    int[] numbers = { 3, 5, 2, 234, 4, 1 };
                    numbers.TakeWhile(n => n < 100);
                    numbers.SkipWhile(n => n < 100);

                }


                // 映射运算符
                // Select SelectMany
                {
                    {
                        // 返回 “我的文档” 下所有目录的描述，而每个子集合包含相应目录下的文件
                        string sampleDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        DirectoryInfo[] dirs = new DirectoryInfo(sampleDirectory).GetDirectories();

                        // 关联子查询：子查询引用外部查询对象
                        var query =
                            from d in dirs
                            where (d.Attributes & FileAttributes.System) == 0
                            select new
                            {
                                DirectoryName = d.FullName,
                                Created = d.CreationTime,
                                Files = from f in d.GetFiles()  // 此处引用 外部 查询变量 
                                        where (f.Attributes & FileAttributes.Hidden) == 0
                                        select new { FileName = f.Name, f.Length, }
                            };

                        // Here's how to enumerate the results manually:

                        foreach (var dirFiles in query)
                        {
                            Console.WriteLine("Directory: " + dirFiles.DirectoryName);
                            foreach (var file in dirFiles.Files)
                                Console.WriteLine("    " + file.FileName + "Len: " + file.Length);
                        }

                        NutshellEntities dataContext = new NutshellEntities();
                        // 检索每个客户的姓名及其 大额购买记录
                        // 外连接：不论客户有没有购买记录，所有的客户都会被查询出来
                        var query2 = from c in dataContext.Customer
                                     select new
                                     {
                                         c.Name,
                                         Purchases = from p in c.Purchase    // 利用导航属性
                                                     where p.Price > 1000
                                                     select new { p.Description, p.Price }
                                     };
                        // 内连接：只查询大额购买的客户，其他客户不会被查询出来、
                        var query3 = from c in dataContext.Customer
                                     where c.Purchase.Any(p => p.Price > 1000)  // 增加一个筛选条件
                                     select new
                                     {
                                         c.Name,
                                         Purchases = from p in c.Purchase    // 利用导航属性
                                                     where p.Price > 1000
                                                     select new { p.Description, p.Price }
                                     };
                        // 使用let 改进 内连接
                        var query4 = from c in dataContext.Customer
                                     let highValueP = from p in c.Purchase
                                                      where p.Price > 1000
                                                      select new { p.Description, p.Price }
                                     where highValueP.Any()
                                     select new { c.Name, Purchases = highValueP };
                        // 映射自定义实体类: 将结果 映射为 【多层对象层次结构】
                        var query5 = from c in dataContext.Customer
                                     select new MyCustomerEntity
                                     {
                                         Name = c.Name,
                                         Purchases =
                                         (
                                            from p in c.Purchase
                                            where p.Price > 1000
                                            select new MyPurchaseEntity
                                            {
                                                Description = p.Description,
                                                Price = p.Price
                                            }
                                         ).ToList()
                                     };
                        List<MyCustomerEntity> result = query5.ToList();
                    }

                    {
                        // SelectMany 返回平面结果集 Select返回层次化结果集
                        // 如果实体类为特定关系定义了关联属性，那么可以扩展该子集合（非筛选交叉连接），实现关联查询效果
                        string[] fullNames = { "Anne Williams", "John Fred Smith", "Sue Green" };
                        var query1 = fullNames.SelectMany(n => n.Split());
                        var query2 = fullNames.SelectMany(n => n.Split().Select(n2 => new { n2, n }));
                        var query3 = fullNames.SelectMany(n => n.Split(), (n, n2) => new { n, n2 });

                        IEnumerable<string> query4 =
                                from fullName in fullNames
                                from x in fullName.Split().Select(name => new { name, fullName })
                                orderby x.fullName, x.name
                                select x.name + " came from " + x.fullName;


                        IEnumerable<string> query5 = fullNames
                            .SelectMany(fName => fName.Split().Select(name => new { name, fName }))
                            .OrderBy(x => x.fName)
                            .ThenBy(x => x.name)
                            .Select(x => x.name + " came from " + x.fName);

                        // 在 LINQ to SQL 和 EF 中使用 “SelectMany”
                        var players = new[] { "Tom", "Jay", "Mary" }.AsQueryable();

                        IEnumerable<string> query6 =
                            from name1 in players
                            from name2 in players
                            where name1.CompareTo(name2) < 0
                            orderby name1, name2
                            select name1 + " vs " + name2;
                        
                        NutshellEntities dataContext = new NutshellEntities();
                        // 内连接
                        var query7 = from c in dataContext.Customer
                                     from p in c.Purchase
                                     where p.Price > 1000
                                     from pi in p.PurchaseItem
                                     select new { c.Name, p.Description, p.Price ,pi.Detail};

                        // 外连接
                        var query8 = from c in dataContext.Customer
                                     from p in c.Purchase.Where(p => p.Price > 1000).DefaultIfEmpty()
                                     //from pi in p.PurchaseItem.DefaultIfEmpty()
                                     select new
                                     {
                                         c.Name,
                                         Description = p == null ? null : p.Description,
                                         Price = p == null ? (decimal?)null : p.Price,
                                      //   Detail = pi == null ? null : pi.Detail
                                     };
                                     
                                    
                    }

                }

                // 连接运算符
                // Join GroupJoin Zip
                {
                    {
                        { 
                        // Join GroupJoin 将两个输入序列连接为一个输出序列，Join 生成平面的输出，GroupJoin 产生层次化的输出
                        NutshellEntities dataContext = new NutshellEntities();
                        var query1 = from c in dataContext.Customer
                                     join p in dataContext.Purchase
                                     on c.ID equals p.CustomerID
                                     select c.Name + " bought a " + p.Description;

                        Customer[] localCustomers = dataContext.Customer.ToArray(); // 转化到本地
                        Purchase[] localPurchases = dataContext.Purchase.ToArray();
                        // 【SelectMany】  vs. 【Join】  
                        var slowQuery2 = from c in localCustomers
                                             //from p in c.Purchase where c.ID == p.CustomerID
                                         from p in localPurchases where c.ID == p.CustomerID
                                         select c.Name + " bought a " + p.Description;
                        var fastQuery3 = from c in localCustomers   // 外部序列 localCustomers
                                         join p in localPurchases   // 内部序列 localPurchases
                                         on c.ID equals p.CustomerID
                                         select c.Name + " bought a " + p.Description;
                        // Join 在流式语法中的使用
                        var query4 = localCustomers.Join(localPurchases,
                                            c => c.ID,
                                            p => p.CustomerID,  // IDE 不会给出提示
                                            (c, p) => new { c.Name, p.Description, p.Price }
                            );
                        var query5 = localCustomers.Join(localPurchases,
                                            c => c.ID,
                                            p => p.CustomerID,
                                            (c, p) => new { c, p })
                                            .OrderBy(x => x.p.Price)
                                            .Select(x => x.c.Name + " bought a " + x.p.Description);
                    }
                        {
                            // GroupJoin (join + into) 返回外部元素分组的层次化结果结果
                            NutshellEntities dataContext = new NutshellEntities();
                            IEnumerable<IEnumerable<Purchase>> query1 =
                                                                    from c in dataContext.Customer
                                                                    join p in dataContext.Purchase
                                                                    on c.ID equals p.CustomerID
                                                                    into custPurchases
                                                                    select custPurchases;
                            // 左外连接(层次)
                            var query2 =
                                       from c in dataContext.Customer
                                       join p in dataContext.Purchase
                                       on c.ID equals p.CustomerID
                                       into custPurchases
                                       select new { CustName = c.Name, custPurchases };
                            // 内连接 查询仅有购买记录的客户 
                            var query3 = from c in dataContext.Customer
                                         join p in dataContext.Purchase.Where(p => p.Price > 1000)
                                         on c.ID equals p.CustomerID
                                         into custPurchases
                                         where custPurchases.Any()
                                         select new { CustName = c.Name, custPurchases };

                            // 左外连接(平面) DefaultIfEmpty 需要有允许为 null 的地方
                            var query4 = from c in dataContext.Customer
                                         join p in dataContext.Purchase
                                         on c.ID equals p.CustomerID
                                         into custPurchases
                                         from pg in custPurchases.DefaultIfEmpty()
                                         select new
                                         {
                                             CustName = c.Name,
                                             Description = (pg == null) ? null : pg.Description,
                                             Price = (pg == null) ? (decimal?)null : pg.Price
                                         };
                            // 连接查找表
                            // 将所有的购买记录都加载进查找表中
                            ILookup<int?, Purchase> purchLookup =
                                        dataContext.Purchase.ToLookup(p => p.CustomerID, p => p);
                            var query5 = from c in dataContext.Customer
                                         from p in purchLookup[c.ID]
                                         select new { c.Name, p.Description, p.Price };
                            // + DefaultIfEmpty --> 外连接
                            var query6 = from c in dataContext.Customer
                                         from p in purchLookup[c.ID].DefaultIfEmpty()
                                         select new
                                         {
                                             CustName = c.Name,
                                             Description = (p == null) ? null : p.Description,
                                             Price = (p == null) ? (decimal?)null : p.Price
                                         };
                        }
                        {
                            // Zip 同时枚举两个集合中的元素(像拉链一下)
                            int[] numbers = { 3, 5, 7 };
                            string[] words = { "three", "five", "seven", "none" };
                           IEnumerable<string> zip =   numbers.Zip(words, (n, w) => n + "=" + w);
                        }

                    }
                }

                // 排序运算符
                // Ordery ThenBy Reverse
                {
                    var names = new[] { "Tom", "Dick", "Harry", "Mary", "Jay" }.AsQueryable();

                    // By length, then alphabetically
                    var query1 = names.OrderBy(s => s.Length).ThenBy(s => s);

                    // By length, then second character, then first character
                    var query2 =   names.OrderBy(s => s.Length).ThenBy(s => s[1]).ThenBy(s => s[0]);

                    var query3 =
                        from s in names
                        orderby s.Length, s[1], s[0]
                        select s;

                    NutshellEntities dataContext = new NutshellEntities();
                    var query4 =  dataContext.Purchase
                    .OrderByDescending(p => p.Price)
                    .ThenBy(p => p.Description);

                    var query5 = from p in dataContext.Purchase
                                 orderby p.Price descending, p.Description
                                 select p;
                    // IComparer
                    var query6 = names.OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase);

                    var query7 =
                                    from c in dataContext.Customer
                                    orderby c.Name.ToUpper()
                                    select c.Name;
                    // IOrderedEnumerable IOrderedQueryable
                    IOrderedEnumerable<string> query8 = names.AsEnumerable().OrderBy(s => s.Length);
                    IOrderedEnumerable<string> query9 = query8.ThenBy(s => s);

                    var query10 = names.OrderBy(s => s.Length).AsEnumerable();
                    query10 = query10.Where(n => n.Length > 3);

                }

                // 分组运算符
                // GroupBy
                {
                    string[] files = Directory.GetFiles(Path.GetTempPath()).Take(100).ToArray();

                    IEnumerable<IGrouping<string, string>> query1 =
                        files.GroupBy(file => Path.GetExtension(file));

                    foreach (IGrouping<string, string> grouping in query1)
                    {
                        Console.WriteLine("Extension: " + grouping.Key);
                        foreach (string filename in grouping)
                            Console.WriteLine("   - " + filename);
                    }

                    // 指定 elementSelector 参数对输入元素处理，但与 排序主键选择器 keySelector 无关
                    var query2 =  files.GroupBy(file => Path.GetExtension(file), file => file.ToUpper());

                    var query3 = from file in files
                                 group file.ToUpper() by Path.GetExtension(file);

                    // + into --> 对分组结果 筛选
                    var query4 = from file in files
                                 group file.ToUpper() by Path.GetExtension(file) 
                                 into grouping
                                 where grouping.Count() < 5
                                 select grouping;
                    // 分组求和: 分组统计一年销售额
                    NutshellEntities dataContext = new NutshellEntities();
                    var query5 = from p in dataContext.Purchase
                                 group p.Price by p.Date.Year
                                 into salesByYear
                                 select new
                                 {
                                     Year = salesByYear.Key,
                                     TotalPrice = salesByYear.Sum()
                                 };
                    // 对多个键进行分组
                    var query6 = from n in new[] { "Tom", "Dick", "Harry", "Mary", "Jay" }.AsQueryable()
                                 group n by new
                                 {
                                     FirstLetter = n[0],
                                     Length = n.Length
                                 };
                    // The following groups purchases by year, then returns only those groups where
                    // the average purchase across the year was greater than $1000:

                    var query7 = from p in dataContext.Purchase
                                 group p.Price by p.Date.Year into salesByYear
                                 where salesByYear.Average(x => x) > 1000
                                 select new
                                 {
                                     Year = salesByYear.Key,
                                     TotalSales = salesByYear.Count(),
                                     AvgSale = salesByYear.Average(),
                                     TotalValue = salesByYear.Sum()
                                 };
                    
                }

                // 集合运算符
                // Concat Union Intersect Except
                {
                    int[] seq1 = { 1, 2, 3 }, seq2 = { 3, 4, 5 };

                    // 合集
                   IEnumerable<int> concat =  seq1.Concat(seq2);
                   // 合集 不含共有
                    IEnumerable<int> union   = seq1.Union(seq2);
                    // 交接
                    seq1.Intersect(seq2);
                    // 补集
                    seq1.Except(seq2);
                    seq2.Except(seq1);
                    var query1 = from s1 in seq1
                                 where !seq2.Contains(s1)
                                 select s1;


                }

                // 转换方方法：导入，导出
                // OfType Cast , ToArray ToList ToDictionary ToLookup AsEnumerable AsQueryable
                {
                    // Cast 兼容类型，抛异常     OfType 忽略不兼容类型
                    ArrayList classicList = new ArrayList();
                    classicList.AddRange(new int[] { 3, 4, 5 });

                    DateTime offender = DateTime.Now;
                    classicList.Add(offender);

                    IEnumerable<int>
                        ofTypeSequence = classicList.OfType<int>(),
                        castSequence = classicList.Cast<int>();

                    // ofTypeSequence.Dump("Notice that the offending DateTime element is missing");

                    try
                    {
                        castSequence.ToList();
                    }
                    catch (InvalidCastException ex)
                    {
                       // ex.Message.Dump("Notice what the offending DateTime element does to the Cast sequence");
                    }
                }
            }
            // 序列 --> 元素或标量值
            {
                // 元素运算符
                // Fisrt FirstOfDefault Last LastOfDefault Single SingleOrDefault ElementAt DefaultIfEmpty
                {
                    // DefaultIfEmpty
                    // 输入序列为空时，返回一个default元素列表，否则返回原始输入序列
                    // 实现展平结果的外连接
                }
                // 聚合方法
                // Aggregate Average Count LongCount Sum Max Min
                {
                    // Average
                    // 查询返回了平均购买记录大于＄500的用户
                    NutshellEntities dataContext = new NutshellEntities();
                    var query1 = from c in dataContext.Customer
                                 where c.Purchase.Average(p => p.Price) > 500
                                 select c;
                    // Aggregate
                    int[] numbers = { 1, 2, 3,4 };
                    int sum1 = numbers.Aggregate(0, (total, n) => total + n);
                    int sum2 = numbers.Select(n => n * n).Aggregate((total, n) => total + n);

                }
                // 量词运算符
                // All Any Contains SequenceEqual
                {
                    int[] numbers = { 1, 2, 3, 4 };
                    bool hasAThree = numbers.Contains(3);
                    bool hasABiggerThree = numbers.Any(n => n > 3);
                    NutshellEntities dataContext = new NutshellEntities();

                    IQueryable<Customer> query1 = dataContext.Customer.Where(c => c.Purchase.All(p => p.Price < 100));
                    var query2 = "Hello".Distinct();
                    query2.SequenceEqual("Helo");
                }

            }
            // Void --> 序列
            {
                // 生成集合方法
                // Empty Rnage Repeatxdd
                {
                    int[][] numbers =
                    {
                      new int[] { 1, 2, 3 },
                      new int[] { 4, 5, 6 },
                      null                     // this null makes the query below fail.
                    };

                    IEnumerable<int> flatExp = numbers.SelectMany(innerArray => innerArray);
                    IEnumerable<int> flat = numbers.SelectMany(innerArray => innerArray ?? Enumerable.Empty<int>());
                    Enumerable.Range(5, 5);
                    Enumerable.Repeat(true, 3);
                }
            }

            Console.ReadLine();
        }
    }
}
