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
                TagStatus.Sending => "通信中",
                TagStatus.Success => "成功",
                TagStatus.Fail => "失败",
                TagStatus.Online => "在线",
                TagStatus.Heartbeat => "心跳",
                TagStatus.Key => "按键",
                TagStatus.GroupControl => "群控",
                TagStatus.Duplicate => "去重",
                _ => "--",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "通信中" => TagStatus.Sending,
                "成功" => TagStatus.Success,
                "失败" => TagStatus.Fail,
                "在线" => TagStatus.Online,
                "心跳" => TagStatus.Heartbeat,
                "按键" => TagStatus.Key,
                "群控" => TagStatus.GroupControl,
                "去重" => TagStatus.Duplicate,
                _ => TagStatus.Init,
            };
        }
        #endregion
    }
}
