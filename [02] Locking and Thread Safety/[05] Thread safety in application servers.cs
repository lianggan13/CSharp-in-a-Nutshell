using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace _02__Locking_and_Thread_Safety
{
    /// <summary>
    /// Web应用服务器的安全性
    /// </summary>
    public class _05__Thread_safety_in_application_servers
    {
        /*
         应用服务器为每个客户端的每一个请求创建一个独立的对象实例，
        减少线程间交互，而交互通常发生在静态字段上 */

        public static void Show()
        {
            UserCache.Show();

            UserCacheAsync.Show();
        }

        class User { public int ID; }

        static class UserCache
        {
            public static void Show()
            {
                new Thread(() => Console.WriteLine(UserCache.GetUser(1).ID)).Start();
                new Thread(() => Console.WriteLine(UserCache.GetUser(1).ID)).Start();
                new Thread(() => Console.WriteLine(UserCache.GetUser(1).ID)).Start();
            }

            static Dictionary<int, User> _users = new Dictionary<int, User>();

            public static User GetUser(int id)
            {
                User u = null;

                lock (_users)
                    if (_users.TryGetValue(id, out u))  // 首先尝试去缓存字典中取数据
                        return u;

                u = RetrieveUser(id);           // Method to retrieve from database;
                lock (_users) _users[id] = u;
                return u;
            }

            static User RetrieveUser(int id)
            {
                Thread.Sleep(1000);  // simulate a time-consuming operation
                return new User { ID = id };
            }
        }

        static class UserCacheAsync
        {
            public async static void Show()
            {
                new Thread(() => Console.WriteLine(UserCacheAsync.GetUser(1).Id)).Start();
                new Thread(() => Console.WriteLine(UserCacheAsync.GetUser(1).Id)).Start();
                new Thread(() => Console.WriteLine(UserCacheAsync.GetUser(1).Id)).Start();

                var u = await UserCacheAsync.GetUser(2);
                Console.WriteLine(u.ID);
            }


            static Dictionary<int, Task<User>> _users = new Dictionary<int, Task<User>>();

            public static Task<User> GetUser(int id)
            {
                Task<User> u = null;
                lock (_users)
                    if (!_users.TryGetValue(id, out u))
                        _users[id] = Task.Run<User>(() => RetrieveUser(id));

                return _users[id];
            }

            static User RetrieveUser(int id)
            {
                Thread.Sleep(1000);  // simulate a time-consuming operation
                return new User { ID = id };
            }

        }

    }
}
