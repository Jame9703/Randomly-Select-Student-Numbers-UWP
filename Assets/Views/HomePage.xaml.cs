//本页使用的名称及变量:
//Selecting:当选中“不许偷看”后显示的Grid,装载了一个ProgressRing和一个Text为“抽取中”的TextBlock
//isRandomizing:声明的bool类型，用于判断是否已经开始抽取
//StackPanelCheckBoxes:用于装载CheckBox集合（用于选择抽取对象）
//StartorStopButton:控制抽取的开始和停止
//ResultTextBox:最终输出结果的TextBox

using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using muxc = Microsoft.UI.Xaml.Controls;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    public sealed partial class HomePage : Page
    {
        ClassPage classpage = new ClassPage();
        public DispatcherTimer timer;
        private bool isRandomizing = false;
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static readonly string ClassNameKey = "ClassName";
        public HomePage()
        {
            this.InitializeComponent();
            PopupNotice popupNotice = new PopupNotice("按“开始”键以开始抽取");
            popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
            popupNotice.ShowAPopup();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            Text2.Text = System.DateTime.Now.ToString("M");
            // 初始化行号
            UpdateLineNumbers();
            segmented.SelectedIndex = 0;//在xaml中设置会导致控件未加载就被调用//System.NullReferenceException:“Object reference not set to an instance of an object.”
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCheckBoxes();
            string ClassName = localSettings.Values[ClassNameKey] as string;
        }
        private void CreateCheckBoxes()
        {
            StackPanelCheckBoxes.Children.Clear();//将曾经的CheckBox删去（如果有）
            var lines = classpage.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            StackPanelCheckBoxes.Orientation = Orientation.Vertical; //设置为竖直布局
            //创建CheckBox并在“StackPanelCheckBoxes”中显示
            for (int i = 0; i < lines.Length; i++)//此次i指Index
            {
                if (lines[i] != String.Empty)//手动删除空项
                {
                    var checkBox = new CheckBox();
                    checkBox.Click += checkBox_Click;
                    string line = lines[i];
                    checkBox.Content = i+1 + "." + line;//遍历Editor中的每一行，用来生成CheckBox集合
                    checkBox.Margin = new Thickness(0); //设置边距以避免与其他元素重叠(设置了个寂寞)
                    checkBox.IsChecked = true;
                    StackPanelCheckBoxes.Children.Add(checkBox);
                    checkBox.FontFamily = (FontFamily)Application.Current.Resources["HarmonyOSSans"];
                    checkBox.FontSize = 16;
                }
            }
            var _lines = classpage.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxesCount.Text = "已选择" + _lines.Length + "/" + _lines.Length;
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            List<int> checkedCheckBoxes = new List<int>();
            checkedCheckBoxes.Clear();
            string[] lines = classpage.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)//此处i指Index
                {
                    // 遍历所有CheckBox并检查它们的IsChecked属性
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();
                    if (checkBox[i].IsChecked == true)
                    {
                        checkedCheckBoxes.Add(i);
                    }
                }
                checkedCheckBoxesCount.Text = "已选择" + checkedCheckBoxes.Count.ToString() + "/" + lines.Length;
            }
            else
            {
                checkedCheckBoxesCount.Text = "未选择";
            }
        }
        private void StartorStopButton_Click(object sender, RoutedEventArgs e)
        {
            Storyboard1.Begin();
            Storyboard2.Begin();
            Storyboard3.Begin();
            Storyboard4.Begin();
            if (Numbers.Value == 1)
            {
                int a; bool success = int.TryParse(NumberBox.Text, out a);//注意:a或1不能为整数，否则会执行整数除法，只会保留整数部分
                if (success)
                {
                    // 转换成功,将NumberBox中的值作为抽取间隔
                    double frequency = 1.0/a;
                    timer.Interval = TimeSpan.FromSeconds(frequency);
                }
                else
                {
                    // 转换失败，NumberBox中的值不是有效的整数，自动更改为默认值
                    NumberBox.Text = "10";
                }
                if (classpage.Editor.Text == string.Empty)
                {
                    PopupNotice popupNotice = new PopupNotice("请先填写班级信息");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                    popupNotice.ShowAPopup();
                }
                else
                {
                        ResultTextBox.Visibility = Visibility.Visible;
                        if (!isRandomizing)
                        {
                            //开始抽取
                            isRandomizing = true;
                            timer.Start();
                            StartorStopButton.Label = "停止";
                            StartorStopButton.Icon = new SymbolIcon(Symbol.Pause);
                            StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 236, G = 179, B = 1 });
                        }
                        else if (isRandomizing)
                        {
                            //停止抽取
                            timer.Stop();
                            isRandomizing = false;
                            StartorStopButton.Label = "开始";
                            StartorStopButton.Icon = new SymbolIcon(Symbol.Play);
                            StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
                        }
                    }
            }
            else 
            {
                //停止抽取
                timer.Stop();
                isRandomizing = false;
                StartorStopButton.Label = "开始";
                StartorStopButton.Icon = new SymbolIcon(Symbol.Play);
                StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
                List<int> checkedCheckBoxes = new List<int>();
                checkedCheckBoxes.Clear();
                RandomNumbersGridView.Items.Clear();
                string[] lines = classpage.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        // 遍历所有CheckBox并检查它们的IsChecked属性
                        var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();
                        if (checkBox[i].IsChecked == true)
                        {
                            checkedCheckBoxes.Add(i);
                        }
                    }
                    int a; bool success = int.TryParse(Numbers.Text, out a);
                    if (success)
                    {
                        // 转换成功
                        int TakeCount = int.Parse(Numbers.Text);
                        //将checkedCheckBoxes打乱顺序
                        Random random = new Random();
                        checkedCheckBoxes = checkedCheckBoxes.OrderBy(x => random.Next()).ToList();
                        // 取前几位数字
                        List<int> Result = checkedCheckBoxes.Take(TakeCount).ToList();
                        foreach (int randomIndex in Result)
                        {
                            //if (ModeComboBox.SelectedItem.ToString() == "仅抽取学号")
                            {
                                Border border = new Border()
                                {
                                    CornerRadius = new CornerRadius(8, 8, 8, 8),
                                    BorderBrush = new SolidColorBrush(Colors.LightBlue),
                                    BorderThickness = new Thickness(4),
                                    Margin = new Thickness(5),
                                    Width = 100,
                                    Height = 60,
                                    Child = new TextBlock
                                    {
                                        Name = "textBlock",
                                        FontSize = 33,
                                        FontFamily = new FontFamily("{StaticResource HarmonyOSSans}"),
                                        //Width = 50,
                                        Height = 50,
                                        Margin = new Thickness(5),
                                        TextAlignment = TextAlignment.Center,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Text = (randomIndex + 1).ToString(),
                                    }
                                };
                                RandomNumbersGridView.Items.Add(border);
                            }
                            //else if (ModeComboBox.SelectedItem.ToString() == "仅抽取姓名")
                            {
                                Border border = new Border()
                                {
                                    CornerRadius = new CornerRadius(8, 8, 8, 8),
                                    BorderBrush = new SolidColorBrush(Colors.LightBlue),
                                    BorderThickness = new Thickness(4),
                                    Margin = new Thickness(5),
                                    Width = 150,
                                    Height = 60,
                                    Child = new TextBlock
                                    {
                                        Name = "textBlock",
                                        FontSize = 33,
                                        FontFamily = new FontFamily("{StaticResource HarmonyOSSans}"),
                                        //Width = 50,
                                        Height = 50,
                                        Margin = new Thickness(5),
                                        TextAlignment = TextAlignment.Center,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Text = lines[randomIndex],
                                    }
                                };
                                RandomNumbersGridView.Items.Add(border);
                            }
                            //else if (ModeComboBox.SelectedItem.ToString() == "抽取学号和姓名")
                            {
                                Border border = new Border()
                                {
                                    CornerRadius = new CornerRadius(8, 8, 8, 8),
                                    BorderBrush = new SolidColorBrush(Colors.LightBlue),
                                    BorderThickness = new Thickness(4),
                                    Margin = new Thickness(5),
                                    Width = 195,
                                    Height = 60,
                                    Child = new TextBlock
                                    {
                                        Name = "textBlock",
                                        FontSize = 33,
                                        FontFamily = new FontFamily("{StaticResource HarmonyOSSans}"),
                                        //Width = 50,
                                        Height = 50,
                                        Margin = new Thickness(5),
                                        TextAlignment = TextAlignment.Center,
                                        HorizontalAlignment = HorizontalAlignment.Center,
                                        VerticalAlignment = VerticalAlignment.Center,
                                        Text = (randomIndex + 1) + "." + lines[randomIndex],
                                    }
                                };
                                RandomNumbersGridView.Items.Add(border);
                            }
                        }
                    }
                    else
                    {
                        // 转换失败，NumberBox中的值不是有效的整数
                        Numbers.Text = "10";
                    }
                }
                else
                {
                    PopupNotice popupNotice = new PopupNotice("请先填写班级信息");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                    popupNotice.ShowAPopup();
                }

            }
        }
        private void Timer_Tick(object sender, object e)
        {
            List<int> checkedCheckBoxes = new List<int>();
            checkedCheckBoxes.Clear();
            string[] lines = classpage.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    // 遍历所有CheckBox并检查它们的IsChecked属性
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();
                    if (checkBox[i].IsChecked == true)
                    {
                        checkedCheckBoxes.Add(i);
                        Random random = new Random();
                        int randomIndex = checkedCheckBoxes[random.Next(checkedCheckBoxes.Count)];
                        //if (ModeComboBox.SelectedItem.ToString() == "仅抽取学号")
                        {
                            ResultTextBox.Text = (randomIndex + 1).ToString();
                        }
                        //else if (ModeComboBox.SelectedItem.ToString() == "仅抽取姓名")
                        {
                            ResultTextBox.Text = lines[randomIndex];
                        }
                        //else if (ModeComboBox.SelectedItem.ToString() == "抽取学号和姓名")
                        {
                            ResultTextBox.Text = (randomIndex + 1) + "." + lines[randomIndex];
                        }
                    }
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请先填写班级信息");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowAPopup();
            }
        }

        public void UpdateLineNumbers()
        {
            string text = classpage.Editor.Text;
            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //lineNumberTextBlock.Text = string.Empty;

            for (int i = 0; i < lines.Length; i++)
            {
                //lineNumberTextBlock.Text += $"{i + 1}\n";
            }
        }
        public void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLineNumbers();
            CreateCheckBoxes();
        }
        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (NumberBox.Text == string.Empty)
            {
                NumberBox.Text = "10";
            }
            PopupNotice popupNotice = new PopupNotice("成功将抽取间隔设置为" + NumberBox.Text+"毫秒");
            popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
            popupNotice.ShowAPopup();
        }



        private void segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (segmented.SelectedIndex == 0)//单人模式
            {
                //ResultTextBox.Text = "dan";
                StartorStopButton.Background = new SolidColorBrush(Colors.Blue);
            }
            else//多人模式
            {
                StartorStopButton.Background = new SolidColorBrush(Colors.Blue);
            }
        }

        private void changeCheckBoxes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            List<int> checkedCheckBoxes = new List<int>();
            checkedCheckBoxes.Clear();
            string[] lines = classpage.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)//此处i指Index
                {
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                    checkBox[i].IsChecked = true;
                    checkedCheckBoxes.Add(i);
                }
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<int> checkedCheckBoxes = new List<int>();
            checkedCheckBoxes.Clear();
            string[] lines = classpage.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)//此处i指Index
                {
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                    checkBox[i].IsChecked = false;
                }
            }
        }
    }
}
