using System;
using System.Globalization;
using System.Windows.Data;

namespace VeeamFileExplorer.Converters
{
    class DateTimeToStringConverter : IValueConverter
    {
        public static DateTimeToStringConverter Instance = new DateTimeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime) value;

            //Adding zero behind a number if it is less than ten for plain visual output
            string day = dt.Day < 10 ? String.Concat(0, dt.Day) : dt.Day.ToString();
            string month = dt.Month < 10 ? String.Concat(0, dt.Month) : dt.Month.ToString();
            string year = dt.Year.ToString();
            string hour = dt.Hour < 10 ? String.Concat(0, dt.Hour) : dt.Hour.ToString();
            string minute = dt.Minute < 10 ? String.Concat(0, dt.Minute) : dt.Minute.ToString();

            return String.Concat(day, ".", month, ".", year, " ", hour, ":", minute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
