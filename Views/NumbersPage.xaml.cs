using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
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
        List <int> RandomNumbersList = new List<int>();
        bool isGridViewUpdated;
        bool isListUpdated;
        bool isTextBoxUpdated;
        public NumbersPage()
        {
            this.InitializeComponent();
            isGridViewUpdated = false;
            isListUpdated = false;
            isTextBoxUpdated = false;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(ContentGrid).Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0f, 1f);
            animation.InsertKeyFrame(1f, 0f);
            animation.Duration = TimeSpan.FromSeconds(1);
            var visual = ElementCompositionPreview.GetElementVisual(ContentGrid);
            visual.StartAnimation("Opacity", animation);
            GC.Collect();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var compositor = ElementCompositionPreview.GetElementVisual(ContentGrid).Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0f, 0f);
            animation.InsertKeyFrame(1f, 1f);
            animation.Duration = TimeSpan.FromSeconds(1);
            var visual = ElementCompositionPreview.GetElementVisual(ContentGrid);
            visual.StartAnimation("Opacity", animation);
        }
        private void ClearAll()
        {
            RangeFromNumberBox.Text = "";
            SelectCountNumberBox.Text = "";
            ResultTextBox.Text = "";
            RangeToNumberBox.Text = "";
        }
        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (RangeFromNumberBox.Text == "" | RangeToNumberBox.Text == "" | SelectCountNumberBox.Text == "")
            {
                PopupNotice popupNotice = new PopupNotice("请输入完整信息");
                popupNotice.ShowPopup();
            }
            else
            {
                isGridViewUpdated = false;
                isListUpdated = false;
                isTextBoxUpdated = false;
                int Start = int.Parse(RangeFromNumberBox.Text);
                int End = int.Parse(RangeToNumberBox.Text);
                if (Start > End)
                {
                    PopupNotice popupNotice = new PopupNotice("起始值不能大于终止值");
                    popupNotice.PopupContent.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Warning;
                    popupNotice.ShowPopup();
                }
                bool success = int.TryParse(SelectCountNumberBox.Text, out int TakeCount);
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
                        RandomNumbersList = numbers.Take(TakeCount).ToList();
                    });
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        //清除旧的列表内容
                        //RandomNumbersListStackPanel.Children.Clear();
                        // 清除旧的网格内容
                        //RandomNumbersGridView.Items.Clear();
                        //按需清除文本内容
                        if (CheckBox.IsChecked != true)
                        {
                            ResultTextBox.Text = "";
                        }
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
            RandomNumbersGridView.ItemsSource = null;
            RandomNumbersListView.ItemsSource = null;
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
            if (segmented.SelectedIndex == 0)
            {
                //网格
                RandomNumbersGridView.Visibility = Visibility.Visible;
                RandomNumbersListView.Visibility = Visibility.Collapsed;
                RandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
                if (isGridViewUpdated == false)
                {
                    ShowRandomNumbersInGridView();
                    isGridViewUpdated = true;
                }
            }
            else if (segmented.SelectedIndex == 1)
            {
                //列表
                RandomNumbersGridView.Visibility = Visibility.Collapsed;
                RandomNumbersListView.Visibility = Visibility.Visible;
                RandomNumbersTextBoxGrid.Visibility = Visibility.Collapsed;
                if (isListUpdated == false)
                {
                    ShowRandomNumbersInList();
                    isListUpdated = true;
                }
            }
            else
            {
                //文本
                RandomNumbersGridView.Visibility = Visibility.Collapsed;
                RandomNumbersListView.Visibility = Visibility.Collapsed;
                RandomNumbersTextBoxGrid.Visibility = Visibility.Visible;
                if (isTextBoxUpdated == false)
                {
                    ShowRandomNumbersInTextBox();
                    isTextBoxUpdated = true;
                }
            }
        }

        private void ShowRandomNumbersInGridView()
        {
            RandomNumbersGridView.ItemsSource = null;
            RandomNumbersGridView.ItemsSource = RandomNumbersList;
        }

        private void ShowRandomNumbersInList()
        {
            RandomNumbersListView.ItemsSource = null;
            RandomNumbersListView.ItemsSource = RandomNumbersList;
        }

        private void ShowRandomNumbersInTextBox()
        {
            foreach (int result in RandomNumbersList)
            {
                //文本
                RandomNumbersGridView.Visibility = Visibility.Collapsed;
                RandomNumbersListView.Visibility = Visibility.Collapsed;
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
}
