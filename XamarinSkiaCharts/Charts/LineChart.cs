using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamarinSkiaCharts.Charts
{
    public class LineChart : SKCanvasView
    {
        public static readonly BindableProperty PointsProperty = BindableProperty.Create(nameof(Points),
            typeof(List<int>),
            typeof(LineChart),
            new List<int>(),
            propertyChanged: async (bindable, oldValue, newValue) =>
            {
                var chart = ((LineChart)bindable);

                chart.Max = chart.Points?.Max() + 1 ?? 0;
            });

        public List<int> Points
        {
            get => (List<int>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public LineChart()
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
            const int POINT_SEGMENT_WIDTH = 100;

            if (_moved)
            {
                _firstPointXAxis += _xMoved;
            }

            var pointXAxis = _firstPointXAxis;

            using (var paint = new SKPaint())
            {
                // Create gradient for background
                var transparentMauiPurpleColor = new SKColor(0XB2, 0X7F, 0XFF, 0X0);
                var mauiPurpleColor = new SKColor(0XB2, 0X7F, 0XFF);

                //paint.Color = SKColors.Red;
                paint.Style = SKPaintStyle.Fill;
                paint.Shader = SKShader.CreateLinearGradient(
                                    new SKPoint(0, 0),
                                    new SKPoint(info.Width, info.Height),
                                    new SKColor[] { mauiPurpleColor, transparentMauiPurpleColor },
                                    null,
                                    SKShaderTileMode.Clamp);

                var linearPath = new SKPath();
                for (var i = 0; i < Points.Count; i++)
                {
                    var yAxis = info.Height - (info.Height * (Points[i] / Max));
                    if (i == 0)
                    {
                        linearPath.MoveTo(new SKPoint(pointXAxis, yAxis));
                    }
                    else 
                    {
                        linearPath.LineTo(new SKPoint(pointXAxis, yAxis));
                    }
                    pointXAxis += POINT_SEGMENT_WIDTH + 20;

                    if (i == Points.Count - 1)
                        _lastPointXAxis = pointXAxis + POINT_SEGMENT_WIDTH;
                }
                linearPath.LineTo(new SKPoint((Points.Count - 1) * POINT_SEGMENT_WIDTH, info.Height));
                linearPath.LineTo(new SKPoint(0, info.Height));

                //
                linearPath.Close();
                canvas.DrawPath(linearPath, paint);
            }
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            base.OnTouch(e);
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    _moved = true;
                    _xOrigin = e.Location.X;
                    break;
                default:
                    //var scrolled = (_xOrigin - e.Location.X) * -1;
                    //var scrolledToLeftChartEdge = _firstBarXAxis + scrolled >= 40;
                    //var scrolledToRightChartEdge = _lastBarXAxis + scrolled <= _chartWidth;
                    //if (scrolledToLeftChartEdge || scrolledToRightChartEdge)
                    //    return;

                    _xMoved = (_xOrigin - e.Location.X) * -1;
                    _xOrigin = e.Location.X;
                    InvalidateSurface();

                    break;
            }
            e.Handled = true;
        }

        public float Max;
        private float _xOrigin;
        private float _chartWidth;
        private float _xMoved = -1;
        private bool _moved = false;
        private float _lastPointXAxis;
        private float _firstPointXAxis = 20.0f;
    }
}