using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;

namespace ImageViewerApp
{
    /// <summary>
    /// Converts property to string for WPF binding. 
    /// </summary>
    [ValueConversion(typeof(Enum[]), typeof(String[]))]
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Array)value).OfType<Enum>().Select(val => val.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
