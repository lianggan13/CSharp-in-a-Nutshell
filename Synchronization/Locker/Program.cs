// See https://aka.ms/new-console-template for more information
using Utilities;

List<string> m_list = new List<string>();

new Thread(AddItem).Start();
new Thread(AddItem).Start();

Console.ReadKey();

void AddItem()
{
    lock (m_list)
    {
        m_list.Add($"Item {m_list.Count()} ThreadId={Thread.CurrentThread.ManagedThreadId}");
    }

    string[] items;
    lock (m_list)
        items = m_list.ToArray();
    foreach (string s in items)
        LogHelper.Info(s);
}