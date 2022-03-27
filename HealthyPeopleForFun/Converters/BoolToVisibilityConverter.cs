using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HealthyPeopleForFun.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts bool value to Visibility
		/// </summary>
		/// <param name="value">The value produced by the binding source</param>
		/// <param name="targetType">The type of the binding target property</param>
		/// <param name="parameter">The converter parameter to use</param>
		/// <param name="culture">The culture to use in the converter</param>
		public object Convert(Object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// Converts Visibility back to bool value
		/// </summary>
		/// <param name="value">The value that is produced by the binding target</param>
		/// <param name="targetType">The type to convert to</param>
		/// <param name="parameter">The converter parameter to use</param>
		/// <param name="culture">The culture to use in the converter</param>
		public object ConvertBack(Object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible ? true : false;
		}
	}
}
