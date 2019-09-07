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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Threading;

namespace MCFloatColor
{


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ///// <summary>
        ///// 更新状态栏的文字，多线程时被委托调用
        ///// </summary>
        ///// <param name="text"></param>
        //private void UpdateColorbox(string text)
        //{
        //    ColorBox1.Text = text;
        //}
        //private void UpdateStatusWordInThread(string text)
        //{
        //    UpdateColorbox d = new UpdateColorbox(UpdateColorbox);
        //    this.Dispatcher.Invoke(d, text);
        //}

        public MainWindow()
        {
            InitializeComponent();
        }
        public System.Drawing.Color SavedColor;
        public decimal AlphaValue;
        public int mode_rgba = 0;
        public int mode_rgb = 0;

        private void ColorChoose_Click(object sender, RoutedEventArgs e)
        {
            //创建对象
            ColorDialog colorDialog = new ColorDialog();
            //允许使用该对话框的自定义颜色  
            colorDialog.AllowFullOpen = true;
            colorDialog.FullOpen = true;
            //colorDialog.ShowHelp = true;
            //初始化当前文本框中的字体颜色，  
            //colorDialog.Color = System.Drawing.Color.White;
            colorDialog.Color = SavedColor;
            //当用户在ColorDialog对话框中点击"确定"按钮  
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                #region 数据转换+颜色显示
                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color);
                SolidColorBrush HinglightColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));
                ColorChoose.Background = HinglightColor;
                EnterColor.Background = HinglightColor;
                SavedColor = colorDialog.Color;

                byte[] RGB = { sb.Color.R, sb.Color.G, sb.Color.B };
                byte[] IRGB = new byte[RGB.Length];
                for (int i = 0; i < RGB.Length; i++)
                {
                    IRGB[i] = (Byte)(255 - RGB[i]);
                }
                SolidColorBrush InvertedColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, IRGB[0], IRGB[1], IRGB[2]));
                ColorChoose.Foreground = InvertedColor;
                EnterColor.Foreground = InvertedColor;
                //获取颜色，进行设置
                #endregion
                #region 颜色转换-String
                string[] CRGB = new string[4];
                for (int i = 0; i < RGB.Length; i++)
                {
                    CRGB[i] = Convert.ToString(Math.Round(RGB[i] * Convert.ToDecimal(MaxFloatValue.Text) / 255, Convert.ToInt32(ReservedDigits.Text)));
                }
                CRGB[3] = Convert.ToString(Math.Round(AlphaValue / 100, Convert.ToInt32(ReservedDigits.Text)));
                for (int i = 0; i < CRGB.Length; i++)
                {
                    if (CRGB[i].Length == 1) { CRGB[i] = CRGB[i] + ".0"; }
                }
                ColorBox1.Text = "(" + CRGB[0] + "," + CRGB[1] + "," + CRGB[2] + "," + CRGB[3] + ")";
                ColorBox2.Text = "(" + CRGB[0] + "," + CRGB[1] + "," + CRGB[2] + ")";
                #endregion
            }
        }

        private void Copy1_Click(object sender, RoutedEventArgs e)
        {
            ColorBox1.Focus();
            ColorBox1.SelectionStart = 0;  //设置起始位置 
            ColorBox1.SelectionLength = 99;  //设置长度
            System.Windows.Clipboard.SetDataObject(ColorBox1.SelectedText);
        }

        private void Copy2_Click(object sender, RoutedEventArgs e)
        {
            ColorBox2.Focus();
            ColorBox2.SelectionStart = 0;  //设置起始位置 
            ColorBox2.SelectionLength = 99;  //设置长度
            System.Windows.Clipboard.SetDataObject(ColorBox2.SelectedText);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush scb = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            ColorChoose.Background = scb;
            EnterColor.Background = scb;
            SavedColor = System.Drawing.Color.Red;
        }


        private bool EnterColorState;
        private void EnterColor_Click(object sender, RoutedEventArgs e)
        {
            if (EnterColorState == true)
            {
                EnterFloatColorBox.Visibility = Visibility.Hidden;
                EnterColor.Margin = new Thickness(165, 10, 0, 0);
                EnterColor.Width = 150;
                EnterColor.Height = 50;
                EnterColor.Content = "输入浮点颜色";
                ColorChoose.Visibility = Visibility.Visible;
                EnterColorState = false;
                string ARGB = EnterFloatColorBox.Text;
                if (ARGB != "")
                {
                    ARGB = ARGB.Replace("(", "");
                    ARGB = ARGB.Replace(")", "");
                    string[] StrRgb = ARGB.Split(',');
                    decimal[] DecRgb = new decimal[StrRgb.Length];
                    for (int i = 0; i < StrRgb.Length; i++)
                    {
                        DecRgb[i] = Math.Round(Convert.ToDecimal(StrRgb[i]) * 255 / Convert.ToDecimal(MaxFloatValue.Text));
                    }
                    SavedColor = System.Drawing.Color.FromArgb(255, Convert.ToByte(DecRgb[0]), Convert.ToByte(DecRgb[1]), Convert.ToByte(DecRgb[2]));
                    if (StrRgb.Length == 4)
                    {
                        AlphaValue = Math.Round(DecRgb[3] * 100 / 255, Convert.ToInt32(ReservedDigits.Text));
                        Percent.Text = Convert.ToString(AlphaValue);
                    }
                    #region 数据转换+颜色显示
                    System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(SavedColor);
                    SolidColorBrush HinglightColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));
                    ColorChoose.Background = HinglightColor;
                    EnterColor.Background = HinglightColor;
                    byte[] RGB = { sb.Color.R, sb.Color.G, sb.Color.B };
                    byte[] IRGB = new byte[RGB.Length];
                    for (int i = 0; i < RGB.Length; i++)
                    {
                        IRGB[i] = (Byte)(255 - RGB[i]);
                    }
                    SolidColorBrush InvertedColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, IRGB[0], IRGB[1], IRGB[2]));
                    ColorChoose.Foreground = InvertedColor;
                    EnterColor.Foreground = InvertedColor;
                    //获取颜色，进行设置
                    #endregion
                    #region 颜色转换-String
                    string[] CRGB = new string[4];
                    for (int i = 0; i < RGB.Length; i++)
                    {
                        CRGB[i] = Convert.ToString(Math.Round(RGB[i] * Convert.ToDecimal(MaxFloatValue.Text) / 255, Convert.ToInt32(ReservedDigits.Text)));
                    }
                    CRGB[3] = Convert.ToString(Math.Round(AlphaValue / 100, Convert.ToInt32(ReservedDigits.Text)));
                    for (int i = 0; i < CRGB.Length; i++)
                    {
                        if (CRGB[i].Length == 1) { CRGB[i] = CRGB[i] + ".0"; }
                    }
                    ColorBox1.Text = "(" + CRGB[0] + "," + CRGB[1] + "," + CRGB[2] + "," + CRGB[3] + ")";
                    ColorBox2.Text = "(" + CRGB[0] + "," + CRGB[1] + "," + CRGB[2] + ")";
                    #endregion
                }
            }
            else
            {
                EnterFloatColorBox.Visibility = Visibility.Visible;
                EnterColor.Margin = new Thickness(215, 10, 0, 0);
                EnterColor.Width = 100;
                EnterColor.Height = 50;
                EnterColor.Content = "确认";
                ColorChoose.Visibility = Visibility.Hidden;
                EnterColorState = true;
                EnterFloatColorBox.Focus();
                EnterFloatColorBox.Select(0, 99);
            }
        }
        private void Percent_TextChanged(object sender, TextChangedEventArgs e)
        {
            AlphaValue = Convert.ToDecimal(Percent.Text);
            Percent.Text = Convert.ToString(Math.Round(AlphaValue, 0));
            #region 数据转换+颜色显示
            System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(SavedColor);
            byte[] RGB = { sb.Color.R, sb.Color.G, sb.Color.B };
            //获取颜色，进行设置
            #endregion
            #region 颜色转换-String
            string[] CRGB = new string[4];
            for (int i = 0; i < RGB.Length; i++)
            {
                CRGB[i] = Convert.ToString(Math.Round(RGB[i] * Convert.ToDecimal(MaxFloatValue.Text) / 255, Convert.ToInt32(ReservedDigits.Text)));
            }
            CRGB[3] = Convert.ToString(Math.Round(AlphaValue / 100, Convert.ToInt32(ReservedDigits.Text)));
            for (int i = 0; i < CRGB.Length; i++)
            {
                if (CRGB[i].Length == 1) { CRGB[i] = CRGB[i] + ".0"; }
            }
            ColorBox1.Text = CRGB[0] + "," + CRGB[1] + "," + CRGB[2] + "," + CRGB[3];
            ColorBox2.Text = CRGB[0] + "," + CRGB[1] + "," + CRGB[2];
            #endregion
        }
        public static void ColorConvertThread()
        {

        }
        public static void ChangeModeThread(string text)
        {

    //        updateStatusStripDelegate d = new updateStatusStripDelegate(updateStatusWordInDelegate)；
    //this.Dispatcher.Invoke(d, text);


        }


        private void Rgba_show_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mode_rgba == 0)
            {
                mode_rgba = 1;
                rgba_show.Content = "(R,G,B,A)=";
            }
            else if (mode_rgba == 1)
            {
                mode_rgba = 2;
                rgba_show.Content = "[R,G,B,A]=";
            }
            else
            {
                mode_rgba = 0;
                rgba_show.Content = " R,G,B,A =";
            }
            //ThreadStart ChangeModeRef = new ThreadStart(ChangeModeThread);
            //Thread childThread = new Thread(ChangeModeRef);
            //childThread.Start();
        }

        private void Rgb_show_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mode_rgb == 0)
            {
                mode_rgb = 1;
                rgb_show.Content = "(R,G,B)=";

            }
            else if (mode_rgb == 1)
            {
                mode_rgb = 2;
                rgb_show.Content = "[R,G,B]=";
            }
            else
            {
                mode_rgb = 0;
                rgb_show.Content = " R,G,B =";
            }
        }
    }
}
