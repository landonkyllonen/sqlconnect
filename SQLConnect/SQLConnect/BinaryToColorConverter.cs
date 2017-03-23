using System;
using System.Globalization;
using Xamarin.Forms;

namespace SQLConnect
{
	public class BinaryToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string)
			{
				string valueAsString = value.ToString();
				switch (valueAsString)
				{
					case ("0"):
						{
							return Color.Teal;
						}
					case ("1"):
						{
							return Color.Gray;
						}
					default:
						{
							return value;
						}
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
