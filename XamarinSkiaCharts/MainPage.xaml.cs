using System.Collections.Generic;
using Xamarin.Forms;

namespace XamarinSkiaCharts
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new Dictionary<string,int>()
            {
                {"Apples",5},
                {"Bananas",7 },
                {"Strawberries",4 },
                {"Blueberries",5 },
                {"Oranges",8 },
                {"Grapes",7 },
                {"Watermelons",6 },
                {"Pears",5 },
                {"Cantalopes",8 },
                {"Citrus",5 },
                {"Starfruit",4 },
                {"Papaya",7 },
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //BarChart.LoadChart = true;
        }
    }
}