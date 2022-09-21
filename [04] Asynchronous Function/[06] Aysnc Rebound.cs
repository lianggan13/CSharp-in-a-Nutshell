using System.Threading.Tasks;

namespace _04__Asynchronous_Function
{
    public class _06__Aysnc_Rebound
    {
        public static void Show()
        {

        }
    }

    public class MultiAsyncMethod
    {
        public void Show()
        {
            A();
        }
        async void A()
        {
            await B();
        }
        // 方法B 和 方法C 不会应用 UI 应用程序的简单线程安全模 即  ConfigureAwait(false) 避免重复回弹到UI消息循环中， 阻止任务将延续提交到同步上下文中
        async Task B()
        {
            for (int i = 0; i < 1000; i++)
            {
                await C().ConfigureAwait(false);
            }
        }
        async Task C()
        { // await ...}

        }
    }
}
