namespace eStation_PTL_Demo.Entity
{
    internal class ApConfigB
    {
        /// <summary>
        /// Connect type
        /// </summary>
        public int ConnType { get; set; } = 0;
        /// <summary>
        /// Server address
        /// </summary>
        public string ServerAddress { get; set; } = string.Empty;
        /// <summary>
        /// Connect parameters
        /// </summary>
        public string[] ConnParams { get; set; } = [];
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; } = 0;
        /// <summary>
        /// Auto IP
        /// </summary>
        public bool AutoIP { get; set; } = true;
        /// <summary>
        /// Local IP
        /// </summary>
        public string LocalIP { get; set; } = string.Empty;
        /// <summary>
        /// Subnet mask
        /// </summary>
        public string Subnet { get; set; } = string.Empty;
        /// <summary>
        /// Gateway
        /// </summary>
        public string Gateway { get; set; } = string.Empty;
        /// <summary>
        /// Heartbeat speend, >= 15 seconds
        /// </summary>
        public int Heartbeat { get; set; } = 15;
    }
}
