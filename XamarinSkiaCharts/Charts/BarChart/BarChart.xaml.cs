using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinSkiaCharts.Charts
{
    public partial class BarChart : StackLayout
    {
        public static readonly BindableProperty PointsProperty = BindableProperty.Create(nameof(Points),
            typeof(Dictionary<string, float>),
            typeof(BarChart),
            new Dictionary<string, float>(),
            propertyChanged: async (bindable, oldValue, newValue) =>
            {
                var chartView = ((BarChart)bindable);

                chartView.Chart.Max = chartView.Points?.Select(x => x.Value).Max() * 1.1f ?? 0.0f;
                chartView.Chart.Points = (Dictionary<string, float>)newValue;
            });

        public Dictionary<string, float> Points
        {
            get => (Dictionary<string, float>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public BarChart()
        {
            InitializeComponent();
        }

        public async Task LoadChart() => await Chart.LoadChartAnimation();
    }
}