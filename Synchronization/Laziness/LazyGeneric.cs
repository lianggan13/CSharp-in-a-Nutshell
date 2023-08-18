namespace _15__Lazy_Initialization
{
    public class LazyGeneric
    {
        public static void Show()
        {
            var v = new Foo().Expensive;

        }

        class Foo
        {
            // 双检锁 执行一次volatitle读操作，避免在对象初始化后进行锁操作
            Lazy<Expensive> _expensive = new Lazy<Expensive>(() => new Expensive(), true);
            public Expensive Expensive { get { return _expensive.Value; } }
        }

        class Expensive { }
    }
}
