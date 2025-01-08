using System.ComponentModel;

namespace eStation_PTL_Demo.Model
{
    /// <summary>
    /// Base model
    /// </summary>
    internal class BaseModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
