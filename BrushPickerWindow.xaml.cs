using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BrushPicker
{
    /// <summary>
    /// BrushPickerWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BrushPickerWindow : Window
    {
        public Brush NowBrush { get { return _nowBrush; } }
        private Brush _nowBrush;
        public void SetNowBrush(Brush b)
        {
            _nowBrush = b;
        }

        public BrushPickerWindow()
        {
            InitializeComponent();
            InitColorBar();
        }

        private void InitColorBar()
        {
            // グラデーションを何色で表現するか
            int colorBarSplit = 16;
            var barBrush = new LinearGradientBrush();
            barBrush.StartPoint = new Point(0, 0.5);
            barBrush.EndPoint = new Point(1, 0.5);
            for (int i = 0; i < colorBarSplit; i++)
            {
                double length = 1.0 / colorBarSplit * i;
                uint h = (uint)Math.Round(360.0 / colorBarSplit * i);
                barBrush.GradientStops.Add(new GradientStop(GetColorFromHsv(h), length));
            }
            colorBarRect.Fill = barBrush;
        }

        /// <summary>
        /// HSVからRGBカラーを取得
        /// </summary>
        /// <param name="hsv"></param>
        /// <returns></returns>
        private static Color GetColorFromHsv(Hsv hsv)
        {
            return GetColorFromHsv(hsv.H, hsv.S, hsv.V);
        }

        /// <summary>
        /// HSVからRGBカラーを取得
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private static Color GetColorFromHsv(uint h, byte s = 255, byte v = 255)
        {
            double r, g, b;
            double max = v, min = max - ((s / 255.0) * max);
            if (h >= 0 && h < 60)
            {
                r = max;
                g = (h / 60.0) * (max - min) + min;
                b = min;
            }
            else if (h >= 60 && h < 120)
            {
                r = ((120 - h) / 60.0) * (max - min) + min;
                g = max; b = min;
            }
            else if (h >= 120 && h < 180)
            {
                r = min; g = max;
                b = ((h - 120) / 60.0) * (max - min) + min;
            }
            else if (h >= 180 && h < 240)
            {
                r = min;
                g = ((240 - h) / 60.0) * (max - min) + min;
                b = max;
            }
            else if (h >= 240 && h < 300)
            {
                r = ((h - 240) / 60.0) * (max - min) + min;
                g = min; b = max;
            }
            else if (h >= 300 && h <= 360)
            {
                r = max; g = min;
                b = ((360 - h) / 60.0) * (max - min) + min;
            }
            else
            {
                r = g = b = 0;
            }
            byte ir = (byte)Math.Round(r), ig = (byte)Math.Round(g), ib = (byte)Math.Round(b);
            return Color.FromArgb(255, ir, ig, ib);
        }

    }

    public class Hsv
    {
        public uint H { get; private set; }
        public byte S { get; private set; }
        public byte V { get; private set; }

        public Hsv(uint h, byte s, byte v)
        {
            H = h % 360;
            S = s;
            V = v;
        }
    }
}
