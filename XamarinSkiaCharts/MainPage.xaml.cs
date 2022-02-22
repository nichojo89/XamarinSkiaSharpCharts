using System.Collections.Generic;
using Xamarin.Forms;

namespace XamarinSkiaCharts
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new Dictionary<string,float>()
            {
                {"Apples",5.0f},
                {"Bananas",7.0f },
                {"Strawberries",4.0f },
                {"Blueberries",5.0f },
                {"Oranges",8.0f },
                {"Grapes",7.0f },
                {"Watermelons",6.0f },
                {"Pears",5.0f },
                {"Cantalopes",8.0f },
                {"Citrus",5.0f },
                {"Starfruit",4.0f },
                {"Papaya",7.0f },
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BarChart.LoadChart = true;
        }
    }
}