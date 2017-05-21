using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFClient.View.ViewModel
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class VisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.Parse((string)parameter) == 0)
            {
                if ((int)value == 0) return Visibility.Collapsed;
                else return Visibility.Visible;
            }
            else if (int.Parse((string)parameter) == 1)
            {
                if ((int)value == 0) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
            else
            {
                if ((bool)value == false) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
