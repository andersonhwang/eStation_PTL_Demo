using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Model
{
    internal class Ap : BaseModel
    {
        string id = "-";
        string alias = "-";
        string ip = "-";
        string mac = "-";
        string appVersion = "-";
        string modVersion = "-";
        ApStatus status = ApStatus.Init;
        DateTime? lastOnline = null;
        DateTime? lastOffline = null;
        DateTime? lastHeartbeat = null;
        DateTime? lastSend = null;
        DateTime? lastReceive = null;
        int code = -1;
        int totalCount = 0;
        int sendCount = 0;
        bool auto = true;
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get => id; set { id = value; NotifyPropertyChanged(nameof(ID)); } }
        /// <summary>
        /// Alias
        /// </summary>
        public string Alias { get => alias; set { alias = value; NotifyPropertyChanged(nameof(Alias)); } }
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get => ip; set { ip = value; NotifyPropertyChanged(nameof(IP)); } }
        /// <summary>
        /// MAC
        /// </summary>
        public string MAC { get => mac; set { mac = value; NotifyPropertyChanged(nameof(MAC)); } }
        /// <summary>
        /// App version
        /// </summary>
        public string AppVersion { get => appVersion; set { appVersion = value; NotifyPropertyChanged(nameof(AppVersion)); } }
        /// <summary>
        /// Mod version
        /// </summary>
        public string ModVersion { get => modVersion; set { modVersion = value; NotifyPropertyChanged(nameof(ModVersion)); } }
        /// <summary>
        /// Status
        /// </summary>
        public ApStatus Status { get => status; set { status = value; NotifyPropertyChanged(nameof(status)); } }
        /// <summary>
        /// Last send time
        /// </summary>
        public DateTime? LastSend { get => lastSend; set { lastSend = value; NotifyPropertyChanged(nameof(LastSend)); } }
        /// <summary>
        /// Last receive time
        /// </summary>
        public DateTime? LastReceive { get => lastReceive; set { lastReceive = value; NotifyPropertyChanged(nameof(LastReceive)); } }
        /// <summary>
        /// Last heartbeat time
        /// </summary>
        public DateTime? LastHeartbeat { get => lastHeartbeat; set { lastHeartbeat = value; NotifyPropertyChanged(nameof(LastHeartbeat)); } }
        /// <summary>
        /// Last online time
        /// </summary>
        public DateTime? LastOnline { get => lastOnline; set { lastOnline = value; NotifyPropertyChanged(nameof(LastOnline)); } }
        /// <summary>
        /// Last offline time
        /// </summary>
        public DateTime? LastOffline { get => lastOffline; set { lastOffline = value; NotifyPropertyChanged(nameof(LastOffline)); } }
        public int Code { get => code; set { code = value; NotifyPropertyChanged(nameof(Code)); } }
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get => totalCount; set { totalCount = value; NotifyPropertyChanged(nameof(TotalCount)); } }
        /// <summary>
        /// Send count
        /// </summary>
        public int SendCount { get => sendCount; set { sendCount = value; NotifyPropertyChanged(nameof(SendCount)); } }
        public bool Auto { get => auto; set { auto = value; NotifyPropertyChanged(nameof(Auto)); } }
    }
}
