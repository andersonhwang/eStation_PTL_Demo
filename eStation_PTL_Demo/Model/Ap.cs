namespace eStation_PTL_Demo.Model
{
    internal class Ap : BaseModel
    {
        string address = "10.10.12.12:9191";
        string sn = string.Empty;
        string version = string.Empty;
        string manufacturer = string.Empty;
        string model = string.Empty;
        ApStatus status = ApStatus.Init;
        DateTime? lastOnline = null;
        DateTime? lastOffline = null;
        DateTime? lastHeartbeat = null;
        DateTime? lastSend = null;
        DateTime? lastReceive = null;
        int code = -1;
        int tokenSend = 0;
        int tokenReceive = 0;
        bool auto = true;

        public string Address { get => address; set { address = value; NotifyPropertyChanged(nameof(Address)); } }
        public string SN { get => sn; set { sn = value; NotifyPropertyChanged(nameof(SN)); } }
        public string Version { get => version; set { version = value; NotifyPropertyChanged(nameof(Version)); } }
        public string Manufacturer { get => manufacturer; set { manufacturer = value; NotifyPropertyChanged(nameof(Manufacturer)); } }
        public string Model { get => model; set { model = value; NotifyPropertyChanged(nameof(Model)); } }
        public ApStatus Status { get => status; set { status = value; NotifyPropertyChanged(nameof(status)); } }
        public DateTime? LastSend { get => lastSend; set { lastSend = value; NotifyPropertyChanged(nameof(LastSend)); } }
        public DateTime? LastReceive { get => lastReceive; set { lastReceive = value; NotifyPropertyChanged(nameof(LastReceive)); } }
        public DateTime? LastHeartbeat { get => lastHeartbeat; set { lastHeartbeat = value; NotifyPropertyChanged(nameof(LastHeartbeat)); } }
        public DateTime? LastOnline { get => lastOnline; set { lastOnline = value; NotifyPropertyChanged(nameof(LastOnline)); } }
        public DateTime? LastOffline { get => lastOffline; set { lastOffline = value; NotifyPropertyChanged(nameof(LastOffline)); } }
        public int Code { get => code; set { code = value; NotifyPropertyChanged(nameof(Code)); } }
        public int TokenSend { get => tokenSend; set { tokenSend = value; NotifyPropertyChanged(nameof(TokenSend)); } }
        public int TokenReceive { get => tokenReceive; set { tokenReceive = value; NotifyPropertyChanged(nameof(TokenReceive)); } }
        public bool Auto { get => auto; set { auto = value; NotifyPropertyChanged(nameof(Auto)); } }
    }

    /// <summary>
    /// AP status
    /// </summary>
    public enum ApStatus
    {
        Init = 0,
        Connecting = 1,
        Online = 2,
        Offline = 3,
        Working = 4,
        ConnectError = 5,
        Receive = 6,
    }
}
