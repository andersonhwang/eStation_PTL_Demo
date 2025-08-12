using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// AP
    /// </summary>
    public class Ap : BaseModel
    {
        private string id = "-";
        private string alias = "-";
        private string ip = "-";
        private string mac = "-";
        private string appVersion = "-";
        private string modVersion = "-";
        private ApStatus status = ApStatus.Init;
        private DateTime? lastOnline = null;
        private DateTime? lastOffline = null;
        private DateTime? lastHeartbeat = null;
        private DateTime? lastSend = null;
        private DateTime? lastReceive = null;
        private int totalCount = 0;
        private int sendCount = 0;
        private int connType = -1;
        private bool encrypted = false;

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get => id; set { id = value; NotifyPropertyChanged(nameof(ID)); NotifyPropertyChanged(nameof(IDDisplay)); } }
        /// <summary>
        /// ID for display
        /// </summary>
        public string IDDisplay => (string.IsNullOrEmpty(Alias) || ID.Equals(Alias)) ? ID : $"{ID}({Alias})";
        /// <summary>
        /// Alias
        /// </summary>
        public string Alias { get => alias; set { alias = value; NotifyPropertyChanged(nameof(Alias)); NotifyPropertyChanged(nameof(IDDisplay)); } }
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
        public ApStatus Status { get => status; set { status = value; NotifyPropertyChanged(nameof(Status)); } }
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
        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get => totalCount; set { totalCount = value; NotifyPropertyChanged(nameof(TotalCount)); } }
        /// <summary>
        /// Send count
        /// </summary>
        public int SendCount { get => sendCount; set { sendCount = value; NotifyPropertyChanged(nameof(SendCount)); } }

        /// <summary>
        /// AP config
        /// </summary>
        public ApConfigB ConfigB = new();

        /// <summary>
        /// Conn type
        /// </summary>
        public int ConnType { get => connType; set { connType = value; NotifyPropertyChanged(nameof(ConnType)); } }

        /// <summary>
        /// Use TLS12
        /// </summary>
        public bool Encrypted { get => encrypted; set { encrypted = value; NotifyPropertyChanged(nameof(Encrypted)); } }
    }
}
