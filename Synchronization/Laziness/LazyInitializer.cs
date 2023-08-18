namespace _15__Lazy_Initialization
{
    public class LazyInitializer
    {
        public static void Show()
        {
            new Foo().Expensive.Dump();
        }

        class Foo
        {
            Expensive _expensive;
            public Expensive Expensive
            {                                    // Implement double-checked locking
                get

                {
                    // 允许多个线程竞争实例化，取最先实例化结果
                    LazyInitializer.EnsureInitialized(ref _expensive, () => new Expensive());
                    return _expensive;
                }
            }
        }

        class Expensive {  /* Suppose this is expensive to construct */  }
    }
}
