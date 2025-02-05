using eStation_PTL_Demo.Model;

namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// AP configuration with sequence entity 
    /// </summary>
    public class ApConfig : SequenceEntity
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
