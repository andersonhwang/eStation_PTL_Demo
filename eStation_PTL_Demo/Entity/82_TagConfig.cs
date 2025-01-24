namespace eStation_PTL_Demo.Entity
{
    public class TagConfig : BaseEntity
    {
        /// <summary>
        /// Start tag ID
        /// </summary>
        public string StartID { get; set; } = "00000000";
        /// <summary>
        /// End tag ID
        /// </summary>
        public string EndID { get; set; } = "FFFFFFFF";
        /// <summary>
        /// 休眠周期，毫秒，默认为2000，范围1～65000
        /// </summary>
        public byte SleepInterval { get; set; }
        /// <summary>
        /// 心跳周期，分组，默认为10，范围1～254
        /// </summary>
        public byte HeartbeatInterval { get; set; } = 10;
        /// <summary>
        /// 自动灭灯，档位5秒，默认36，范围1～254
        /// </summary>
        public byte TurnOffInterval { get; set; } = 5;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TagConfig() => Code = 0x82;
    }
}
