namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// Ptl group order
    /// </summary>
    public class GroupOrder : SequenceEntity
    {
        /// <summary>
        /// LED light flashing times
        /// </summary>
        public int Time { get; set; } = 1;
        /// <summary>
        /// Start group code
        /// </summary>
        public int StartGroup { get; set; } = 0x00;
        /// <summary>
        /// End group code
        /// </summary>
        public int EndGroup { get; set; } = 0xFF;
        /// <summary>
        /// Start tag ID
        /// </summary>
        public string StartID { get; set; } = "00000000";
        /// <summary>
        /// End tag ID
        /// </summary>
        public string EndID { get; set; } = "FFFFFFFF";
        /// <summary>
        /// LED light flashing: True-Yes, False-No
        /// </summary>
        public bool Flashing { get; set; } = true;
        /// <summary>
        /// Beep
        /// </summary>
        public bool Beep { get; set; } = false;
        /// <summary>
        /// Color 0-灭 1-蓝色 2-绿色 3-青色 4-红色 5-紫色 6-黄色 7-白色
        /// </summary>
        public int Color { get; set; } = 0;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupOrder() => Code = 0x84;
    }
}
