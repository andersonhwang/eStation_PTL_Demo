using eStation_PTL_Demo.Enumerator;
using System.ComponentModel;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    /// <summary>
    /// Base of view model
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool isConnect = false;
        private int cmd = -1;
        private int count = 0;

        /// <summary>
        /// AP is conenctk
        /// </summary>
        public bool IsConnect
        {
            get => isConnect;
            set { isConnect = value; NotifyPropertyChanged(nameof(IsConnect)); }
        }

        /// <summary>
        /// Command
        /// </summary>
        public int Cmd
        {
            get => cmd;
            set { cmd = value; NotifyPropertyChanged(nameof(Cmd)); }
        }

        /// <summary>
        /// AP is conenctk
        /// </summary>
        public int Count
        {
            get => count;
            set { count = value; NotifyPropertyChanged(nameof(count)); }
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ViewModelBase() { }

        /// <summary>
        /// Ap status handler
        /// </summary>
        /// <param name="status"></param>
        /// <param name="cmd"></param>
        /// <param name="count"></param>
        public void ApStatusHandler(ApStatus status, int cmd, int count)
        {
            IsConnect = status is ApStatus.Online or ApStatus.Working;
        }

        /// <summary>
        /// Can send
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Yes</returns>
        public bool CanSend(object parameter) => true;

        /// <summary>
        /// Get color
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>Color</returns>
        public static int GetColor(bool r, bool g, bool b)
        {
            var data = 0;
            data += (byte)(r ? 1 : 0);                          // Bit 2
            data <<= 1;
            data += (byte)(g ? 1 : 0);                          // Bit 1
            data <<= 1;
            data += (byte)(b ? 1 : 0);                          // Bit 0
            return data;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Interface async command
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Execute async
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>The task</returns>
        Task ExecuteAsync(object parameter);
    }

    /// <summary>
    /// My command
    /// </summary>
    /// <param name="doExecute">Action execute</param>
    /// <param name="canExecute">Function can execute</param>
    public class MyCommand(Action<object> doExecute, Func<object, bool> canExecute) : ICommand
    {
        private readonly Action<object> _doExecute = doExecute;
        private readonly Func<object, bool> _canExecute = canExecute;
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Can execute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Result</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute(parameter ?? new object());
        }

        /// <summary>
        /// Do execute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public void Execute(object? parameter)
        {
            _doExecute.Invoke(parameter ?? new object());
        }
    }

    /// <summary>
    /// My async command
    /// </summary>
    /// <param name="doExecute">Action execute</param>
    /// <param name="canExecute">Function can execute</param>
    public class MyAsyncCommand(Func<object, Task> doExecute, Predicate<object> canExecute) : IAsyncCommand
    {
        private readonly Func<object, Task> _doExecute = doExecute;
        private readonly Predicate<object> _canExecute = canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public Task ExecuteAsync(object parameter)
        {
            return _doExecute(parameter);
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter ?? new object());
        }
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
