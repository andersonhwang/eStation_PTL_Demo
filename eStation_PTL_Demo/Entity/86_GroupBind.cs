namespace eStation_PTL_Demo.Entity
{
    /// <summary>
    /// Group bind
    /// </summary>
    public class GroupBind : SequenceEntity, ITag
    {
        /// <summary>
        /// Group code
        /// </summary>
        public byte Group { get; set; } = 0xFF;

        /// <summary>
        /// Items ID
        /// </summary>
        public string[] Items { get; set; } = [];

        /// <summary>
        /// Get length
        /// </summary>
        /// <returns>Length</returns>
        public byte Length() => (byte)(Items.Length * 4 + 8);

        /// <summary>
        /// Default constructor
        /// </summary>
        public GroupBind() => Code = 0x86;

        /// <summary>
        /// Convert PTLGroupBind to PTLBind
        /// </summary>
        /// <returns>PTLBind</returns>
        public Bind Convert()
        {
            var list = new List<BindItem>();
            foreach (var item in Items) list.Add(new BindItem { TagID = item, Group = Group });
            return new Bind { Token = Token, Items = [.. list] };
        }
    }
}
