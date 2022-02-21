using System.Collections.Generic;
using Xamarin.Forms;

namespace XamarinSkiaCharts
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new List<int>()
            {
                3,7,3,6,4,9,3,7,5,1,6,4,7
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //BarChart.LoadChart = true;
        }
    }
}