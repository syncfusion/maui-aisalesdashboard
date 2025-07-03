using Syncfusion.Maui.Charts;

namespace AISalesDashboard;

public partial class ProfitMarginChart : ContentView
{
    private int _currentMonth = int.MaxValue;

    public ProfitMarginChart()
    {
        InitializeComponent();
    }

    private void DateTimeAxis_LabelCreated(object sender, ChartAxisLabelEventArgs e)
    {
        DateTime baseDate = new(1899, 12, 30);
        DateTime date = baseDate.AddDays(e.Position);

        ChartAxisLabelStyle labelStyle = new()
        {
            LabelFormat = (date.Month != _currentMonth) ? "MMM-dd" : "dd",
            TextColor = (Application.Current!.UserAppTheme == AppTheme.Dark) ? Color.FromArgb("#E6E1E5") : Colors.Black // or use Colors.Gray or any appropriate default color
        };

        e.LabelStyle = labelStyle;
        _currentMonth = date.Month;
    }
}