using Syncfusion.Maui.AIAssistView;
using Syncfusion.Maui.Themes;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AISalesDashboard;

public partial class DesktopUI : ContentView
{
    private readonly SalesTrendsViewModel _salesTrendsViewModel;
    private readonly PredictionViewModel _predictionViewModel;
    private readonly SalesDataService _salesDataService;
    private readonly AzureBaseService _baseAIService;
    private readonly PredictionService _predictionService;

    public DesktopUI()
    {
        InitializeComponent();
        _salesDataService = new SalesDataService();
        _baseAIService = new AzureBaseService();
        _predictionService = new PredictionService(_baseAIService, _salesDataService);

        _salesTrendsViewModel = new SalesTrendsViewModel(_salesDataService);
        _predictionViewModel = new PredictionViewModel(_predictionService, _salesDataService);

        InitializeViews();
        BindingContext = _salesTrendsViewModel;
    }

    private void InitializeViews()
    {
        var salesChart = new SalesChart { BindingContext = _salesTrendsViewModel };
        var productDetails = new ProductDetails { BindingContext = _salesTrendsViewModel };
        var orderDetails = new OrderDetails { BindingContext = _salesTrendsViewModel };
        var predictionView = new Prediction { BindingContext = _predictionViewModel };

        _salesTrendsViewModel.OnPageSelected += viewName =>
            LoadContentView(viewName, salesChart, productDetails, orderDetails, predictionView);

        main.Content = salesChart;
    }

    private void LoadContentView(string viewName, SalesChart sales, ProductDetails productDetails, OrderDetails orderDetails, Prediction prediction)
    {
        main.Content = viewName switch
        {
            "Products" => productDetails,
            "Orders" => orderDetails,
            "Predictions" => prediction,
            _ => sales,
        };
    }

    private async void AiAssistView_Request(object sender, RequestEventArgs e)
    {
        await GetResultsAsync(e.RequestItem).ConfigureAwait(true);
    }

    private async Task GetResultsAsync(object inputQuery)
    {
        await Task.Delay(500).ConfigureAwait(true);

        if (inputQuery is AssistItem request)
        {
            var prompt = GeneratePrompt(request.Text, _salesTrendsViewModel.SalesData!);
            var aiResponse = await _baseAIService.GetAnswerFromGPT(prompt);

            _salesTrendsViewModel.Messages!.Add(new AssistItem
            {
                Text = CleanAndFormatOutput(aiResponse),
            });
        }
    }

    public static string GeneratePrompt(string customerQuery, ObservableCollection<SalesData> salesData)
    {
        if (string.IsNullOrWhiteSpace(customerQuery))
            return "Please provide a valid query related to sales.";

        if (salesData == null || !salesData.Any())
            return $"Customer query: \"{customerQuery}\". Unfortunately, there is no sales data available.";

        var summary = salesData
            .GroupBy(s => s.ProductName)
            .Select(g => new
            {
                ProductName = g.Key,
                TotalSales = g.Sum(s => s.Cost),
                TotalProfit = g.Sum(s => s.Profit)
            })
            .ToList();

        var formattedSummary = string.Join("; ",
            summary.Select(s => $"{s.ProductName}: {s.TotalSales} total sales, {s.TotalProfit} total profit"));

        return $"Customer query: \"{customerQuery}\". Based on historical sales data, here are the key insights: {formattedSummary}.";
    }

    private static string CleanAndFormatOutput(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
            return string.Empty;

        aiResponse = aiResponse
            .Replace("####", "")
            .Replace("###", "")
            .Replace("**", "")
            .Replace("/", "")
            .Replace("[", "")
            .Replace("]", "");

        aiResponse = Regex.Replace(aiResponse, @"(\d+\.\s)([A-Za-z\s]+)",
            m => $"{m.Groups[1].Value}{m.Groups[2].Value.Trim()}");

        return aiResponse.Trim();
    }

    private void TogglePopup(bool isVisible)
    {
        _salesTrendsViewModel.ShowAssistView = isVisible;
        assistViewHeader.IsVisible = isVisible;
        assistViewBorder.IsVisible = isVisible;
        clickToShowPopup.IsVisible = !isVisible;
        close.IsVisible = isVisible;
    }

    private void ClickToShowPopup_Clicked(object sender, EventArgs e)
    {
        TogglePopup(true);
    }

    private void Close_Clicked(object sender, EventArgs e)
    {
        TogglePopup(false);
    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        headerLabel.Text = e.SelectedItemIndex switch
        {
            0 => "Home",
            1 => "Products",
            2 => "Orders",
            3 => "Predictions",
            _ => headerLabel.Text
        };
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        navigationDrawer.ToggleDrawer();
    }

    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        var currentTheme = Application.Current?.UserAppTheme;
        var value = sender as Switch;
        if (Application.Current != null)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                var theme = mergedDictionaries.OfType<Syncfusion.Maui.Themes.SyncfusionThemeResourceDictionary>().FirstOrDefault();
                if (theme != null && value != null)
                {
                    if (value.IsToggled == true)
                    {
                        theme.VisualTheme = SfVisuals.MaterialDark;
                        Application.Current.UserAppTheme = AppTheme.Dark;
                    }
                    else
                    {
                        theme.VisualTheme = SfVisuals.MaterialLight;
                        Application.Current.UserAppTheme = AppTheme.Light;
                    }
                }
            }
        }
    }
}

public class BoolToDarkThemeColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isSelected = bool.TryParse(value?.ToString(), out var result) && result;

        return isSelected && Application.Current!.UserAppTheme == AppTheme.Dark ? 
            Color.FromArgb("#4A4458") : Colors.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}