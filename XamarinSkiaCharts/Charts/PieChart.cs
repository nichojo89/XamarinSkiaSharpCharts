using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamarinSkiaCharts.Charts
{
    public class PieChart : SKCanvasView
    {
        public static readonly BindableProperty PointsProperty = BindableProperty.Create(nameof(Points),
            typeof(Dictionary<string, float>),
            typeof(PieChart),
            new Dictionary<string, float>());

        public Dictionary<string, float> Points
        {
            get => (Dictionary<string, float>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public PieChart()
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

            var radius = (info.Width / 2) - 170;
            var center = new SKPoint(info.Rect.MidX, info.Rect.MidY);
            var purple = new SKColor(0XB2, 0X7F, 0XFF);
            var translucent = new SKColor(0XB2, 0X7F, 0XFF, 0X0);
            //Draw Circle
            using (var innerPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateRadialGradient(
                center,
                radius,
                new SKColor[] { translucent, purple},
                null,
                SKShaderTileMode.Clamp)
            })
            {
                canvas.DrawCircle(center, radius, innerPaint);
            };

            //Draw splits into pie using 𝝅
            using (var borderPaint = new SKPaint()
            {
                StrokeWidth = _borderWidth,
                Style = SKPaintStyle.Stroke,
                StrokeCap = SKStrokeCap.Round,
                Color = Color.White.ToSKColor()
            } )
            {
                var scale = 100f / Points.Select(x => x.Value).Sum();
                
                //Draw first initial line
                canvas.DrawLine(
                    new SKPoint(center.X, center.Y - radius),
                    center,
                    borderPaint);

                var lineDegrees = 0f;
                var textDegrees = 0f;

                using (var textPaint = new SKPaint
                {
                    TextSize = 30,
                    Color = purple,
                    TextAlign = SKTextAlign.Center
                })
                {
                    for (var i = 0; i < Points.Count; i++)
                    {
                        var point = Points.ElementAt(i);
                        lineDegrees += 360 * (point.Value * scale / 100);
                        textDegrees += (360 * (point.Value * scale / 100) / 2);

                        var lineStartingPoint = PointFromDegrees(lineDegrees, radius, info.Rect);
                        var textPoint = PointFromDegrees(textDegrees, radius, info.Rect, 60);
                        var valuePoint = new SKPoint(textPoint.X, textPoint.Y + 30);

                        canvas.DrawLine(
                            lineStartingPoint,
                            center,
                            borderPaint);

                        canvas.DrawText(point.Key,
                            textPoint,
                            textPaint);

                        canvas.DrawText(point.Value.ToString(),
                            valuePoint,
                            textPaint);

                        textDegrees += (360 * (point.Value * scale / 100) / 2);
                    }
                }
            }
        }

        /// <summary>
        /// Converts degrees around a circle to a Point
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="radius"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private SKPoint PointFromDegrees(float degrees, int radius, SKRect rect, int padding = 0)
        {
            const int offset = 90;

            var x = (float)(rect.MidX + (radius + padding) * Math.Cos((degrees - offset) * (Math.PI / 180)));
            var y = (float)(rect.MidY + (radius + padding) * Math.Sin((degrees - offset) * (Math.PI / 180)));

            return new SKPoint(x,y);
        }

        private int _borderWidth = 2;
    }
}