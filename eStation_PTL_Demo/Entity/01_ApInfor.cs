namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// AP Infor
    /// </summary>
    public class ApInfor : BaseEntity
    {
        /// <summary>
        /// Client ID 
        /// </summary>
        public string ID { get; set; } = string.Empty;
        /// <summary>
        /// MAC address 
        /// </summary>
        public string MAC { get; set; } = string.Empty;
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; } = string.Empty;
        /// <summary>
        /// Alias, only for display -XX
        /// </summary>
        public string Alias { get; set; } = "00";
        /// <summary>
        /// Client type
        /// </summary>
        public ConnType ClientType { get; set; } = ConnType.MQTT;
        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; set; } = string.Empty; 
        /// <summary>
        /// App version
        /// </summary>
        public int AppVersion { get; set; } = 0;
        /// <summary>
        /// Mod version
        /// </summary>
        public int[] ModVersion { get; set; } = [0, 0, 0];
        /// <summary>
        /// Disk size
        /// </summary>
        public int DiskSize { get; internal set; } = 0;
        /// <summary>
        /// Free space
        /// </summary>
        public int FreeSpace { get; internal set; } = 0;
        /// <summary>
        /// Default cosntructor
        /// </summary>
        public ApInfor() => Code = 0x01;
    }
}
