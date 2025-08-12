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
        /// Command - Config
        /// </summary>
        public ICommand CmdCertificate { get; private set; }

        /// <summary>
        /// Command - Time
        /// </summary>
        public ICommand CmdTime { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApInforViewModel()
        {
            CmdOTA = new MyCommand(DoOTA, CanOTA);
            CmdConfig = new MyCommand(DoConfig, CanConfig);
            CmdCertificate = new MyCommand(DoCertificate, CanCertificate);
            CmdTime = new MyCommand(DoTime, CanTime);

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
        /// Can certificate
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanCertificate(object parameter) => IsConnect;

        /// <summary>
        /// Can certificate
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanTime(object parameter) => IsConnect;

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
        /// Do Certificate
        /// </summary>
        /// <param name="parameter"></param>
        public void DoCertificate(object parameter)
        {
            var ctf = new ClientCertificateWindow();
            ctf.ShowDialog();
        }

        /// <summary>
        /// Do Time
        /// </summary>
        /// <param name="parameter"></param>
        public void DoTime(object parameter)
        {
            var tw = new TimeWindow();
            tw.ShowDialog();
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
            AP.ConnType = info.ConfigB.ConnType;
            AP.Encrypted = info.ConfigB.Encrypted;
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
