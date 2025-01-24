using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    public class TagConfigViewModel : ViewModelBase
    {
        TagConfig config = new();
        /// <summary>
        /// Tag config
        /// </summary>
        public TagConfig Config
        {
            get => config;
            set { config = value; NotifyPropertyChanged(nameof(Config)); }
        }

        public ICommand CmdSendConfig { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TagConfigViewModel()
        {
            CmdSendConfig = new MyAsyncCommand(GroupConfig, CanSend);
        }

        /// <summary>
        /// Group configure
        /// </summary>
        /// <param name="parameter"></param>
        public async Task GroupConfig(object parameter)
        {
            await SendService.Instance.Send(Config);
        }
    }
}
