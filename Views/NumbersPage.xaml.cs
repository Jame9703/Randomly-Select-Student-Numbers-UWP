using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NumbersPage : Page
    {
        public ObservableCollection<int> RandomNumbersList { get; set; } = new ObservableCollection<int>();
        bool isGridViewUpdated;
        bool isListUpdated;
        bool isTextBoxUpdated;
        public NumbersPage()
        {
            this.InitializeComponent();
            isGridViewUpdated = false;
            isListUpdated = false;
            isTextBoxUpdated = false;
            segmented.SelectionChanged += segmented_SelectionChanged;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            GC.Collect();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        private void IntModeClearAll()
        {
            IntModeRangeFromNumberBox.Text = "";
            IntModeSelectCountNumberBox.Text = "";
            IntModeResultTextBlock.Text = "";
            IntModeRangeToNumberBox.Text = "";
        }
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (IntModeRangeFromNumberBox.Text == "" | IntModeRangeToNumberBox.Text == "" | IntModeSelectCountNumberBox.Text == "")
            {
                PopupNotice popupNotice = new PopupNotice("请输入完整信息");
                popupNotice.ShowPopup();
            }
            else
            {
                isGridViewUpdated = false;
                isListUpdated = false;
                isTextBoxUpdated = false;
                int Start = int.Parse(IntModeRangeFromNumberBox.Text);
                int End = int.Parse(IntModeRangeToNumberBox.Text);
                if (Start > End)
                {
                    PopupNotice popupNotice = new PopupNotice("起始值不能大于终止值");
                    popupNotice.PopupContent.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Warning;
                    popupNotice.ShowPopup();
                }
                bool success = int.TryParse(IntModeSelectCountNumberBox.Text, out int TakeCount);
                if (success)
                {
                    // 转换成功
                    await Task.Run(() =>
                    {
                        // 生成指定范围内的数字序列
                        List<int> numbers = Enumerable.Range(Start, End - Start + 1).ToList();
                        // 使用随机数生成器对数字序列进行打乱
                        Random random = new Random();
                        numbers = numbers.OrderBy(x => random.Next()).ToList();

                        // 取前几位数字
                        RandomNumbersList = new ObservableCollection<int>(numbers.Take(TakeCount));
                    });
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        //清除旧的列表内容
                        //RandomNumbersListStackPanel.Children.Clear();
                        // 清除旧的网格内容
                        //RandomNumbersGridView.Items.Clear();
                        //按需清除文本内容

                        IntModeResultTextBlock.Text = "";
                        if (segmented.SelectedIndex == 0)
                        {
                            if (isGridViewUpdated == false)
                            {
                                ShowRandomNumbersInGridView();
                                isGridViewUpdated = true;
                            }
                        }
                        else if (segmented.SelectedIndex == 1)
                        {
                            if (isListUpdated == false)
                            {
                                ShowRandomNumbersInList();
                                isListUpdated = true;
                            }
                        }
                        else
                        {
                            if (isTextBoxUpdated == false)
                            {
                                ShowRandomNumbersInTextBox();
                                isTextBoxUpdated = true;
                            }
                        }
                    });

                }
                else
                {
                    // 转换失败，NumberBox中的值不是有效的整数
                    IntModeSelectCountNumberBox.Text = "10";
                }
            }
        }

        private void IntModeResetAll_Click(object sender, RoutedEventArgs e)
        {
            IntModeClearAll();
        }

        private void IntModeClearResult_Click(object sender, RoutedEventArgs e)
        {
            IntModeResultTextBlock.Text = "";
            IntModeRandomNumbersGridView.ItemsSource = null;
            IntModeRandomNumbersListView.ItemsSource = null;
        }

        private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                IntModeResultTextBlock.FontSize = slider.Value;
            }
        }

        private void segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (segmented.SelectedIndex == 0)
            {
                //网格
                IntModeRandomNumbersGridView.Visibility = Visibility.Visible;
                IntModeRandomNumbersListView.Visibility = Visibility.Collapsed;
                IntModeRandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
                if (isGridViewUpdated == false)
                {
                    ShowRandomNumbersInGridView();
                    isGridViewUpdated = true;
                }
            }
            else if (segmented.SelectedIndex == 1)
            {
                //列表
                IntModeRandomNumbersGridView.Visibility = Visibility.Collapsed;
                IntModeRandomNumbersListView.Visibility = Visibility.Visible;
                IntModeRandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
                if (isListUpdated == false)
                {
                    ShowRandomNumbersInList();
                    isListUpdated = true;
                }
            }
            else
            {
                //文本
                IntModeRandomNumbersGridView.Visibility = Visibility.Collapsed;
                IntModeRandomNumbersListView.Visibility = Visibility.Collapsed;
                IntModeRandomNumbersTextBoxGrid.Visibility = Visibility.Visible;
                if (isTextBoxUpdated == false)
                {
                    ShowRandomNumbersInTextBox();
                    isTextBoxUpdated = true;
                }
            }
        }

        private void ShowRandomNumbersInGridView()
        {
            IntModeRandomNumbersGridView.ItemsSource = null;
            IntModeRandomNumbersGridView.ItemsSource = RandomNumbersList;
        }

        private void ShowRandomNumbersInList()
        {
            IntModeRandomNumbersListView.ItemsSource = null;
            IntModeRandomNumbersListView.ItemsSource = RandomNumbersList;
        }

        private void ShowRandomNumbersInTextBox()
        {
            StringBuilder sb = new StringBuilder();
            //文本
            IntModeRandomNumbersGridView.Visibility = Visibility.Collapsed;
            IntModeRandomNumbersListView.Visibility = Visibility.Collapsed;
            IntModeRandomNumbersTextBoxGrid.Visibility = Visibility.Visible;
            switch (IntModeSpanModeComboBox.SelectedIndex)
            {
                case 0:
                    foreach (int result in RandomNumbersList)
                    {
                        sb.Append("--" + result.ToString() + "--");
                    }
                    IntModeResultTextBlock.Text = sb.ToString();
                    break;
                case 1:
                    foreach (int result in RandomNumbersList)
                    {
                        sb.Append("“" + result.ToString() + "”");
                    }
                    IntModeResultTextBlock.Text = sb.ToString();
                    break;
                case 2:
                    foreach (int result in RandomNumbersList)
                    {
                        sb.Append("‘" + result.ToString() + "’");
                    }
                    IntModeResultTextBlock.Text = sb.ToString();
                    break;
                case 3:
                    foreach (int result in RandomNumbersList)
                    {
                        sb.Append(result.ToString() + " ");
                    }
                    IntModeResultTextBlock.Text = sb.ToString();
                    break;
                case 4:
                    foreach (int result in RandomNumbersList)
                    {
                        sb.Append(result.ToString() + "\n");
                    }
                    IntModeResultTextBlock.Text = sb.ToString();
                    break;
            }
        }
    }
}
