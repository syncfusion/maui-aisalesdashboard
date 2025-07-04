using Syncfusion.Maui.Buttons;
using Syncfusion.Maui.Charts;
using Syncfusion.Maui.Themes;

namespace AISalesDashboard;

public partial class PredictionDetails : ContentView
{

#if WINDOWS || MACCATALYST
    int _currentMonth = int.MaxValue;
#endif

    public PredictionDetails()
    {
        InitializeComponent();
        InitializeSegmentedControl();
    }

    private void InitializeSegmentedControl()
    {

        var selectedColor = (Color)(Application.Current!.RequestedTheme == AppTheme.Dark
                            ? Application.Current.Resources["SalesDemoContentLabelTextColorDark"]
                            : Application.Current.Resources["SalesDemoContentLabelTextColor"]);

        segmentedControl.ItemsSource = new List<SfSegmentItem>
        {
            new SfSegmentItem { Text = "\ue820", TextStyle = new SegmentTextStyle { FontFamily = "MauiSampleFontIcon", FontSize = 16 , TextColor = selectedColor} },
            new SfSegmentItem { Text = "\ue829", TextStyle = new SegmentTextStyle { FontFamily = "MauiSampleFontIcon", FontSize = 16 , TextColor = selectedColor} }
        };
    }

    private void OnDateTimeAxisLabelCreated(object sender, ChartAxisLabelEventArgs e)
    {

#if WINDOWS || MACCATALYST

        DateTime baseDate = new(1899, 12, 30);
        DateTime date = baseDate.AddDays(e.Position);

        ChartAxisLabelStyle labelStyle = new()
        {
            LabelFormat = (date.Month != _currentMonth) ? "MMM-dd" : "dd",
            TextColor = (Application.Current!.UserAppTheme == AppTheme.Dark) ? Color.FromArgb("#E6E1E5") : Colors.Black,
        };

        e.LabelStyle = labelStyle;
        _currentMonth = date.Month;
#endif
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

    private void OnSegmentedControlSelectionChanged(object sender, Syncfusion.Maui.Buttons.SelectionChangedEventArgs e)
    {
        InitializeSegmentedControl();
        
        if (e.NewIndex == 0)
        {
            chart.IsVisible = true;
            dataGrid.IsVisible = false;
        }
        else if (e.NewIndex == 1)
        {
            chart.IsVisible = false;
            dataGrid.IsVisible = true;
        }
    }
}