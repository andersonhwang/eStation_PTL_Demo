using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Tag group
    /// </summary>
    /// <param name="id">Tag ID</param>
    public class TagGroup(string id) : TagBasic(id)
    {
        private const int defaultTime = 1;
        private int time = defaultTime;
        private int startGroup = 0x00;
        private int endGroup = 0xFF;
        private string startID = "000000000000";
        private string endID = "FFFFFFFFFFFF";
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
                    // 非法输入时可选：恢复为上次有效值或0
                    timeText = defaultTime.ToString();
                    Time = defaultTime;
                }
                NotifyPropertyChanged(nameof(TimeText));
            }
        }
        /// <summary>
        /// Time, betweeen 1~250(ON) or -1(OFF)
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
        /// Start group code
        /// </summary>
        public int StartGroup { get => startGroup; set { startGroup = value; NotifyPropertyChanged(nameof(StartGroup)); } }
        /// <summary> 
        /// End group code
        /// </summary>
        public int EndGroup { get => endGroup; set { endGroup = value; NotifyPropertyChanged(nameof(EndGroup)); } }
        /// <summary>
        /// First tag ID
        /// </summary>
        public string StartID { get => startID; set { startID = value; NotifyPropertyChanged(nameof(StartID)); } }
        /// <summary>
        /// Last tag ID
        /// </summary>
        public string EndID { get => endID; set { endID = value; NotifyPropertyChanged(nameof(EndID)); } }
    }
}
