using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace _01__Threading_Basics
{
    public class _05__Threading_in_Rich_client_Apps
    {
        public static void Show()
        {
			// 跨UI线程，访问UI元素 Dispatcher
			{
				Thread NetServer = new Thread(() => { new MyWindowWithNoSyncContext().ShowDialog(); });
				NetServer.SetApartmentState(ApartmentState.STA);
				NetServer.IsBackground = true;
				NetServer.Start();
			}
			// 跨UI线程，访问UI元素 SynchronizationContext
			{
				//Util.CreateSynchronizationContext();
				Thread NetServer = new Thread(() => { new MyWindowWithSyncContext().ShowDialog(); });
				NetServer.SetApartmentState(ApartmentState.STA);
				NetServer.IsBackground = true;
				NetServer.Start();
				
			}

        }
    }

	partial class MyWindowWithNoSyncContext : Window
	{
		TextBox txtMessage;

		public MyWindowWithNoSyncContext()
		{
			InitializeComponent();
			new Thread(Work).Start();
		}

		void Work()
		{
			Thread.Sleep(2000);           // Simulate time-consuming task
			UpdateMessage("Dispatcher");
		}

		void UpdateMessage(string message)
		{
			Action action = () => txtMessage.Text = message;
			Dispatcher.BeginInvoke(action);
		}

		void InitializeComponent()
		{
			SizeToContent = SizeToContent.WidthAndHeight;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			Content = txtMessage = new TextBox { Width = 250, Margin = new Thickness(10), Text = "Ready" };
		}
	}

	partial class MyWindowWithSyncContext : Window
	{
		TextBox txtMessage;
		SynchronizationContext _uiSyncContext;

		public MyWindowWithSyncContext()
		{
			InitializeComponent();
			// Capture the synchronization context for the current UI thread:
			_uiSyncContext = SynchronizationContext.Current;
			new Thread(Work).Start();
		}

		void Work()
		{
			Thread.Sleep(3000);           // Simulate time-consuming task
			UpdateMessage("SynchronizationContext");
		}

		void UpdateMessage(string message)
		{
			// Marshal the delegate to the UI thread: 消息封送，同步上下文
			// _uiSyncContext 为 null
			_uiSyncContext?.Post(_ => txtMessage.Text = message, null);
		}

		void InitializeComponent()
		{
			SizeToContent = SizeToContent.WidthAndHeight;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			Content = txtMessage = new TextBox { Width = 250, Margin = new Thickness(10), Text = "Ready" };
		}
	}


}
