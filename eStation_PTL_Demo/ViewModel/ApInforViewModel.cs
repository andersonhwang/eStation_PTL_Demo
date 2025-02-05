using eStation_PTL_Demo.Core;
using eStation_PTL_Demo.Entity;
using eStation_PTL_Demo.Enumerator;
using eStation_PTL_Demo.Model;
using System.Windows.Input;

namespace eStation_PTL_Demo.ViewModel
{
    public class ApInforViewModel : ViewModelBase
    {
        private Ap ap = new();

        /// <summary>
        /// AP
        /// </summary>
        public Ap AP { get => ap; private set { ap = value; NotifyPropertyChanged(nameof(AP)); } }

        /// <summary>
        /// Command - OTA
        /// </summary>
        public ICommand CmdOTA { get; private set; }
        /// <summary>
        /// Command - Config
        /// </summary>
        public ICommand CmdConfig { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApInforViewModel()
        {
            CmdOTA = new MyCommand(DoOTA, CanOTA);
            CmdConfig = new MyCommand(DoConfig, CanConfig);

            SendService.Instance.Register(UpdateApStatus);
            SendService.Instance.Register(UpdateTaskResult);
            SendService.Instance.Register(UpdateHeartbeat);
            SendService.Instance.Register(UpdateApInfor);
            SendService.Instance.Register(UpdateApHeartbeat);
        }

        /// <summary>
        /// Can OTA
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanOTA(object parameter) => IsConnect;

        /// <summary>
        /// Can config
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanConfig(object parameter) => IsConnect;

        /// <summary>
        /// Do OTA
        /// </summary>
        /// <param name="parameter"></param>
        public void DoOTA(object parameter)
        {
            var ota = new FirmwareWindow();
            ota.ShowDialog();
        }

        /// <summary>
        /// Do config
        /// </summary>
        /// <param name="parameter"></param>
        public void DoConfig(object parameter)
        {
            var config = new ConfigWindow(SendService.Instance.GetConfig(AP.ID));
            config.ShowDialog();
        }

        /// <summary>
        /// Update AP status
        /// </summary>
        /// <param name="status">AP status</param>
        public void UpdateApStatus(ApStatus status)
        {
            AP.Status = status;
            switch (AP.Status)
            {
                case ApStatus.Working: AP.LastSend = DateTime.Now; break;
                case ApStatus.Offline: AP.LastOffline = DateTime.Now; break;
                case ApStatus.Online: AP.LastOnline = DateTime.Now; break;
            }
        }

        /// <summary>
        /// Update task result
        /// </summary>
        /// <param name="result">Task result</param>
        public void UpdateTaskResult(TaskResult result)
        {
            AP.LastReceive = DateTime.Now;
        }

        /// <summary>
        /// Update heartbeat
        /// </summary>
        /// <param name="heartbeat">Heartbeat</param>
        public void UpdateHeartbeat(ApHeartbeat heartbeat)
        {
            AP.LastHeartbeat = DateTime.Now;
        }

        /// <summary>
        /// Update AP infor
        /// </summary>
        /// <param name="info">AP information</param>
        public void UpdateApInfor(ApInfor info)
        {
            AP.ID = info.ID;
            AP.IP = info.IP;
            AP.Alias = info.Alias;
            AP.MAC = info.MAC;
            AP.AppVersion = info.AppVersion.ToString();
            AP.ModVersion = $"S:{info.ModVersion[0]},R:{info.ModVersion[1]},K:{info.ModVersion[2]}";
        }

        /// <summary>
        /// Update AP heartbeat
        /// </summary>
        /// <param name="apHeartbeat">AP heartbeat</param>
        public void UpdateApHeartbeat(ApHeartbeat apHeartbeat)
        {
            AP.LastHeartbeat = DateTime.Now;
            AP.SendCount = apHeartbeat.SendCount;
            AP.TotalCount = apHeartbeat.TotalCount;
        }
    }
}
