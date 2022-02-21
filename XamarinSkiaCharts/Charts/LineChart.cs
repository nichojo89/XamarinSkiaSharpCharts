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
                const int segmentLength = 100;
                for (var i = 0; i < Points.Count; i++)
                {
                    var yAxis = info.Height - (info.Height * (Points[i] / Max));
                    if (i == 0)
                    {
                        linearPath.MoveTo(new SKPoint(0, yAxis));
                    }
                    else 
                    {
                        linearPath.LineTo(new SKPoint(segmentLength * i, yAxis));
                    }
                }
                linearPath.LineTo(new SKPoint((Points.Count - 1) * segmentLength, info.Height));
                linearPath.LineTo(new SKPoint(0, info.Height));

                //
                linearPath.Close();
                canvas.DrawPath(linearPath, paint);
            }
        }

        public float Max;
    }
}