using Syncfusion.Maui.Charts;

namespace AISalesDashboard;

public partial class RevenueTrendChart : ContentView
{
    private int _currentMonth = int.MaxValue;

    public RevenueTrendChart()
    {
        InitializeComponent();
    }

    private async void OnPeriodSelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.FirstOrDefault() is DateRangeOption selectedOption
            && this.BindingContext is SalesTrendsViewModel salesTrendsViewModel)
        {
            salesTrendsViewModel.SelectedDateRange = selectedOption.Value ?? throw new InvalidOperationException("DateRangeOption Value cannot be null.");
            await salesTrendsViewModel.Initialize();
            await salesTrendsViewModel.LoadDashboardData();
        }
    }

    private void OnDateTimeAxisLabelCreated(object sender, ChartAxisLabelEventArgs e)
    {
        DateTime baseDate = new(1899, 12, 30);
        DateTime date = baseDate.AddDays(e.Position);
        
        ChartAxisLabelStyle labelStyle = new()
        {
            LabelFormat = (date.Month != _currentMonth) ? "MMM-dd" : "dd",
            TextColor = (Application.Current!.UserAppTheme == AppTheme.Dark) ? Color.FromArgb("#E6E1E5") : Colors.Black,
            FontAttributes = (date.Month != _currentMonth) ? FontAttributes.Bold : FontAttributes.None
        };
        
        e.LabelStyle = labelStyle;
        _currentMonth = date.Month;
    }

    private void OnNumericalAxisLabelCreated(object sender, ChartAxisLabelEventArgs e)
    {
        double value = Convert.ToDouble(e.Label);
        e.Label = value switch
        {
            >= 1_000_000_000 => $"{value / 1_000_000_000:0.#}B",
            >= 1_000_000 => $"{value / 1_000_000:0.#}M",
            >= 1_000 => $"{value / 1_000:0.#}K",
            _ => value.ToString("0.#")
        };
    }
}