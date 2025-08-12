using System.Buffers;

namespace eStation_PTL_Demo.Entity
{
    public class ApData
    {
        public required string Id { get; set; }
        public required string Topic { get; set; }
        public required string PayloadSegment { get; set; }
    }
}
