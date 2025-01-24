using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Test
{
    [ValueConversion(typeof(string), typeof(bool))]
    internal class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value switch
            {
                "Left" => HorizontalAlignment.Left,
                "Center" => HorizontalAlignment.Center,
                "Right" => HorizontalAlignment.Right,
                "Stretch" => HorizontalAlignment.Stretch,
                _ => (object)HorizontalAlignment.Center,
            };
            return current == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                HorizontalAlignment.Left => "Left",
                HorizontalAlignment.Center => "Center",
                HorizontalAlignment.Right => "Right",
                HorizontalAlignment.Stretch => "Stretch",
                _ => HorizontalAlignment.Center,
            };
        }
    }
}
