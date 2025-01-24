using eStation_PTL_Demo.Model;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    /// <summary>
    /// AP connect
    /// </summary>
    public class ApConnectViewModel : ViewModelBase
    {
        private ConnInfo conn = new();
        /// <summary>
        /// AP information
        /// </summary>
        public ConnInfo Conn { get { return conn; } set { conn = value; NotifyPropertyChanged(nameof(conn)); } }

        /// <summary>
        /// Command connect
        /// </summary>
        public ICommand CmdConnect { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApConnectViewModel()
        {
            CmdConnect = new MyCommand(DoConnect, CanConnect);
        }

        /// <summary>
        /// Can connect
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Result</returns>
        private bool CanConnect(object arg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Do conenct
        /// </summary>
        /// <param name="obj"></param>
        private void DoConnect(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
