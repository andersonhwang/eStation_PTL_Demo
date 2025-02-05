namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Debug information
    /// </summary>
    public class DebugInfo : BaseModel
    {
        private int type = 0;
        private string header = string.Empty;
        private string topic = string.Empty;

        /// <summary>
        /// Debug type: 0-Request, 1-Response
        /// </summary>
        public int DebugType { get { return type; } set { type = value; NotifyPropertyChanged(nameof(DebugType)); } }
        /// <summary>
        /// Header
        /// </summary>
        public string Header { get => header; set { header = value; NotifyPropertyChanged(nameof(Header)); } }
        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get => topic; set { topic = value; NotifyPropertyChanged(nameof(Topic)); } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Header</param>
        /// <param name="topic">Topic</param>
        public DebugInfo(int type, string header, string topic)
        {
            DebugType = type;
            Header = header;
            Topic = topic;
        }
    }

    /// <summary>
    /// Debug item
    /// </summary>
    public class DebugItem : BaseModel
    {
        private DateTime time;
        private int code;
        private string name;
        private string data;

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time { get => time; }
        /// <summary>
        /// Code
        /// </summary>
        public int Code { get => code; }
        /// <summary>
        /// Data
        /// </summary>
        public string Data { get => data; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get => name; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Code</param>
        /// <param name="data">Data</param>
        public DebugItem(int code, string data)
        {
            this.code = code;
            this.data = data;
            time = DateTime.Now;
            name = code switch
            {
                0x00 => "ApRegister",
                0x01 => "ApInfor",
                0x02 => "ApHeartbeat",
                0x03 => "TaskResponse",
                0x04 => "TaskResult",
                0x80 => "ApREsponse",
                0x81 => "ApConfig",
                0x82 => "TagConfig",
                0x83 => "Order",
                0x84 => "GroupOrder",
                0x85 => "Bind",
                0x86 => "GroupBind",
                0x87 => "OTA",
                _ => string.Empty,
            };
        }
    }
}
