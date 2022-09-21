using Common;
using LinqDBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MyLinqToXML
{
    class Program
    {
        static void Main(string[] args)
        {

#if false

            // X-DOM Overview 文档对象模型 概述
            {
                XElement config = XElement.Parse(
                                @"<configuration>
	                                <client enabled='true'>
		                                <timeout>30</timeout>
	                                </client>
                                </configuration>");

                foreach (XElement child in config.Elements())
                    child.Name.Dump("Child element name");

                XElement client = config.Element("client");

                bool enabled = (bool)client.Attribute("enabled");   // Read attribute
                enabled.Dump("enabled attribute");

                client.Attribute("enabled").SetValue(!enabled);     // Update attribute

                int timeout = (int)client.Element("timeout");       // Read element
                timeout.Dump("timeout element");

                client.Element("timeout").SetValue(timeout * 2);    // Update element

                client.Add(new XElement("retries", 3));             // Add new elememt

            }
            // Instantiation X-DOM 实例化 X-DOM
            {
                // 方法 添加 元素节点 节点值 属性 熟悉值
                XElement lastName = new XElement("lastname", "Bloggs");
                lastName.Add(new XComment("nice name"));

                XElement customer = new XElement("customer");
                customer.Add(new XAttribute("id", 123));
                customer.Add(new XElement("firstname", "Joe"));
                customer.Add(lastName);

                customer.Dump();

                // Functional Construction 函数式构建
                new XElement("customer", new XAttribute("id", 123),
                            new XElement("firstname", "joe"),
                            new XElement("lastname", "bloggs",
                                new XComment("nice name")
                            )
                        );

                // Fucntional Construction from Database Query 为每个实体对象 创建 元素节点
                NutshellEntities dataContext = new NutshellEntities();
                new XElement("customers",
                    from c in dataContext.Customer
                    select new XElement("customer",
                        new XAttribute("id", c.ID),
                        new XElement("name", c.Name,
                              new XComment("nice name"))));
                // Automatic Deep Cloing 自动深度克隆
                var address =
                    new XElement("address",
                        new XElement("street", "Lawley St"),
                        new XElement("town", "North Beach")
                    );

                var customer1 = new XElement("customer1", address);
                var customer2 = new XElement("customer2", address);

                customer1.Element("address").Element("street").Value = "Another St";
                Console.WriteLine(customer2.Element("address").Element("street").Value);

            }
#elif false

            // Navigating and Querying 导航查询
            {
                // FirstNode LastNode and Nodes
                var bench =
                        new XElement("bench",
                            new XElement("toolbox",
                                new XElement("handtool", "Hammer"),
                                new XElement("handtool", "Rasp")
                            ),
                            new XElement("toolbox",
                                new XElement("handtool", "Saw"),
                                new XElement("powertool", "Nailgun")
                            ),
                            new XComment("Be careful with the nailgun")
                        );

                bench.FirstNode.Dump("FirstNode");
                bench.LastNode.Dump("LastNode");

                foreach (XNode node in bench.Nodes())
                    Console.WriteLine(node.ToString(SaveOptions.DisableFormatting) + "."); //  toolbox = HammerRasp
                                                                                           //  toolbox = SawNailgun
                // Enumerating Elements 检索元素
                foreach (XElement e in bench.Elements())
                    Console.WriteLine(e.Name + "=" + e.Value);
                // Quering Elements
                IEnumerable<string> toolboxWithNailgun = from toolbox in bench.Elements()
                                              where toolbox.Elements().Any(tool => tool.Value == "Nailgun")
                                              select toolbox.Value;
                var handTools = from toolbox in bench.Elements()
                         from tool in toolbox.Elements()
                         where tool.Name == "handtool"
                         select tool.Value;

                int toolboxCount = bench.Elements("toolbox").Count();

                var handTools2 =
                    from tool in bench.Elements("toolbox").Elements("handtool")
                    select tool.Value.ToUpper();

                toolboxWithNailgun.Dump("The toolbox with the nailgun");
                handTools.Dump("The hand tools in all toolboxes");
                toolboxCount.Dump("Number of toolboxes");
                handTools2.Dump("The hand tools in all toolboxes");

                XElement singnal = bench.Element("toolbox").Element("xyc");
                string nullableValue = singnal?.Value;

                // Quering Elements - Recursive
                // Descendants     返回所有叶子节点 并 可筛选
                // DescendantNodes 返回包含所有的父节点 和 叶子节点
                bench.Descendants("handtool").Count().Dump();
                foreach (XNode node in bench.DescendantNodes())
                    Console.WriteLine(node.ToString(SaveOptions.DisableFormatting));
                (
                    from c in bench.DescendantNodes().OfType<XComment>()
                    where c.Value.Contains("careful")
                    orderby c.Value
                    select c.Value
                ).Dump("Comments anywhere in the X-DOM containing the word 'careful'");
            }
#elif false
            // Updating X-DOM 更新 X-DOM
            {
                // SetValue Replaces Child Content
                XElement settings =
                            new XElement("settings",
                            new XElement("timeout", 30));

                settings.Dump("Original XML");
                settings.SetValue("blah");
                settings.Dump("Notice the timeout node has disappeared");

                // SetElementValue 添加或覆盖
                XElement settings2 = new XElement("settings");

                settings.SetElementValue("timeout", 30); settings2.Dump("Adds child element");
                settings.SetElementValue("timeout", 60); settings2.Dump("Updates child element");

                // AddAfterSelf Remove Replace
                XElement items =
                            new XElement("items",
                                new XElement("one"),
                                new XElement("three")
                            );

                items.Dump("Original XML");
                items.FirstNode.AddAfterSelf(new XElement("two"));
                items.Dump("After calling items.FirstNode.AddAfterSelf");
                items.FirstNode.ReplaceWith(new XComment("One was here"));
                items.Dump("After calling ReplaceWith");


                XElement contacts = XElement.Parse(@"
                                        <contacts>
	                                        <customer name='Mary'/>
	                                        <customer name='Chris' archived='true'/>
	                                        <supplier name='Susan'>
		                                        <phone archived='true'>012345678<!--confidential--></phone>
	                                        </supplier>
                                        </contacts>");

                contacts.Dump("Before");
                contacts.Elements("customer").Remove();
                contacts.Dump("After");

                contacts.Dump("Before");
                contacts.Elements()         // <customer name='Chris' archived='true'/> 被移除
                                    .Where(e1 => (bool?)e1.Attribute("archived") == true)
                                    .Remove();
                contacts.Dump("After");

                contacts.Dump("Before");
                contacts.Descendants()      // 所有 属性archived的值为 true 的元素被移除
                    .Where(e2 => (bool?)e2.Attribute("archived") == true)
                    .Remove();
                contacts.Dump("After");

                contacts.Dump("Before");
                contacts.Elements()
                    .Where(e3 => e3.DescendantNodes().OfType<XComment>().Any(c => c.Value == "confidential"))
                    .Remove();
                contacts.Dump("After");

                contacts.Descendants().OfType<XComment>().Remove(); // 移除整个树🌳中的注释节点
                contacts.DescendantNodes().OfType<XComment>().Remove();

                // Get and Set Value
                var e = new XElement("date", DateTime.Now);
                e.SetValue(DateTime.Now.AddDays(1));
                e.Value.Dump();

                DateTime dt = (DateTime)e;
                XAttribute a = new XAttribute("resolution", 1.234);
                double res = (double)a;

                // Nullables 可空类型
                int? timeout = (int?)e.Element("timeout");
                double resolution = (double?)e.Attribute("resolution") ?? 1.0;

                var nullXe = new XElement("Empty");
                try
                {
                    int timeout1 = (int)nullXe.Element("timeout");
                }
                catch (Exception ex)
                {
                    ex.Message.Dump("Element (\"timeout\") returns null so the result cannot be cast to int");
                }
                int? timeout2 = (int?)nullXe.Element("timeout");
                timeout2.Dump("Casting to a nullable type solve this problem");

                var data = XElement.Parse(@"
                            <data>
	                            <customer id='1' name='Mary' credit='100' />
	                            <customer id='2' name='John' credit='150' />
	                            <customer id='3' name='Anne' />
                            </data>");

                IEnumerable<string> query =
                    from cust in data.Elements()
                    where (int?)cust.Attribute("credit") > 100  // 属性值转换为 nullable，避免 NullReferenceException 
                    select cust.Attribute("name").Value;
                query.Dump();

                IEnumerable<string> query2 =
                     from cust in data.Elements()
                     where cust.Attributes("credit").Any() && (int)cust.Attribute("credit") > 100
                     select  cust.Attribute("name").Value;
                query2.Dump();

                // XText 与 XElement 混合内容
                XElement summary =
                        new XElement("summary",
                            new XText("An XAttribute is "),
                            new XElement("bold", "not"),
                            new XText(" an XNode")
                        );

                summary.Dump(); // An XAttribute is not an XNode
                {
                    var e1 = new XElement("test", "Hello");
                    e1.Add("World");
                    var e2 = new XElement("test", "Hello", "World");
                    var e3 = new XElement("test", new XText("Hello"), new XText("World"));
                    e1.Dump(); e2.Dump(); e3.Dump();    // 均输出 Hello World
                    e1.Nodes().Count().Dump("Number of children in e1");    // 均为 2
                    e2.Nodes().Count().Dump("Number of children in e2");
                    e3.Nodes().Count().Dump("Number of children in e3");
                }
            }
#elif false
            // Documents and Declarations 文档和声明
            {
                // Building an XHTML document 建立一个XHTML 文档{
                {
                    var styleInstruction = new XProcessingInstruction(
                      "xml-stylesheet", "href='styles.css' type='text/css'");

                    var docType = new XDocumentType("html",
                      "-//W3C//DTD XHTML 1.0 Strict//EN",
                      "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd", null);

                    XNamespace ns = "http://www.w3.org/1999/xhtml";
                    var root =
                        new XElement(ns + "html",
                            new XElement(ns + "head",
                                new XElement(ns + "title", "An XHTML page")),
                            new XElement(ns + "body",
                                new XElement(ns + "h1", "This is a heading."),
                                new XElement(ns + "p", "This is some content."))
                        );

                    var doc =
                        new XDocument(
                            new XDeclaration("1.0", "utf-8", "no"),
                            new XComment("Reference a stylesheet"),
                            styleInstruction,
                            docType,
                            root
                        );

                    string tempPath = Path.Combine(Path.GetTempPath(), "sample.html");
                    doc.Save(tempPath);
                    Process.Start(tempPath);                      // This will display the page in IE or FireFox
                    File.ReadAllText(tempPath).Dump();

                    doc.Root.Name.LocalName.Dump("Root element's local name");

                    XElement bodyNode = doc.Root.Element(ns + "body");
                    (bodyNode.Document == doc).Dump("bodyNode.Document == doc");

                    (doc.Root.Parent == null).Dump("doc.Root.Parent is null");

                    foreach (XNode node in doc.Nodes())
                        Console.Write(node.Parent == null);
                }
                // Declarations 声明
                {
                    var doc =
                            new XDocument(
                                new XDeclaration("1.0", "utf-16", "yes"),
                                new XElement("test", "data")
                            );

                    string tempPath = Path.Combine(Path.GetTempPath(), "test.xml");
                    doc.Save(tempPath);
                    File.ReadAllText(tempPath).Dump();
            

                    var output = new StringBuilder();
                    var settings = new XmlWriterSettings { Indent = true };
                    using (XmlWriter xw = XmlWriter.Create(output, settings))
                        doc.Save(xw);

                    output.ToString().Dump("Notice the encoding is utf-16 and not utf-8");
                }
                // Names and Namespaces 名称和命名空间
                {
                    // 直接使用 {}
                    new XElement("{http://domain.com/xmlspace}customer", "Bloggs");

                    // 使用 XName 和 XNamespace
                    XName localName = "customer";
                    localName.Dump("localName");
                    XName fullName1 = "{http://domain.com/xmlspace}customer";
                    fullName1.Dump("fullname1");

                    XNamespace ns = "http://domain.com/xmlspace";
                    XName fullName2 = ns + "customer";
                    fullName2.Dump("fullname2 - same result, but cleaner and more efficient");
 
                    var data =
                        new XElement(ns + "data",
                            new XAttribute(ns + "id", 123)
                        );

                    data.Dump();
                    XElement x = data.Element(ns + "customer");     // OK
                    XElement y = data.Element("customer");          // null
                    // 为每一个元素分配命名空间
                    var data2 =
                        new XElement(ns + "data",
                            new XElement("customer", "Bloggs"),
                            new XElement("purchase", "Bicycle")
                        );

                    data.Dump("Before");
                    foreach (XElement e in data2.DescendantsAndSelf())
                        if (e.Name.Namespace == "")
                            e.Name = ns + e.Name.LocalName;
                    data.Dump("After");

                    // Prefixes 前缀
                    XNamespace ns1 = "http://domain.com/space1";
                    XNamespace ns2 = "http://domain.com/space2";

                    var mix =
                        new XElement(ns1 + "data",
                            new XElement(ns2 + "element", "value"),
                            new XElement(ns2 + "element", "value"),
                            new XElement(ns2 + "element", "value")
                        );
                    mix.Dump("Without prefixes");
                    mix.SetAttributeValue(XNamespace.Xmlns + "ns1", ns1);
                    mix.SetAttributeValue(XNamespace.Xmlns + "ns2", ns2);
                    mix.Dump("With prefixes");

                    XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                    var nil = new XAttribute(xsi + "nil", true);
                    var cust =
                        new XElement("customers",
                            new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                            new XElement("customer",
                                new XElement("lastname", "Bloggs"),
                                new XElement("dob", nil),
                                new XElement("credit", nil)
                            )
                        );
                    cust.Dump();
                }
            }
#elif true
            // Annotations 注解
            {
#if true
                {
                    XElement e = new XElement("test");
                    e.AddAnnotation("Hello");
                    e.Annotation<string>().Dump("String annotations");
                    e.RemoveAnnotations<string>();
                    e.Annotation<string>().Dump("String annotations");
                }
                {
                    XElement e = new XElement("test");
                    e.AddAnnotation(new Customer { ID = 13, Name = "Linaggan", Purchase = null });
                    e.Annotations<Customer>().First().Name.Dump();
                    e.RemoveAnnotations<Customer>();
                    e.Annotations<Customer>().Count();

                }
#endif
            }

            // Projecting into an X-DOM 将数据映射到X-DOM中
            {
                var customers =
                        new XElement("Customers",
                            new XElement("Customer", new XAttribute("id", 13),
                                new XElement("name", "Lianggan13"),
                                new XElement("buys", 3)));
                NutshellEntities dataContext = new NutshellEntities();
                var customers2 =
                        new XElement("Customers",
                           from c in dataContext.Customer.ToList()  // 注:此处 要有 ToList()
                           select new XElement("Customer", new XAttribute("id", c.ID),
                                    new XElement("name", c.Name),
                                    new XElement("buys", (c.Purchase.Count))));

                {
                    IEnumerable<XElement> queryxe =
                                    from c in dataContext.Customer
                                    select new XElement("Customer", new XAttribute("id", c.ID),
                                                new XElement("name", c.Name),
                                                new XElement("buys", c.Purchase.Count));


                    //var customers3 = new XElement("Customers", queryxe.AsQueryable());
                }
                {
                    // 在查询的客户信息中还包含客户最近的高价购买记录的详细信息 （排除空元素）
                    IEnumerable<XElement> queryxe2 =
                                    from c in dataContext.Customer
                                    let lastBigBuy = (
                                            from p in c.Purchase
                                            where p.Price > 1000
                                            orderby p.Date descending
                                            select p
                                    ).FirstOrDefault()
                                    select new XElement("Customer", new XAttribute("id", c.ID),
                                        new XElement("name", c.Name),
                                        new XElement("buys", c.Purchase.Count),
                                        lastBigBuy == null ? null : // 客户没有lastBigBuy时，返回null，其内容自动被忽略
                                            new XElement("LastBigBuy",
                                                    new XElement("Description", lastBigBuy.Description),
                                                    new XElement("Price", lastBigBuy.Price)
                                                    ));


                    // var customers4 = new XElement("Customers", queryxe2);
                }

                // Transfer X-DOM 转换 X-DOM
                // 获取项目文件中的文件信息
                XElement project = XElement.Parse(@"
                                    <Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
	                                    <PropertyGroup>
		                                    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
		                                    <ProductVersion>9.0.11209</ProductVersion>
	                                    </PropertyGroup>
	                                    <ItemGroup>
		                                    <Compile Include=""ObjectGraph.cs"" />
		                                    <Compile Include=""Program.cs"" />
		                                    <Compile Include=""Properties\AssemblyInfo.cs"" />
		                                    <Compile Include=""Tests\Aggregation.cs"" />
		                                    <Compile Include=""Tests\Advanced\RecursiveXml.cs"" />
	                                    </ItemGroup>
                                    </Project>");
                XNamespace ns = project.Name.Namespace;
                var queryFiles =
                        new XElement("ProjectReport",
                            from complieItem in project.Elements(ns + "ItemGroup").Elements(ns + "Compile")
                            let include = complieItem.Attribute("Include")
                            where include != null
                            select new XElement("File", include.Value));
                queryFiles.Save(Directory.GetCurrentDirectory()+"\\"+"Files.xml");

                var paths =
                      from complieItem in project.Elements(ns + "ItemGroup").Elements(ns + "Compile")
                      let include = complieItem.Attribute("Include")
                      where include != null
                      select include.Value;
                var queryFoderFiles = new XElement("ProjectReport",  ExpandPaths(paths));
                queryFoderFiles.Save(Directory.GetCurrentDirectory()+"\\"+ "FoderFiles .xml");
            }



#endif
            Console.ReadKey();
        }
        public static IEnumerable<XElement> ExpandPaths(IEnumerable<string> paths)
        {
            var brokenup = from path in paths
                           let split = path.Split(new char[] { '\\' }, 2)
                           orderby split[0]
                           select new
                           {
                               name = split[0],
                               remainder = split.ElementAtOrDefault(1)
                           };
            IEnumerable<XElement> files = from b in brokenup
                                          where b.remainder == null
                                          select new XElement("File", b.name);
            IEnumerable<XElement> folders = from b in brokenup
                                            where b.remainder != null
                                            group b.remainder by b.name into grp
                                            select new XElement("Folder",
                                                new XAttribute("Name", grp.Key),
                                                ExpandPaths(grp));
            return files.Concat(folders);
        }
    }
}
