using eStation_PTL_Demo.Enumerator;
using System.Globalization;
using System.Windows.Data;

namespace eStation_PTL_Demo.Converter
{
    /// <summary>
    /// AP status converter
    /// </summary>
    [ValueConversion(typeof(ApStatus), typeof(string))]
    public class ApStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string)) return string.Empty;

            return (ApStatus)value switch
            {
                ApStatus.Init => "Wait",
                ApStatus.Connecting => "Connecting",
                ApStatus.Online => "Online",
                ApStatus.Offline => "Offline",
                ApStatus.Working => "Working",
                ApStatus.ConnectError => "Error",
                _ => "--",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "Wait" => ApStatus.Init,
                "Connecting" => ApStatus.Connecting,
                "Online" => ApStatus.Online,
                "Offline" => ApStatus.Offline,
                "Working" => ApStatus.Working,
                "Error" => ApStatus.ConnectError,
                _ => ApStatus.Init,
            };
        }
        #endregion
    }
}
