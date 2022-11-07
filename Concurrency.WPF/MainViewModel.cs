using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Concurrency.WPF
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            //MyValue = NotifyTask.Create(CalculateMyValueAsync());
            MyValue = new BindableTask<int>(CalculateMyValueAsync());
        }

        public BindableTask<int> MyValue { get; private set; }

        private async Task<int> CalculateMyValueAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            //throw new Exception("a error occured...");
            return 13;
        }
    }

    public class BindableTask<T> : INotifyPropertyChanged
    {
        private readonly Task<T> _task;

        public BindableTask(Task<T> task)
        {
            _task = task;
            var _ = WatchTaskAsync();
        }

        private async Task WatchTaskAsync()
        {
            try
            {
                await _task;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                OnPropertyChanged($"{nameof(IsNotCompleted)}");
                OnPropertyChanged($"{nameof(IsSuccessfullyCompleted)}");
                OnPropertyChanged($"{nameof(IsFaulted)}");
                OnPropertyChanged($"{nameof(Result)}");
                OnPropertyChanged($"{nameof(Error)}");
            }
        }

        public bool IsNotCompleted
        {
            get { return !_task.IsCompleted; }
        }

        public bool IsSuccessfullyCompleted
        {
            get { return _task.Status == TaskStatus.RanToCompletion; }
        }

        public bool IsFaulted
        {
            get { return _task.IsFaulted; }
        }

        public T Result
        {
            get { return IsSuccessfullyCompleted ? _task.Result : default; }
        }

        public string Error
        {
            get { return IsFaulted ? _task.Exception.InnerException.Message : default; }
        }


        public event PropertyChangedEventHandler PropertyChanged = null;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
