using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Tag group
    /// </summary>
    /// <param name="id">Tag ID</param>
    public class TagGroup(string id) : TagBasic(id)
    {
        int time = 0;
        int bind = 0;
        TagStatus status = TagStatus.Init;
        DateTime? lastSend = null;
        DateTime? lastReceive = null;
        protected int sendCount = 0;
        protected int receiveCount = 0;

        public int Time { get => time; set { time = value; NotifyPropertyChanged(nameof(Time)); } }
        public int Bind { get => bind; set { bind = value; NotifyPropertyChanged(nameof(bind)); } }
        public TagStatus Status { get => status; set { status = value; NotifyPropertyChanged(nameof(Status)); } }
        public DateTime? LastSend { get => lastSend; set { lastSend = value; NotifyPropertyChanged(nameof(LastSend)); } }
        public DateTime? LastReceive { get => lastReceive; set { lastReceive = value; NotifyPropertyChanged(nameof(LastReceive)); } }
        public int SendCount { get => sendCount; set { sendCount = value; NotifyPropertyChanged(nameof(SendCount)); NotifyPropertyChanged(nameof(ReceiveCountDisplay)); } }
        public int ReceiveCount { get => receiveCount; set { receiveCount = value; NotifyPropertyChanged(nameof(ReceiveCount)); NotifyPropertyChanged(nameof(ReceiveCountDisplay)); } }
        public string ReceiveCountDisplay { get => $"{ReceiveCount}/{SendCount}"; }
    }
}
