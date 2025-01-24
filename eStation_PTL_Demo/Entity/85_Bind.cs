namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// Bind
    /// </summary>
    public class Bind : BaseEntity, ITag
    {
        /// <summary>
        /// Items
        /// </summary>
        public BindItem[] Items { get; set; } = [];

        /// <summary>
        /// Get length
        /// </summary>
        /// <returns>Length</returns>
        public byte Length() => (byte)(Items.Length * 8 + 8);

        /// <summary>
        /// Default constructor
        /// </summary>
        public Bind() => Code = 0x85;
    }

    /// <summary>
    /// Bind item
    /// </summary>
    public class BindItem : ITagItem
    {
        /// <summary>
        /// Tag ID
        /// </summary>
        public string TagID { get; set; } = string.Empty;

        /// <summary>
        /// Group
        /// </summary>
        public byte Group { get; set; } = 0xFF;

        /// <summary>
        /// Get bytes
        /// </summary>
        /// <returns>By</returns>
        public byte[] Bytes() =>
            [
                byte.Parse(TagID[6..8], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(TagID[8..10], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(TagID[10..12], System.Globalization.NumberStyles.HexNumber),
                Group
            ];
    }
}