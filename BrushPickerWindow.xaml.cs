using Microsoft.Win32;
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
            nowBrushRect.Fill = _nowBrush;
        }

        /// <summary>
        /// 現在カラーバーが指しているH
        /// </summary>
        uint nowH = 0;

        public BrushPickerWindow()
        {
            InitializeComponent();
            SetNowBrush(new SolidColorBrush(Colors.Red));
            InitColorBar();
            InitColorCells();
            UpdateColorCells();
            InitImageBrushComboBox();
        }

        public BrushPickerWindow(Color defaultColor)
        {
            InitializeComponent();
            SetNowBrush(new SolidColorBrush(defaultColor));
            nowH = new Hsv(defaultColor).H;
            InitColorBar();
            InitColorCells();
            UpdateColorCells();
            InitImageBrushComboBox();
        }

        public BrushPickerWindow(Brush defaultBrush)
        {
            InitializeComponent();
            SetNowBrush(defaultBrush);
            InitColorBar();
            InitColorCells();
            UpdateColorCells();
            InitImageBrushComboBox();
        }

        /// <summary>
        /// カラーバー（グラデーションになっていて、Hを選べるバー）を作成
        /// </summary>
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

        private List<List<Rectangle>> ColorCells = new List<List<Rectangle>>();

        /// <summary>
        /// 縦横何セルずつ表示するか
        /// </summary>
        private static int cellSplit = 12;
        /// <summary>
        /// カラーセル（カラーバーの下のSVを既定個でわったもの）の初期化
        /// </summary>
        private void InitColorCells()
        {
            double chipSize = colorCellsCanvas.Width / cellSplit * 1.0;
            for (int y = 0; y < cellSplit; y++)
            {
                var list = new List<Rectangle>();
                for (int x = 0; x < cellSplit; x++)
                {
                    var rect = new Rectangle() { Width = chipSize, Height = chipSize };
                    Canvas.SetLeft(rect, x * chipSize); Canvas.SetTop(rect, y * chipSize);
                    var s = (byte)Math.Round(x * 255.0 / (cellSplit - 1));
                    var v = (byte)Math.Round(255 - y * 255.0 / (cellSplit - 1));
                    rect.Fill = new SolidColorBrush(GetColorFromHsv(nowH, s, v));
                    colorCellsCanvas.Children.Add(rect);
                    list.Add(rect);
                }
                ColorCells.Add(list);
            }
        }
        /// <summary>
        /// カラーセルの更新
        /// </summary>
        private void UpdateColorCells()
        {
            for (int y = 0; y < cellSplit; y++)
            {
                for (int x = 0; x < cellSplit; x++)
                {
                    var rect = ColorCells[y][x];
                    byte s = (byte)Math.Round(x * 255.0 / (cellSplit - 1));
                    byte v = (byte)Math.Round(255 - y * 255.0 / (cellSplit - 1));
                    rect.Fill = new SolidColorBrush(GetColorFromHsv(nowH, s, v));
                }
            }
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

        /// <summary>
        /// カラーバーを動かした時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorBarSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            uint h = (uint)Math.Round(e.NewValue % 360);
            nowH = h;
            UpdateColorCells();
        }

        /// <summary>
        /// カラーセルをクリックした時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorCellsCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(colorCellsCanvas);
            int ChipSize = (int)Math.Round(colorCellsCanvas.Width / cellSplit * 1.0);
            int x = (int)Math.Round(p.X) / ChipSize;
            int y = (int)Math.Round(p.Y) / ChipSize;
            byte s = (byte)Math.Round(x * 255.0 / (cellSplit - 1));
            byte v = (byte)Math.Round(255 - y * 255.0 / (cellSplit - 1));
            SetNowBrush(new SolidColorBrush(GetColorFromHsv(new Hsv(nowH, s, v))));
        }

        //-----------------------------------------------------------------------------------------------------
        //
        // Image Brush
        //
        //-----------------------------------------------------------------------------------------------------

        private string nowImageFilePath = "";
        private Stretch nowStretch = Stretch.Uniform;
        private List<string> stretchDescriptions = new string[] { "縦横比を維持して画面に収める", "縦横比を維持して画面を埋める", "画面一杯に広げる", "本のサイズを維持する" }.ToList();
        private List<Stretch> stretchs = new Stretch[] { Stretch.Uniform, Stretch.UniformToFill, Stretch.Fill, Stretch.None }.ToList();

        private void InitImageBrushComboBox()
        {
            UniformComboBox.ItemsSource = stretchDescriptions;
            UniformComboBox.SelectedIndex = 0;
            TileComboBox.ItemsSource = tileModeDescriptions;
            TileComboBox.SelectedIndex = 0;
            TileWidthComboBox.ItemsSource = tileSizes.Select(a => $"{a * 100}%");
            TileWidthComboBox.SelectedIndex = 0;
            TileHeightComboBox.ItemsSource = tileSizes.Select(a => $"{a * 100}%");
            TileHeightComboBox.SelectedIndex = 0;
        }

        private void UpdateNowImageBrush()
        {
            if (!System.IO.File.Exists(nowImageFilePath)) return;
            var image = new BitmapImage(new Uri(nowImageFilePath));
            SetNowBrush(new ImageBrush(image) { Stretch = nowStretch, TileMode = nowTileMode, Viewport = nowTileViewport });
        }

        /// <summary>
        /// 画像を選択ボタンをクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageSelectButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "画像|*.jpg;*.jpeg;*.png;*.gif;*.bmp|すべてのファイル|*.*";
            dialog.Title = "画像を選択";
            if (dialog.ShowDialog() == true)
            {
                nowImageFilePath = dialog.FileName;
                UpdateNowImageBrush();
            }
        }

        private void UniformComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UniformComboBox.SelectedIndex < 0) return;
            nowStretch = stretchs[UniformComboBox.SelectedIndex];
            UpdateNowImageBrush();
        }

        private List<string> tileModeDescriptions = new string[] { "敷き詰めない", "そのまま敷き詰め", "左右反転しながら敷き詰め", "上下反転しながら敷き詰め", "上下左右反転しながら敷き詰め", }.ToList();
        private List<TileMode> tileModes = new TileMode[] { TileMode.None, TileMode.Tile, TileMode.FlipX, TileMode.FlipY, TileMode.FlipXY }.ToList();
        private TileMode nowTileMode = TileMode.None;
        private Rect nowTileViewport = new Rect(0, 0, 0.1, 0.1);
        private List<double> tileSizes = new double[] { 0.1, 0.2, 0.25, 0.33, 0.5, 1.0 }.ToList();

        /// <summary>
        /// タイルモード
        /// </summary>
        private void TileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TileComboBox.SelectedIndex < 0) return;
            nowTileMode = tileModes[TileComboBox.SelectedIndex];
            if (TileComboBox.SelectedIndex == 0)
                TileSizePanel.IsEnabled = false;
            else
                TileSizePanel.IsEnabled = true;
            UpdateNowImageBrush();
        }

        /// <summary>
        /// タイルの縦のサイズ
        /// </summary>
        private void TileHeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TileHeightComboBox.SelectedIndex < 0) return;
            double h = tileSizes[TileHeightComboBox.SelectedIndex];
            nowTileViewport.Height = h;
            UpdateNowImageBrush();
        }
        /// <summary>
        /// タイルの横のサイズ
        /// </summary>
        private void TileWidthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TileHeightComboBox.SelectedIndex < 0) return;
            double w = tileSizes[TileHeightComboBox.SelectedIndex];
            nowTileViewport.Width = w;
            UpdateNowImageBrush();
        }

        //===============================================================================================
        //
        // Opacity  OK  Cancel
        //
        //===============================================================================================

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (NowBrush == null) return;
            double value = Math.Round(opacitySlider.Value, 2);
            var newBrush = NowBrush.Clone();
            newBrush.Opacity = value;
            SetNowBrush(newBrush);
            opacityLabel.Text = $"透明度:{value * 100}%";
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

        public Hsv(Color color)
        {
            int h, s, v;
            byte max, min = new byte[] { color.R, color.G, color.B }.Min();
            if (color.R == color.G && color.G == color.B)
            {
                max = color.R; h = 0;
            }
            else if (color.R >= color.G && color.R >= color.B)
            {
                max = color.R; h = 60 * ((color.G - color.B) / (max - min));
            }
            else if (color.G >= color.R && color.G >= color.B)
            {
                max = color.G; h = 60 * ((color.B - color.R) / (max - min)) + 120;
            }
            else
            {
                max = color.B; h = 60 * ((color.R - color.G) / (max - min)) + 240;
            }
            while (h < 0) h += 360;
            s = max == 0 ? 0 : (max - min) / max * 255;
            v = max;
            H = (uint)h;
            S = (byte)s;
            V = (byte)v;
        }
    }
}
