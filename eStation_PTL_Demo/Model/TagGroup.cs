using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Tag group
    /// </summary>
    /// <param name="id">Tag ID</param>
    public class TagGroup(string id) : TagBasic(id)
    {
        private int time = 1;
        private int startGroup = 0x00;
        private int endGroup = 0xFF;
        private string startID = "000000000000";
        private string endID = "FFFFFFFFFFFF";

        /// <summary>
        /// Time, betweeen 1~250(ON) or -1(OFF)
        /// </summary>
        public int Time { get => time; set { time = value; NotifyPropertyChanged(nameof(Time)); } }
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
