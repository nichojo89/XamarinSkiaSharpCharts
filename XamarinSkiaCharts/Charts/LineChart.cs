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
            typeof(Dictionary<string,float>),
            typeof(LineChart),
            new Dictionary<string, float>(),
            propertyChanged: async (bindable, oldValue, newValue) =>
            {
                var chart = ((LineChart)bindable);

                chart.Max = chart.Points?.Select(x => x.Value).Max() + 1 ?? 0;
            });

        public Dictionary<string, float> Points
        {
            get => (Dictionary<string, float>)GetValue(PointsProperty);
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
                _firstPointXAxis += _xMoved;

            var pointXAxis = _firstPointXAxis;
            var linearPath = new SKPath();

            // Create gradient for background
            var transparentMauiPurpleColor = new SKColor(0XB2, 0X7F, 0XFF, 0X0);
            var mauiPurpleColor = new SKColor(0XB2, 0X7F, 0XFF, 0XAF);

            using (var gradientPaint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                StrokeCap = SKStrokeCap.Round,
                Shader = SKShader.CreateLinearGradient(
                                    new SKPoint(info.Rect.Left, info.Rect.MidY),
                                    new SKPoint(info.Rect.Right, info.Rect.MidY),
                                    new SKColor[] {transparentMauiPurpleColor, mauiPurpleColor },
                                    new float[] { 0, 1 },
                                    SKShaderTileMode.Decal)
            })
            {
                using (var textPaint = new SKPaint
                {
                    TextSize = 30,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Color = Color.FromHex("#7F2CF6").ToSKColor()
                })
                {
                    //Generate path
                    for (var i = 0; i < Points.Count; i++)
                    {
                        var point = Points.ElementAt(i);
                        var yAxis = info.Height - (info.Height * (point.Value / Max));

                        if (i == 0)
                        {
                            linearPath.MoveTo(new SKPoint(pointXAxis, yAxis));
                        }
                        else
                        {
                            linearPath.LineTo(new SKPoint(pointXAxis, yAxis));
                        }

                        canvas.DrawCircle(new SKPoint(pointXAxis, yAxis), 10, gradientPaint);

                        var isLastDataPoint = i == Points.Count - 1;

                        //Draw text
                        var pointText = $"{point.Key}: {point.Value}";
                        canvas.DrawText(pointText,
                            new SKPoint(
                            isLastDataPoint || i == Points.Count - 2
                            ? pointXAxis - gradientPaint.MeasureText(pointText) - 20
                            : pointXAxis + 20,
                            yAxis - 10), textPaint);

                        //Remember last point x axis
                        if (isLastDataPoint)
                            _lastPointXAxis = pointXAxis;

                        //Move x axis to next point
                        pointXAxis += POINT_SEGMENT_WIDTH + 20;
                    }
                }

                    //Draw Line
                    gradientPaint.Style = SKPaintStyle.Stroke;
                gradientPaint.StrokeWidth = 7;
                canvas.DrawPath(linearPath, gradientPaint);
            

                linearPath.LineTo(new SKPoint(_lastPointXAxis, info.Height));
                linearPath.LineTo(new SKPoint(0, info.Height));

            
                linearPath.Close();
                

                gradientPaint.Style = SKPaintStyle.Fill;
                gradientPaint.Shader = SKShader.CreateLinearGradient(
                                    new SKPoint(info.Rect.MidX, info.Rect.Top),
                                    new SKPoint(info.Rect.MidX, info.Rect.Bottom),
                                    new SKColor[] { mauiPurpleColor, transparentMauiPurpleColor },
                                    new float[] { 0, 1 },
                                    SKShaderTileMode.Decal);

                canvas.DrawPath(linearPath, gradientPaint);
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
                    var scrolled = (_xOrigin - e.Location.X) * -1;
                    var scrolledToLeftChartEdge = _firstPointXAxis + scrolled >= 0;
                    var scrolledToRightChartEdge = _lastPointXAxis + scrolled <= _chartWidth;
                    if (scrolledToLeftChartEdge || scrolledToRightChartEdge)
                        return;

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
        private float _firstPointXAxis = 0.0f;
    }
}