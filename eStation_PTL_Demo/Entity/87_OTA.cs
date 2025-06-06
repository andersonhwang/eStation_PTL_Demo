using eStation_PTL_Demo.Enumerator;

namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// OTA entity
    /// </summary>
    internal class OTA : SequenceEntity
    {
        /// <summary>
        /// Type, 0-AP, 1-PTL
        /// </summary>
        public int Type { get; init; } = 0;
        /// <summary>
        /// Version 
        /// </summary>
        public int Version { get; init; } = 0;
        /// <summary>
        /// Factory code
        /// </summary>
        public FactoryCode Factory { get; init; } = FactoryCode.PHY6252;
        /// <summary>
        /// Firmware MD5
        /// </summary>
        public string MD5 { get; init; } = string.Empty;
        /// <summary>
        /// Firmware data
        /// </summary>
        public byte[] Firmware { get; init; } = [];

        /// <summary>
        /// Default constructor
        /// </summary>
        public OTA() => Code = 0x87;
    }
}
