using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NumbersPage : Page
    {

        public NumbersPage()
        {
            this.InitializeComponent();
        }
        public void c()
        {
            txtFrom.Text = "";
            txtQuan.Text = "";
            txtRe.Text = "";
            txtTo.Text = "";
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (txtFrom.Text == "" | txtTo.Text == "" | txtQuan.Text == "")
            {
                PopupNotice popupNotice = new PopupNotice("请输入完整信息");
                popupNotice.ShowAPopup();
            }
            else
            {
               int Start = int.Parse(txtFrom.Text);
               int End = int.Parse(txtTo.Text);
                int a; bool success = int.TryParse(txtQuan.Text, out a);
                if (success)
                {
                    // 转换成功
                    int TakeCount = int.Parse(txtQuan.Text);
                    // 生成指定范围内的数字序列
                    List<int> numbers = Enumerable.Range(Start, End - Start + 1).ToList();

                    // 使用随机数生成器对数字序列进行打乱
                    Random random = new Random();
                    numbers = numbers.OrderBy(x => random.Next()).ToList();

                    // 取前几位数字
                    List<int> Result = numbers.Take(TakeCount).ToList();
                    //清除旧的列表内容
                    RandomNumbersListStackPanel.Children.Clear();
                    // 清除旧的网格内容
                    RandomNumbersGridView.Items.Clear();
                    //按需清除文本内容
                    if (CheckBox.IsChecked != true)
                    {
                        txtRe.Text = "";
                    }
                    foreach (int result in Result)
                    {
                        //列表
                        Border border1 = new Border()
                        {
                            CornerRadius = new CornerRadius(8, 8, 8, 8),
                            BorderBrush = new SolidColorBrush(Colors.LightBlue),
                            BorderThickness = new Thickness(4),
                            Margin = new Thickness(5),
                            Height = 60,
                            Child = new TextBlock
                            {
                                FontSize = 33,
                                FontFamily = new FontFamily("{StaticResource HarmonyOSSans}"),
                                Width = 150,
                                Height = 50,
                                Margin = new Thickness(5),
                                TextAlignment = TextAlignment.Center,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Text = result.ToString(),
                            }
                        };
                        RandomNumbersListStackPanel.Children.Add(border1);
                        //文本
                        if (ComboBox2.SelectedItem.ToString() == "-- --")
                        {
                            txtRe.Text += "--" + result.ToString() + "--";
                        }
                        else if (ComboBox2.SelectedItem.ToString() == "“ ”")
                        {
                            txtRe.Text += "“" + result.ToString() + "”";
                        }
                        else if (ComboBox2.SelectedItem.ToString() == "‘ ’")
                        {
                            txtRe.Text += "‘" + result.ToString() + "’";
                        }
                        //网格
                                Border border2 = new Border()
                                {
                                    CornerRadius = new CornerRadius(8, 8, 8, 8),
                                    BorderBrush = new SolidColorBrush(Colors.LightBlue),
                                    BorderThickness = new Thickness(4),
                                    Margin = new Thickness(5),
                                    Height = 60,
                                    Child  = new TextBlock 
                                    {
                                        Name = "textBlock",
                                        FontSize = 33,
                                        FontFamily = new FontFamily("{StaticResource HarmonyOSSans}"),
                                        Width = 150,
                                        Height = 50,
                                        Margin = new Thickness(5),
                                        TextAlignment = TextAlignment.Center,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Text = result.ToString(),
                                     }
                                };
                        RandomNumbersGridView.Items.Add(border2);
                    }
                }
                else
                {
                    // 转换失败，NumberBox中的值不是有效的整数
                    txtQuan.Text = "10";
                }

                /* Guid temp = Guid.NewGuid();
                int guidseed = BitConverter.ToInt32(temp.ToByteArray(), 1);
                Random r = new Random(guidseed);
                HashSet<int> Set = new HashSet<int>();
                re: for (int i = 1; i <= n; i++)
                {int random = r.Next(x, y + 1);Set.Add(random);}
                if (n != Set.Count)
                {Set.Clear();goto re;}    */
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            c();
        }



        private void 清空结果_Click(object sender, RoutedEventArgs e)
        {
            txtRe.Text = "";
            RandomNumbersGridView.Items.Clear();
            RandomNumbersListStackPanel.Children.Clear();
        }

        private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                txtRe.FontSize = slider.Value;
            }
        }
    }
}
