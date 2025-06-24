using Syncfusion.Maui.DataGrid.Exporting;

namespace AISalesDashboard;

public partial class OrderList : ContentView
{
    private int toggleCount = 1;

    public OrderList()
    {
        InitializeComponent();
    }

    private void SfButton_Clicked(object sender, EventArgs e)
    {
        bool shouldShowDropdown = toggleCount % 2 != 0;
        customDropdown.IsVisible = shouldShowDropdown;

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

    private void ExportAsPDF(object sender, TappedEventArgs e)
    {
        using var stream = new MemoryStream();
        var pdfExport = new DataGridPdfExportingController();
        var pdfDocument = pdfExport.ExportToPdf(dataGrid, new DataGridPdfExportingOption());

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