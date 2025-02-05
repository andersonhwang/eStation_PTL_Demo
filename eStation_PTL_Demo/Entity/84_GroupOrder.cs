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
        /// Group order items
        /// </summary>
        public GroupOrderItem[] Items { get; set; } = [];

        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupOrder() => Code = 0x84;
    }

    /// <summary>
    /// Ptl group order item
    /// </summary>
    public class GroupOrderItem : ITagItem
    {
        /// <summary>
        /// Group code
        /// </summary>
        public int Group { get; set; } = 0;
        /// <summary>
        /// Beep
        /// </summary>
        public bool Beep { get; set; } = false;
        /// <summary>
        /// Color 0-灭 1-蓝色 2-绿色 3-青色 4-红色 5-紫色 6-黄色 7-白色
        /// </summary>
        public int Color { get; set; } = 0;
        /// <summary>
        /// LED light flashing: True-Yes, False-No
        /// </summary>
        public bool Flashing { get; set; } = true;
        /// <summary>
        /// Get Ptl order item bytes
        /// </summary>
        /// <returns></returns>
        public byte[] Bytes() => [(byte)Group, Byte()];

        /// <summary>
        /// Get color byte
        /// </summary>
        /// <returns>Color byte</returns>
        private byte Byte()
            => (byte)((Beep ? 0x08 : 0x00) | (Color == 0 ? 0x00 : Flashing ? 0xC0 : 0x40) | Color);
    }
}
