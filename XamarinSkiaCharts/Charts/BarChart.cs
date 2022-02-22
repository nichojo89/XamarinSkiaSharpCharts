using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamarinSkiaCharts.Charts
{
    public class BarChart : SKCanvasView
    {
        public static readonly BindableProperty LoadChartProperty = BindableProperty.Create(nameof(LoadChart),
            typeof(bool),
            typeof(BarChart),
            false,
            propertyChanged: async (bindable, oldValue, newValue) =>
            {
                if ((bool)newValue)
                    await ((BarChart)bindable).LoadChartAnimation();
            });

        public bool LoadChart
        {
            get => (bool)GetValue(LoadChartProperty);
            set => SetValue(LoadChartProperty, value);
        }

        public static readonly BindableProperty PointsProperty = BindableProperty.Create(nameof(Points),
            typeof(Dictionary<string, float>),
            typeof(BarChart),
            new Dictionary<string, float>(),
            propertyChanged: async (bindable, oldValue, newValue) =>
            {
                var chart = ((BarChart)bindable);

                chart.Max = chart.Points?.Select(x => x.Value).Max() + 1 ?? 0.0f;
                if (!chart.ChartsLoading)
                {
                    //New data added, re-render chart without loading animation
                    ((BarChart)bindable).InvalidateSurface();
                }
            });

        public Dictionary<string, float> Points
        {
            get => (Dictionary<string, float>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public BarChart()
        {
            VerticalOptions = LayoutOptions.FillAndExpand;
            EnableTouchEvents = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var info = e.Info;
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            _chartWidth = info.Width;
            const int BAR_WIDTH = 100;

            if (_moved)
                _firstBarXAxis += _xMoved;

            var barXAxis = _firstBarXAxis;
            
            using (var paint = new SKPaint())
            {
                // Create gradient for background
                var transparentMauiPurpleColor = new SKColor(0XB2, 0X7F, 0XFF, 0X0);
                var mauiPurpleColor = new SKColor(0XB2, 0X7F, 0XFF);
                paint.Shader = SKShader.CreateLinearGradient(
                                    new SKPoint(0, 0),
                                    new SKPoint(info.Width, info.Height),
                                    new SKColor[] { mauiPurpleColor, transparentMauiPurpleColor },
                                    null,
                                    SKShaderTileMode.Clamp);

                for (var i = 0; i < Points.Count; i++)
                {
                    var point = Points.ElementAt(i).Value;
                    var barHeight = info.Height - (info.Height * (point / Max) * _barScale);
                    var bar = new SKRect(barXAxis, barHeight, barXAxis + BAR_WIDTH, info.Height);

                    //Draw bars
                    canvas.DrawRect(bar, paint);
                    barXAxis += BAR_WIDTH + 20;

                    if (i == Points.Count - 1)
                        _lastBarXAxis = barXAxis + BAR_WIDTH;
                }

            }
        }

        /// <summary>
        /// Touching canvas allows horizontal scrolling
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTouch(SKTouchEventArgs e)
        {
            base.OnTouch(e);
            if (!ChartsLoading)
            {
                switch (e.ActionType)
                {
                    case SKTouchAction.Pressed:
                        _moved = true;
                        _xOrigin = e.Location.X;
                        break;
                    default:
                        var scrolled = (_xOrigin - e.Location.X) * -1;
                        var scrolledToLeftChartEdge = _firstBarXAxis + scrolled >= 40;
                        var scrolledToRightChartEdge = _lastBarXAxis + scrolled <= _chartWidth;
                        if (scrolledToLeftChartEdge || scrolledToRightChartEdge)
                            return;

                        _xMoved = (_xOrigin - e.Location.X) * -1;
                        _xOrigin = e.Location.X;
                        InvalidateSurface();

                        break;
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Animates bars from 1/30 scale over 1 second
        /// </summary>
        public async Task LoadChartAnimation()
        {
            for (var i = 0; i <= 30; i++)
            {
                _barScale = i / 30f;
                InvalidateSurface();
                await Task.Delay(33);
            }
            ChartsLoading = false;
        }

        public float Max;
        public bool ChartsLoading = true;

        private float _xOrigin;
        private float _chartWidth;
        private float _xMoved = -1;
        private float _lastBarXAxis;
        private bool _moved = false;
        private float _barScale = 0.0f;
        private float _firstBarXAxis = 20.0f;
    }
}