using System.Globalization;
using System.Windows.Data;

namespace eStation_PTL_Demo.Converter
{
    public class ConnTypeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;

            if (values.Length >= 2 && values[0] is bool isConnect && values[1] is int connType)
            {
                // 原来的逻辑
                result = isConnect && (connType == 1);
            }

            // 检查是否需要取反结果
            if (parameter is string param && param == "Invert")
            {
                return !result;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (bool)value ? [true, 1] : [false, 0];
        }
    }
}
