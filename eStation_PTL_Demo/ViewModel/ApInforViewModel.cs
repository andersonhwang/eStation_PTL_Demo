using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Model;

namespace eStation_PTL_Demo.ViewModel
{
    internal class ApInforViewModel : ViewModelBase
    {
        Ap ap = new();

        /// <summary>
        /// AP
        /// </summary>
        public Ap AP { get => ap; private set { ap = value; NotifyPropertyChanged(nameof(AP)); } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApInforViewModel()
        {
            SendService.Instance.Register(UpdateApInfor);
            SendService.Instance.Register(UpdateApHeartbeat);
        }

        /// <summary>
        /// Update AP infor
        /// </summary>
        /// <param name="apInfor">AP infor</param>
        public void UpdateApInfor(ApInfor apInfor)
        {
            AP.IP = apInfor.IP;
        }

        /// <summary>
        /// Update AP heartbeat
        /// </summary>
        /// <param name="apHeartbeat">AP heartbeat</param>
        public void UpdateApHeartbeat(ApHeartbeat apHeartbeat)
        {
            AP.LastHeartbeat = DateTime.Now;;
        }
    }
}
