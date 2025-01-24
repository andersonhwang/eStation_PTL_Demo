namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// Task response
    /// </summary>
    public class TaskResponse : BaseEntity
    {
        /// <summary>
        /// Response code
        /// </summary>
        public int Response { get; set; } = 0;
        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskResponse() => Code = 0x03;
    }
}
