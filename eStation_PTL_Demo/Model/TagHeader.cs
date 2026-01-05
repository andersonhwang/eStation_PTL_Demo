namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Tag header
    /// </summary>
    /// <param name="id">Tag ID</param>
    public class TagHeader(string id) : TagBasic(id)
    {
        private const int defaultTime = 10;
        private bool autoTest = false;
        private bool autoRegister = false;
        private bool onlyData = true;
        private bool random = false;
        private bool randomBeep = false;
        private int autoMode = 0;
        private int time = defaultTime;
        private int group = 0;
        private int startGroup = 0;
        private int endGroup = 254;
        private string startId = "00000000";
        private string endId = "FFFFFFFF";
        private int speed = 10;
        private int wait = 5;
        private int retryNumber = 0;
        private int round = 10;
        private int currentRound = 0;
        private int roundCount = 0;
        private int sendCount = 0;
        private int successCount = 0;
        private string timeText = defaultTime.ToString();

        public string TimeText
        {
            get => timeText;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    timeText = defaultTime.ToString();
                    Time = defaultTime;
                }
                else if (int.TryParse(value, out int v))
                {
                    timeText = value;
                    Time = v;
                }
                else
                {
                    timeText = defaultTime.ToString();
                    Time = defaultTime;
                }
                NotifyPropertyChanged(nameof(TimeText));
            }
        }

        /// <summary>
        /// Auto test
        /// </summary>
        public bool AutoTest { get => autoTest; set { autoTest = value; NotifyPropertyChanged(nameof(AutoTest)); NotifyPropertyChanged(nameof(AutoTestDisplay)); } }
        /// <summary>
        /// Auto test display
        /// </summary>
        public string AutoTestDisplay { get => autoTest ? "Stop Test" : "Auto Test"; }
        /// <summary>
        /// Auto register
        /// </summary>
        public bool AutoRegister { get => autoRegister; set { autoRegister = value; NotifyPropertyChanged(nameof(AutoRegister)); } }
        /// <summary>
        /// Only data
        /// </summary>
        public bool OnlyData { get => onlyData; set { onlyData = value; NotifyPropertyChanged(nameof(OnlyData)); } }
        public bool Random { get => random; set { random = value; NotifyPropertyChanged(nameof(Random)); } }
        public bool RandomBeep { get => randomBeep; set { randomBeep = value; NotifyPropertyChanged(nameof(RandomBeep)); } }
        /// <summary>
        /// Is bind
        /// </summary>
        public int AutoMode { get => autoMode; set { autoMode = value; NotifyPropertyChanged(nameof(autoMode)); } }
        /// <summary>
        /// Time
        /// </summary>
        public int Time { 
            get => time;
            set
            {
                if (time != value)
                {
                    time = value;
                    TimeText = time.ToString();
                    NotifyPropertyChanged(nameof(Time));
                }
            }
        }
        /// <summary>
        /// Group
        /// </summary>
        public int Group { get => group; set { group = value; NotifyPropertyChanged(nameof(Group)); } }
        public int StartGroup { get => startGroup; set { startGroup = value; NotifyPropertyChanged(nameof(StartGroup)); } }
        public int EndGroup { get => endGroup; set { endGroup = value; NotifyPropertyChanged(nameof(EndGroup)); } }
        public string StartId { get => startId; set { startId = value; NotifyPropertyChanged(nameof(StartId)); } }
        public string EndId { get => endId; set { endId = value; NotifyPropertyChanged(nameof(EndId)); } }

        /// <summary>
        /// Speed
        /// </summary>
        public int Speed { get => speed; set { speed = value; NotifyPropertyChanged(nameof(Speed)); } }
        public int Wait { get => wait; set { wait = value; NotifyPropertyChanged(nameof(Wait)); } }
        public int Round { get => round; set { round = value; NotifyPropertyChanged(nameof(Round)); } }
        public int CurrenRound { get => currentRound; set { currentRound = value; NotifyPropertyChanged(nameof(CurrenRound)); } }
        public int RetryNumber { get => retryNumber; set { retryNumber = value; NotifyPropertyChanged(nameof(RetryNumber)); } }
        /// <summary>
        /// Round count
        /// </summary>
        public int RoundCount { get => roundCount; set { roundCount = value; NotifyPropertyChanged(nameof(TestInfor)); } }
        /// <summary>
        /// Send count
        /// </summary>
        public int SendCount { get => sendCount; set { sendCount = value; NotifyPropertyChanged(nameof(TestInfor)); } }
        /// <summary>
        /// Success count
        /// </summary>
        public int SuccsessCount { get => successCount; set { successCount = value; NotifyPropertyChanged(nameof(TestInfor)); } }
        /// <summary>
        /// Test infor
        /// </summary>
        public string TestInfor { get => string.Format("【统计信息】共计{0}轮，{1}次，成功{2}次，成功率：{3:P}", roundCount, sendCount, successCount, successCount * 1F / (sendCount == 0 ? 1 : sendCount)); }
    }
}
