using eStation_PTL_Demo.Enumerator;
using System.Globalization;
using System.Windows.Data;

namespace eStation_PTL_Demo.Converter
{
    /// <summary>
    /// Tag status converter
    /// </summary>
    [ValueConversion(typeof(TagStatus), typeof(string))]
    public class TagStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) return string.Empty;

            return (TagStatus)value switch
            {
                TagStatus.Init => "--",
                TagStatus.Sending => "Sending",
                TagStatus.Success => "OK",
                TagStatus.Fail => "Error",
                TagStatus.Online => "Idle",
                TagStatus.Heartbeat => "Heartbeat",
                TagStatus.Key => "Key",
                TagStatus.GroupControl => "Group",
                TagStatus.Duplicate => "Duplicate",
                _ => "--",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "Sending" => TagStatus.Sending,
                "OK" => TagStatus.Success,
                "Error" => TagStatus.Fail,
                "Idle" => TagStatus.Online,
                "Heartbeat" => TagStatus.Heartbeat,
                "Key" => TagStatus.Key,
                "Group" => TagStatus.GroupControl,
                "Duplicate" => TagStatus.Duplicate,
                _ => TagStatus.Init,
            };
        }
        #endregion
    }
}
