using System.Globalization;

namespace AISalesDashboard;

public partial class ProductChart : ContentView
{
	public ProductChart()
	{
		InitializeComponent();
	}
}

public class SegmentColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            string stage => GetColorForStage(stage),
            Product model when parameter is string price && price == "BasePrice" => model.BasePrice,
            _ => Colors.Gray
        };
    }

    private static Color GetColorForStage(string stage) => stage switch
    {
        "Closed" => Color.FromArgb("#F7C066"),
        "Resolved" => Color.FromArgb("#1F77B4"),
        "Waiting Response" => Color.FromArgb("#27AE60"),
        "Awaiting Customer" => Color.FromArgb("#FF6F61"),
        "In Progress" => Color.FromArgb("#F8D74B"),
        "New Tickets" => Color.FromArgb("#6B5B95"),
        _ => Colors.Gray
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}