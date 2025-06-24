using Syncfusion.Maui.AIAssistView;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace AISalesDashboard;

public partial class AndroidUI : ContentView
{
    private readonly SalesTrendsViewModel salesTrendsViewModel;
    private readonly HomeAndroid sales;
    private readonly ProductDetailsAndroid productDetails;
    private readonly SalesDataService salesDataService;
    private readonly AzureBaseService baseAIService;
    private readonly PredictionService predictionService;
    private readonly PredictionViewModel predictionViewModel;
    private readonly OrderDetailsAndroid orderDetails;
    private readonly PredictionAndroid prediction;

    public AndroidUI()
    {
		InitializeComponent();
        salesDataService = new SalesDataService();
        baseAIService = new AzureBaseService();
        predictionService = new PredictionService(baseAIService, salesDataService);
        predictionViewModel = new PredictionViewModel(predictionService, salesDataService);
        salesTrendsViewModel = new SalesTrendsViewModel(salesDataService);

        sales = CreateAndroidView<HomeAndroid>();
        productDetails = CreateAndroidView<ProductDetailsAndroid>();
        orderDetails = CreateAndroidView<OrderDetailsAndroid>();
        prediction = new PredictionAndroid() { BindingContext = predictionViewModel };
        mainContentView.Content = sales;
        this.BindingContext = salesTrendsViewModel;
    }

    private T CreateAndroidView<T>() where T : ContentView, new()
    {
        return new T() { BindingContext = salesTrendsViewModel };
    }

    private async void AiAssistView_Request(object sender, RequestEventArgs e)
    {
        var request = e.RequestItem;
        await GetResults(request);
    }

    private async Task GetResults(object inputQuery)
    {
        await Task.Delay(500).ConfigureAwait(true);
        AssistItem request = (AssistItem)inputQuery;

        if (request != null)
        {
            var prompt = GeneratePrompt(request.Text, salesTrendsViewModel.SalesData!);

            // Get AI response
            string aiResponse = await baseAIService.GetAnswerFromGPT(prompt);

            AssistItem assistItem = new AssistItem()
            {
                Text = CleanAndFormatOutput(aiResponse),
            };

            salesTrendsViewModel.Messages!.Add(assistItem);
        }
    }

    public string GeneratePrompt(string customerQuery, ObservableCollection<SalesData> salesData)
    {
        if (string.IsNullOrWhiteSpace(customerQuery))
            return "Please provide a valid query related to sales.";

        if (salesData == null || !salesData.Any())
            return $"Customer query: \"{customerQuery}\". Unfortunately, there is no sales data available.";

        // Group sales by product and calculate total sales and profit
        var summary = salesData
            .GroupBy(s => s.ProductName)
            .Select(g => new
            {
                ProductName = g.Key,
                TotalSales = g.Sum(s => s.Cost),
                TotalProfit = g.Sum(s => s.Profit)
            })
            .ToList();

        // Format the summary data for the AI prompt
        string formattedSummary = string.Join("; ",
            summary.Select(s => $"{s.ProductName}: {s.TotalSales} total sales, {s.TotalProfit} total profit"));

        // Construct the AI prompt dynamically based on customer query
        return $"Customer query: \"{customerQuery}\". Based on historical sales data, here are the key insights: {formattedSummary}.";
    }

    private string CleanAndFormatOutput(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
            return string.Empty;

        var charactersToRemove = new[] { "####", "###", "**", "/", "[", "]" };
        foreach (var character in charactersToRemove)
        {
            aiResponse = aiResponse.Replace(character, "");
        }

        return Regex.Replace(aiResponse, @"(\d+\.\s)([A-Za-z\s]+)", m =>
            $"{m.Groups[1].Value}{m.Groups[2].Value.Trim()}").Trim();
    }

    private void ClickToShowPopup_Clicked(object sender, EventArgs e)
    {
        salesTrendsViewModel.ShowAssistView = true;
        clickToShowPopup.IsVisible = false;
        mainContentView.IsVisible = false;
        menuBar.IsVisible = false;
        assistviewContent.IsVisible = true;
    }

    private void SfButton_Clicked(object sender, EventArgs e)
    {
        assistviewContent.IsVisible = false;
        mainContentView.IsVisible = true;
        clickToShowPopup.IsVisible = true;
        menuBar.IsVisible = true;
    }

    private void OnSectionTapped(object sender, EventArgs e, ContentView contentView, string sectionName)
    {
        mainContentView.Content = contentView;
        UpdateUI(sectionName);
    }

    private void OnHomeTapped(object sender, EventArgs e) => OnSectionTapped(sender, e, sales, "Home");
    private void OnBoxTapped(object sender, EventArgs e) => OnSectionTapped(sender, e, productDetails, "Box");
    private void OnCartTapped(object sender, EventArgs e) => OnSectionTapped(sender, e, orderDetails, "Cart");
    private void OnAnalyticsTapped(object sender, EventArgs e) => OnSectionTapped(sender, e, prediction, "Analytics");

    private void UpdateUI(string activeSection)
    {
        var lightBackground = Color.FromArgb("#F6EDFF");
        var darkBackground = Color.FromArgb("#381E72");
        var lightTextColor = Color.FromArgb("#6750A4");
        var darkTextColor = Color.FromArgb("#D0BCFF");
        var inactiveLightTextColor = Color.FromArgb("#FFFFFF");
        var inactiveDarkTextColor = Color.FromArgb("#381E72");

        bool isLightTheme = Application.Current!.UserAppTheme == AppTheme.Light ||
                            Application.Current!.UserAppTheme == AppTheme.Unspecified;

        var inactiveTextColor = isLightTheme ? inactiveLightTextColor : inactiveDarkTextColor;


        homeBorder.Background = new SolidColorBrush(Colors.Transparent);
        homeIcon.TextColor = inactiveTextColor;
        productBorder.Background = new SolidColorBrush(Colors.Transparent);
        productIcon.TextColor = inactiveTextColor;
        cartBorder.Background = new SolidColorBrush(Colors.Transparent);
        cartIcon.TextColor = inactiveTextColor;
        chartBorder.Background = new SolidColorBrush(Colors.Transparent);
        chartIcon.TextColor = inactiveTextColor;

        switch (activeSection)
        {
            case "Home":
                homeBorder.Background = new SolidColorBrush(isLightTheme ? lightBackground : darkBackground);
                homeIcon.TextColor = isLightTheme ? lightTextColor : darkTextColor;
                break;
            case "Box":
                productBorder.Background = new SolidColorBrush(isLightTheme ? lightBackground : darkBackground);
                productIcon.TextColor = isLightTheme ? lightTextColor : darkTextColor;
                break;
            case "Cart":
                cartBorder.Background = new SolidColorBrush(isLightTheme ? lightBackground : darkBackground);
                cartIcon.TextColor = isLightTheme ? lightTextColor : darkTextColor;
                break;
            case "Analytics":
                chartBorder.Background = new SolidColorBrush(isLightTheme ? lightBackground : darkBackground);
                chartIcon.TextColor = isLightTheme ? lightTextColor : darkTextColor;
                break;
        }
    }
}

