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
                ApStatus.Init => "等待连接",
                ApStatus.Connecting => "连接中…",
                ApStatus.Online => "在线",
                ApStatus.Offline => "离线",
                ApStatus.Working => "通信中",
                ApStatus.ConnectError => "连接错误",
                _ => "--",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "等待连接" => ApStatus.Init,
                "连接中…" => ApStatus.Connecting,
                "在线" => ApStatus.Online,
                "离线" => ApStatus.Offline,
                "通信中" => ApStatus.Working,
                "错误" => ApStatus.ConnectError,
                _ => ApStatus.Init,
            };
        }
        #endregion
    }
}
