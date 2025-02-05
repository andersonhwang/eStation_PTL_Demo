using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Model;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    /// <summary>
    /// AP config
    /// </summary>
    public class ApConfigViewModel : ViewModelBase
    {
        private bool? dialogResult;
        private ApConfigB config = new();

        /// <summary>
        /// Dialog result
        /// </summary>
        public bool? DialogResult { get => dialogResult; set { dialogResult = value; NotifyPropertyChanged(nameof(DialogResult)); } }
        /// <summary>
        /// AP config
        /// </summary>
        public ApConfigB Config { get => config; set { config = value; NotifyPropertyChanged(nameof(Config)); } }

        /// <summary>
        /// Command - Config AP
        /// </summary>
        public ICommand CmdConfig { get; set; }

        /// <summary>
        /// Command - Cancel
        /// </summary>
        public ICommand CmdCancel { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ApConfigViewModel()
        {
            CmdConfig = new MyAsyncCommand(DoConfig, CanConfig);
            CmdCancel = new MyCommand(DoCancel, CanCancel);
        }

        /// <summary>
        /// Can config
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CanConfig(object obj) => true;

        /// <summary>
        /// Can cancel
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CanCancel(object obj) => true;

        /// <summary>
        /// Do config
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task DoConfig(object obj)
        {
            await SendService.Instance.Send(new ApConfig { Config = Config });
            DialogResult = true;
        }

        public void DoCancel(object obj)
        {
            DialogResult = true;
        }
    }
}
