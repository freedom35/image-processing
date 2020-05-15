using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;

using Freedom35.ImageProcessing;

namespace ImageViewerApp
{
    /// <summary>
    /// Converts property to string for WPF binding. 
    /// </summary>
    [ValueConversion(typeof(Enum[]), typeof(String[]))]
    internal sealed class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Array)value).OfType<Enum>().Select(val =>
            {
                if (val is ConvolutionType type)
                {
                    return type.GetDescription();
                }
                else
                {
                    return val.ToString();
                }
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            FieldInfo[] fields = typeof(T).GetFields();

            FieldInfo field = fields.FirstOrDefault(f => Attribute.GetCustomAttribute(f, typeof(DescriptionAttribute)) is DescriptionAttribute attr && attr.Description == description);

            if (field != null)
            {
                return (T)field.GetValue(null);
            }
            else
            {
                return default;
            }
        }
    }
}
