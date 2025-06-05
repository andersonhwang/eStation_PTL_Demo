using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Tag
    /// </summary>
    /// <param name="id">Tag ID</param>
    public class Tag(string id) : TagBasic(id)
    {
        private int? version = null;
        private int? sequence = null;
        private int? rfPower = null;
        private int? battery = null;
        private int? speed = null;
        private int? heartbeat = null;
        private int? turnOff = null;
        private TagStatus status = TagStatus.Init;
        private DateTime? lastSend = null;
        private DateTime? lastReceive = null;
        private DateTime? lastHeartbeat = null;
        private DateTime? lastKey = null;
        protected int heartbeatCount = 0;
        protected int keyCount = 0;
        protected int sendCount = 0;
        protected int receiveCount = 0;
        protected int group = 0;

        public int? Version { get => version; set { version = value; NotifyPropertyChanged(nameof(Version)); } }
        public int? Sequence { get => sequence; set { sequence = value; NotifyPropertyChanged(nameof(Sequence)); } }
        public int? RfPower { get => rfPower; set { rfPower = value; NotifyPropertyChanged(nameof(RfPower)); } }
        public int? Battery { get => battery; set { battery = value; NotifyPropertyChanged(nameof(Battery)); } }
        public int? Speed { get => speed; set { speed = value; NotifyPropertyChanged(nameof(Speed)); } }
        public int? Heartbeat { get => heartbeat; set { heartbeat = value; NotifyPropertyChanged(nameof(Heartbeat)); } }
        public int? TurnOff { get => turnOff; set { turnOff = value; NotifyPropertyChanged(nameof(TurnOff)); } }
        public TagStatus Status { get => status; set { status = value; NotifyPropertyChanged(nameof(Status)); } }
        public DateTime? LastSend { get => lastSend; set { lastSend = value; NotifyPropertyChanged(nameof(LastSend)); } }
        public DateTime? LastReceive { get => lastReceive; set { lastReceive = value; NotifyPropertyChanged(nameof(LastReceive)); } }
        public DateTime? LastHeartbeat { get => lastHeartbeat; set { lastHeartbeat = value; NotifyPropertyChanged(nameof(LastHeartbeat)); } }
        public DateTime? LastKey { get => lastKey; set { lastKey = value; NotifyPropertyChanged(nameof(LastKey)); } }
        public int HeartbeatCount { get => heartbeatCount; set { heartbeatCount = value; NotifyPropertyChanged(nameof(HeartbeatCount)); } }
        public int KeyCount { get => keyCount; set { keyCount = value; NotifyPropertyChanged(nameof(KeyCount)); } }
        public int SendCount { get => sendCount; set { sendCount = value; NotifyPropertyChanged(nameof(SendCount)); NotifyPropertyChanged(nameof(ReceiveCountDisplay)); } }
        public int ReceiveCount { get => receiveCount; set { receiveCount = value; NotifyPropertyChanged(nameof(ReceiveCount)); NotifyPropertyChanged(nameof(ReceiveCountDisplay)); } }
        public string ReceiveCountDisplay { get => $"{ReceiveCount}/{SendCount}"; }
        public int Group { get => group; set { group = value; NotifyPropertyChanged(nameof(Group)); } }
    }
}
