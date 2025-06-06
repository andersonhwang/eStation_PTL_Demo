using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Model
{
    public class OtaFirmware : BaseModel
    {
        private int type = 0;
        private int version = 0;
        private FactoryCode factory = FactoryCode.PHY6252;
        private string md5 = string.Empty;
        private string path = string.Empty;

        /// <summary>
        /// Type, 0-AP, 1-PTL
        /// </summary>
        public int Type { get => type; set { type = value; NotifyPropertyChanged(nameof(Type)); } }
        /// <summary>
        /// Version 
        /// </summary>
        public int Version { get => version; set { version = value; NotifyPropertyChanged(nameof(Version)); } }
        /// <summary>
        /// Factory code
        /// </summary>
        public FactoryCode Factory { get => factory; set { factory = value; NotifyPropertyChanged(nameof(Factory)); } }
        /// <summary>
        /// Firmware MD5
        /// </summary>
        public string MD5 { get => md5; set { md5 = value; NotifyPropertyChanged(nameof(MD5)); } }
        /// <summary>
        /// Firmware path
        /// </summary>
        public string Path { get => path; set { path = value; NotifyPropertyChanged(nameof(Path)); } }
        /// <summary>
        /// Firmware bytes
        /// </summary>
        public byte[] Bytes { get; set; } = [];
    }
}