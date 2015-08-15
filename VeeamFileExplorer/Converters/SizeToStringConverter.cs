using System;
using System.Globalization;
using System.Windows.Data;

namespace VeeamFileExplorer.Converters
{
    class SizeToStringConverter : IValueConverter
    {
        public static SizeToStringConverter Instance = new SizeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = (long) value;
            if (size == 0) return String.Empty;

            string s = String.Concat(size, " B");

            if (size > 1024)
            {
                s = String.Concat((size / 1024), " KB");
            }
            if (size > 1024 * 1024)
            {
                s = String.Concat((size / 1024 / 1024), " MB");
            }
            if (size > 1024 * 1024 * 1024)
            {
                double doubleSize = (double)size / 1024 / 1024 / 1024;

                s = String.Concat(String.Format("{0:F2}", doubleSize), " GB");
            }

            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
