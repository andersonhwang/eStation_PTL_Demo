using System.Globalization;
using System.Windows.Data;

namespace eStation_PTL_Demo.Converter
{
    /// <summary>
    /// Tag status converter
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class OtaTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(parameter.ToString()) == (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
