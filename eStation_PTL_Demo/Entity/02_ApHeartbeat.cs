namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// AP heartbeat
    /// </summary>
    public class ApHeartbeat : BaseEntity
    {
        /// <summary>
        /// [Readonly]Task in working channel
        /// </summary>
        public int TotalCount { get; set; } = 0;
        /// <summary>
        /// [Readonly]Task in cache
        /// </summary>
        public int SendCount { get; set; } = 0;
        /// <summary>
        /// Default constructor
        /// </summary>
        public ApHeartbeat() => Code = 0x02;
    }
}
