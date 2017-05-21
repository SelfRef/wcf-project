using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WPFClient.View.ViewModel
{
    [ValueConversion(typeof(Dictionary<string, string>), typeof(string))]
    public class LangStringsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || (parameter as string).Trim() == "")
                return "<Missing ConverterParameter>";
            else if ((value as Dictionary<string, string>).Any(x => x.Key == (parameter as string)))
                return (value as Dictionary<string, string>)[parameter as string];
            else
                return $"<{parameter as string}>";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
