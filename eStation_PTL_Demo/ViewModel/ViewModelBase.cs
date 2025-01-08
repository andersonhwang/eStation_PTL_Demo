using eStation_PTL_Demo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        public ViewModelBase()
        {
            Worker.Instance.Register(ApStatusHandler);
        }

        /// <summary>
        /// Ap status handler
        /// </summary>
        /// <param name="status"></param>
        /// <param name="cmd"></param>
        /// <param name="count"></param>
        public void ApStatusHandler(ApStatus status, int cmd, int count)
        {
            IsConnect = status is ApStatus.Online or ApStatus.Receive or ApStatus.Working;
        }

        /// <summary>
        /// Can send
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Yes</returns>
        public bool CanSend(object parameter) => Worker.Instance.IsOnline;

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
    /// My command
    /// </summary>
    /// <param name="execute"></param>
    /// <param name="executeFunc"></param>
    public class MyCommand(Action<object> execute, Func<object, bool> executeFunc) : ICommand
    {
        private readonly Action<object> _execute = execute;
        private readonly Func<object, bool> _executeFunc = executeFunc;
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
            return true;
        }

        public void Execute(object? parameter)
        {
            _execute.Invoke(parameter ?? new object());
        }
    }
}
