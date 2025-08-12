namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// Task result
    /// </summary>
    public class TaskResult : BaseEntity
    {
        /// <summary>
        /// PTL infor
        /// </summary>
        public TagResult[] Items { get; set; } = [];

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskResult() => Code = 0x04;
    }

    /// <summary>
    /// PTL information
    /// </summary>
    public class TagResult
    {
        /// <summary>
        /// 返回类型，心跳(0)/数据反馈(1)/按键(2)/群控(3)/数据反馈去重(4)
        /// </summary>
        public int DataType { get; set; } = 0;
        /// <summary>
        /// 标签类型信息
        /// </summary>
        public int Type { get; set; } = 1;
        /// <summary>
        /// 灯条完整mac地址
        /// </summary>
        public string TagID { get; set; } = string.Empty;
        /// <summary>
        /// 灯条固件版本号
        /// </summary>
        public int Version { get; set; } = 0;
        /// <summary>
        /// 低功耗休眠时间间隔，单位毫秒 ，1~65000，默认2000（2秒）
        /// </summary>
        public int SleepInterval { get; set; } = 0;
        /// <summary>
        /// 心跳包时间间隔，单位分钟，1~254，默认10（10分钟） 
        /// </summary>
        public int HeartbeatInterval { get; set; } = 0;
        /// <summary>
        /// 无指令自动灭灯时间间隔，挡位5秒，1~254，默认36（180秒） 
        /// </summary>
        public int TurnOffInterval { get; set; } = 0;
        /// <summary>
        /// 颜色枚举，0-灭 1-蓝色 2-绿色 3-青色 4-红色 5-紫色 6-黄色 7-白色
        /// </summary>
        public int Color { get; set; } = 0;
        /// <summary>
        /// 是否蜂鸣
        /// </summary>
        public bool Beep { get; set; } = false;
        /// <summary>
        /// 是否闪烁
        /// </summary>
        public bool Flashing { get; set; } = false;
        /// <summary>
        /// 绑定号，0-254
        /// </summary>
        public int Group { get; set; } = 0;
        /// <summary>
        /// 信号强度
        /// </summary>
        public int RfPower { get; set; } = -256;
        /// <summary>
        /// 电池电压值实际电压值*10，档位0.1V
        /// </summary>
        public int Voltage { get; set; } = 0;
    }

    public class TaskResultV1
    {
        public string ID { get; set; } = string.Empty;
        public int TotalCount { get; set; } = 0;
        public int SendCount { get; set; } = 0;
        public TagResultV1[] Results { get; set; } = [];

    }

    public class TagResultV1
    {
        public string TagID { get; set; } = string.Empty;
        public string Version { get; set; } = "";
        public int ResultType { get; set; } = 0;
        public int RfPowerSend { get; set; } = -256;
        public int RfPowerRecv { get; set; } = -256;
        public int Battery { get; set; } = 0;
        public int Sequence { get; set; } = 0;
        public Color[] Colors { get; set; } = [];
        public int Group { get; set; } = 0;        
    }

    public class Color
    {
        public bool R { get; set; }
        public bool G { get; set; }
        public bool B { get; set; }

    }
}
