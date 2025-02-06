namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Tag basic
    /// </summary>
    /// <param name="id">Tag ID</param>
    public class TagBasic(string id) : BaseModel
    {
        protected bool select = false;
        protected string id = string.Empty;
        protected bool r = false;
        protected bool g = false;
        protected bool b = false;
        protected bool beep = false;
        protected bool flashing = false;

        public bool Select { get => select; set { select = value; NotifyPropertyChanged(nameof(Select)); } }
        public string TagID { get; private set; } = id;
        public bool R { get => r; set { r = value; NotifyPropertyChanged(nameof(R)); } }
        public bool G { get => g; set { g = value; NotifyPropertyChanged(nameof(G)); } }
        public bool B{ get => b; set { b = value; NotifyPropertyChanged(nameof(B)); } }
        public bool Beep { get => beep; set { beep = value; NotifyPropertyChanged(nameof(Beep)); } }
        public bool Flashing { get => flashing; set { flashing = value; NotifyPropertyChanged(nameof(Flashing)); } }
    }
}
