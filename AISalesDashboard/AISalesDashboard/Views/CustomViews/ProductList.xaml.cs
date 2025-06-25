using Syncfusion.Maui.DataGrid.Exporting;
using System.Globalization;

namespace AISalesDashboard;

public partial class ProductList : ContentView
{
    private int toggleCount = 1;

    public ProductList()
    {
        InitializeComponent();
    }

    private void OnSfButtonClicked(object sender, EventArgs e)
    {
        bool shouldShowDropdown = toggleCount % 2 != 0;
        customdropdown.IsVisible = shouldShowDropdown;
        
        // Choose appropriate background color based on theme
        customComboBox.Background = shouldShowDropdown
            ? GetComboBoxBackground()
            : Colors.Transparent;

        toggleCount++;
    }

    private Color GetComboBoxBackground()
    {
        return (Color)(Application.Current!.RequestedTheme == AppTheme.Dark
            ? Application.Current.Resources["SalesDemoComboboxIconBackgroundAndroidDark"]
            : Application.Current.Resources["SalesDemoComboboxIconBackgroundAndroid"]);
    }

    private void OnExportPdfTapped(object sender, TappedEventArgs e)
    {
        using var stream = new MemoryStream();
        var pdfExport = new DataGridPdfExportingController();
        var pdfDocument = pdfExport.ExportToPdf(this.dataGrid, new DataGridPdfExportingOption());

        pdfDocument.Save(stream);
        pdfDocument.Close(true);

        SaveToFile("ExportFeature.pdf", "application/pdf", stream);
    }

    private void SaveToFile(string filename, string mimeType, MemoryStream stream)
    {
        var saveService = new SaveService();
        saveService.SaveAndView(filename, mimeType, stream);
    }
}

public class ProductStockCountContentBackground : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double stockCount && parameter is string param)
        {
            return param switch
            {
                "Background" => GetBackgroundColor(stockCount),
                "TextColor" => GetTextColor(stockCount),
                _ => Colors.Transparent
            };
        }
        return Colors.Transparent;
    }

    private static Color GetBackgroundColor(double stockCount)
    {
        return stockCount < 10 ? Color.FromArgb("#FEF2F2") : Color.FromArgb("#F0FDF4");
    }

    private static Color GetTextColor(double stockCount)
    {
        return stockCount < 10 ? Color.FromArgb("#DC2626") : Color.FromArgb("#15803D");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ProductIdFormatter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is int id ? $"#P{id:D4}" : value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && str.StartsWith("P"))
        {
            if (int.TryParse(str.Substring(1), out int result))
                return result;
        }
        return value;
    }
}