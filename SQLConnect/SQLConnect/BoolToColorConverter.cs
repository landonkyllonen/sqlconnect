using System;
using System.Globalization;
using Xamarin.Forms;

namespace SQLConnect
{
	public class BoolToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				if ((bool)value)
				{
					return Color.Gray;
				}
				else
				{
					return Color.Teal;
				}
			}
			else { return value; }
			
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

	}
}
