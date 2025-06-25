using System.Globalization;

namespace AISalesDashboard;

public partial class SalesDataGrid : ContentView
{
	public SalesDataGrid()
	{
		InitializeComponent();
	}
}

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {

        if (value == null)
        {
            return null;
        }

        bool isEnabled = (bool)value;

        if (App.Current!.RequestedTheme == AppTheme.Light)
        {
            return isEnabled ? null : Color.FromArgb("#E0E0E0");
        }
        else
        {
            return isEnabled ? null : Color.FromArgb("#5A5A5A");
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}