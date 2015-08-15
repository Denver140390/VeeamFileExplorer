using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace VeeamFileExplorer.Converters
{
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string).Contains(@"\"))
            {
                var uri = new Uri("pack://application:,,,/Images/diskdrive.png");
                var source = new BitmapImage(uri);
                return source;
            }
            else
            {
                var uri = new Uri("pack://application:,,,/Images/folder.png");
                var source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
