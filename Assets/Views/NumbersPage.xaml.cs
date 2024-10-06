using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            GC.Collect();
        }
        private void ClearAll()
        {
            RangeFromNumberBox.Text = "";
            SelectCountNumberBox.Text = "";
            ResultTextBox.Text = "";
            RangeToNumberBox.Text = "";
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (RangeFromNumberBox.Text == "" | RangeToNumberBox.Text == "" | SelectCountNumberBox.Text == "")
            {
                PopupNotice popupNotice = new PopupNotice("请输入完整信息");
                popupNotice.ShowPopup();
            }
            else
            {
                int Start = int.Parse(RangeFromNumberBox.Text);
                int End = int.Parse(RangeToNumberBox.Text);
                int a; bool success = int.TryParse(SelectCountNumberBox.Text, out a);
                if (success)
                {
                    // 转换成功
                    int TakeCount = int.Parse(SelectCountNumberBox.Text);
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
                        ResultTextBox.Text = "";
                    }
                    if (segmented.SelectedIndex == 0)
                    {
                        foreach (int result in Result)
                        {
                            //网格
                            RandomNumbersGridView.Visibility = Visibility.Visible;
                            RandomNumbersListStackPanel.Visibility = Visibility.Collapsed;
                            RandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
                            Border border2 = new Border()
                            {
                                CornerRadius = new CornerRadius(8, 8, 8, 8),
                                BorderBrush = new SolidColorBrush(Colors.LightBlue),
                                BorderThickness = new Thickness(4),
                                Margin = new Thickness(5),
                                Height = 60,
                                Child = new TextBlock
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
                    else if (segmented.SelectedIndex == 1)
                    {
                        foreach (int result in Result)
                        {
                            //列表
                            RandomNumbersGridView.Visibility = Visibility.Collapsed;
                            RandomNumbersListStackPanel.Visibility = Visibility.Visible;
                            RandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
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
                        }
                    }
                    else
                    {
                        foreach (int result in Result)
                        {
                            //文本
                            RandomNumbersGridView.Visibility = Visibility.Collapsed;
                            RandomNumbersListStackPanel.Visibility = Visibility.Collapsed;
                            RandomNumbersTextBoxGrid.Visibility = Visibility.Visible;
                            if (ComboBox2.SelectedItem.ToString() == "-- --")
                            {
                                ResultTextBox.Text += "--" + result.ToString() + "--";
                            }
                            else if (ComboBox2.SelectedItem.ToString() == "“ ”")
                            {
                                ResultTextBox.Text += "“" + result.ToString() + "”";
                            }
                            else if (ComboBox2.SelectedItem.ToString() == "‘ ’")
                            {
                                ResultTextBox.Text += "‘" + result.ToString() + "’";
                            }
                        }
                    }
                }
                else
                {
                    // 转换失败，NumberBox中的值不是有效的整数
                    SelectCountNumberBox.Text = "10";
                }
            }
        }

        private void ResetAll_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }



        private void ClearResult_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = "";
            RandomNumbersGridView.Items.Clear();
            RandomNumbersListStackPanel.Children.Clear();
        }

        private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                ResultTextBox.FontSize = slider.Value;
            }
        }

        private void segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RangeFromNumberBox.Text == "" | RangeToNumberBox.Text == "" | SelectCountNumberBox.Text == "")
            {
                PopupNotice popupNotice = new PopupNotice("请输入完整信息");
                popupNotice.ShowPopup();
            }
            else
            {
                int Start = int.Parse(RangeFromNumberBox.Text);
                int End = int.Parse(RangeToNumberBox.Text);
                int a; bool success = int.TryParse(SelectCountNumberBox.Text, out a);
                if (success)
                {
                    // 转换成功
                    int TakeCount = int.Parse(SelectCountNumberBox.Text);
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
                        ResultTextBox.Text = "";
                    }
                    if (segmented.SelectedIndex == 0)
                    {
                        foreach (int result in Result)
                        {
                            //网格
                            RandomNumbersGridView.Visibility = Visibility.Visible;
                            RandomNumbersListStackPanel.Visibility = Visibility.Collapsed;
                            RandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
                            Border border2 = new Border()
                            {
                                CornerRadius = new CornerRadius(8, 8, 8, 8),
                                BorderBrush = new SolidColorBrush(Colors.LightBlue),
                                BorderThickness = new Thickness(4),
                                Margin = new Thickness(5),
                                Height = 60,
                                Child = new TextBlock
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
                    else if (segmented.SelectedIndex == 1)
                    {
                        foreach (int result in Result)
                        {
                            //列表
                            RandomNumbersGridView.Visibility = Visibility.Collapsed;
                            RandomNumbersListStackPanel.Visibility = Visibility.Visible;
                            RandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
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
                        }
                    }
                    else
                    {
                        foreach (int result in Result)
                        {
                            //文本
                            RandomNumbersGridView.Visibility = Visibility.Collapsed;
                            RandomNumbersListStackPanel.Visibility = Visibility.Collapsed;
                            RandomNumbersTextBoxGrid.Visibility = Visibility.Visible;
                            if (ComboBox2.SelectedItem.ToString() == "-- --")
                            {
                                ResultTextBox.Text += "--" + result.ToString() + "--";
                            }
                            else if (ComboBox2.SelectedItem.ToString() == "“ ”")
                            {
                                ResultTextBox.Text += "“" + result.ToString() + "”";
                            }
                            else if (ComboBox2.SelectedItem.ToString() == "‘ ’")
                            {
                                ResultTextBox.Text += "‘" + result.ToString() + "’";
                            }
                        }
                    }
                }
                else
                {
                    // 转换失败，NumberBox中的值不是有效的整数
                    SelectCountNumberBox.Text = "10";
                }
            }
        }
    }
}
