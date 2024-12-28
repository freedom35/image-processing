using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;

using Freedom35.ImageProcessing;
using System.Diagnostics.CodeAnalysis;

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

        public static bool TryGetValueFromDescription<T>(string description, [NotNullWhen(true)] out T? enumValue) where T : Enum
        {
            FieldInfo[] fields = typeof(T).GetFields();

            // Note: Matching enum description attribute (not name)
            FieldInfo? field = fields.FirstOrDefault(f => Attribute.GetCustomAttribute(f, typeof(DescriptionAttribute)) is DescriptionAttribute attr && attr.Description == description);

            if (field != null)
            {
                enumValue = (T?)field.GetValue(null);
                return enumValue != null;
            }

            // Must init
            enumValue = default;
            return false;
        }
    }
}
