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
                {"Apples",25},
                {"Bananas",13},
                {"Strawberries",25},
                {"Blueberries", 53},
                {"Oranges", 14},
                {"Grapes", 52},
                {"Watermelons", 15},
                {"Pears",34 },
                {"Cantalopes", 67},
                {"Citrus",53 },
                {"Starfruit", 43},
                {"Papaya", 22},
            };
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //This is only neccesary because our chart is the first view that loads when app starts
            //If you want to use this and the bar chart is not on the very first view then you can
            //Remove all the "LoadChart" properties to get this to work without it
            await BarChart.LoadChart();
        }
    }
}