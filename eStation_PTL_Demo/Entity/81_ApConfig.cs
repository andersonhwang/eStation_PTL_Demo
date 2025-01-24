namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// AP configuration with base entityt
    /// </summary>
    internal class ApConfig : BaseEntity
    {
        /// <summary>
        /// AP configuration
        /// </summary>
        public ApConfigB Config { get; set; } = new ApConfigB();

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApConfig() => Code = 0x81;
    }
}
