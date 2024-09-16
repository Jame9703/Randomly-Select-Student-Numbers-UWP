using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    public sealed partial class HomePage : Page
    {

        public DispatcherTimer timer;
        private bool isRandomizing = false;
        List<int> checkedCheckBoxes = new List<int>();
        ClassPage ClassPage = new ClassPage();
        //string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
        //string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        public HomePage()
        {
            this.InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateCheckBoxes();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            Text2.Text = System.DateTime.Now.ToString("M");
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            EndNumberBox.Value = lines.Length;
            BeginNumberBox.Maximum = lines.Length;
            EndNumberBox.Maximum = lines.Length;
            segmented.SelectedIndex = 0;//在xaml中设置会导致后面的控件未加载就被调用//System.NullReferenceException:“Object reference not set to an instance of an object.”
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();
            GC.Collect();
        }

        private void CreateCheckBoxes()
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //从应用设置加载checkedCheckBoxes
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["checkedCheckBoxesString"] != null)
            {
                string checkedCheckBoxesString = (string)localSettings.Values["checkedCheckBoxesString"];
                if (checkedCheckBoxesString != String.Empty)
                {
                    checkedCheckBoxes = checkedCheckBoxesString.Split(',').Select(int.Parse).ToList();
                }
                else
                {
                    checkedCheckBoxes = new List<int>();
                }
            }
            else 
            {
                string checkedCheckBoxesString = string.Join(",", checkedCheckBoxes);
                localSettings.Values["checkedCheckBoxesString"] = checkedCheckBoxesString;
            }
            StackPanelCheckBoxes.Children.Clear();//将曾经的CheckBox删去（如果有）
            StackPanelCheckBoxes.Orientation = Orientation.Vertical; //设置为竖直布局
                                                                     //创建CheckBox并在“StackPanelCheckBoxes”中显示
            for (int i = 0; i < lines.Length; i++)//此次i指Index
            {
                var checkBox = new CheckBox();
                string line = lines[i];
                checkBox.Content = i + 1 + "." + line;//遍历Editor中的每一行，用来生成CheckBox集合
                checkBox.Click += checkBox_Click;
                checkBox.Margin = new Thickness(0); //设置边距以避免与其他元素重叠(设置了个寂寞)
                checkBox.IsChecked = true;
                StackPanelCheckBoxes.Children.Add(checkBox);
                checkBox.FontFamily = (FontFamily)Application.Current.Resources["HarmonyOSSans"];
                checkBox.FontSize = 16;
                if (lines[i] != String.Empty)
                {
                    checkBox.IsEnabled = true;
                    if (checkedCheckBoxes.Contains(i) == true)
                    {
                        checkBox.IsChecked = true;
                    }
                    else
                    {
                        checkBox.IsChecked = false;
                    }
                }
                else
                {
                    checkBox.IsChecked = false;
                    checkBox.IsEnabled = false;
                }

            }
            if(checkedCheckBoxes.Count.ToString()==_lines.Length.ToString())
            {
                SelectAllCheckBox.IsChecked = true;
            }
            else if (checkedCheckBoxes.Count==0)
            {
                SelectAllCheckBox.IsChecked = false;
            }
            else
            {
                SelectAllCheckBox.IsChecked = null;
            }
            checkedCheckBoxesCount.Text = "已选择" + checkedCheckBoxes.Count.ToString() + "/" + _lines.Length.ToString();
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxes.Clear();
            if (_lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)//此次i指Index
                {
                    // 遍历所有CheckBox并检查它们的IsChecked属性
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();
                    if (checkBox[i].IsChecked == true)
                    {
                        checkedCheckBoxes.Add(i);
                    }
                }
                //将checkBoxes转换为string存入应用设置
                string checkedCheckBoxesString = string.Join(",", checkedCheckBoxes);
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["checkedCheckBoxesString"] = checkedCheckBoxesString;
                checkedCheckBoxesCount.Text = "已选择" + checkedCheckBoxes.Count.ToString() + "/" + _lines.Length.ToString();
            }
            else
            {
                checkedCheckBoxesCount.Text = "未选择";
            }
            if (checkedCheckBoxes.Count == _lines.Length)
            {
                SelectAllCheckBox.IsChecked = true;
            }
            else if (checkedCheckBoxes.Count == 0)
            {
                SelectAllCheckBox.IsChecked = false;
            }
            else
            {
                SelectAllCheckBox.IsChecked = null;
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
                if (ClassPage.Current.Editor.Text == string.Empty)
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
                checkedCheckBoxes.Clear();
                RandomNumbersGridView.Items.Clear();
                string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
                string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxes.Clear();
            if (_lines.Length > 0)
            {
                for (int i = 0; i < _lines.Length; i++)
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

            }
            else//多人模式
            {

            }
        }

        private void changeCheckBoxes_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxes.Clear();
            var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
            for(int i=0;i<lines.Length;i++)
            {
                checkBox[i].IsChecked = false;
            }
            if (lines.Length > 0)
            {
                int beginnum;int endnum;
                if (int.TryParse(BeginNumberBox.Text,out beginnum)==true&& int.TryParse(EndNumberBox.Text, out endnum)==true)
                {
                    // 转换成功
                    for (int i = beginnum; i <= endnum; i++)//此处i指Index
                    {

                        if (checkBox[i - 1].IsEnabled == true)
                        {
                            checkBox[i - 1].IsChecked = true;
                            checkedCheckBoxes.Add(i - 1);

                        }

                    }
                    if (checkedCheckBoxes.Count == _lines.Length)
                    {
                        SelectAllCheckBox.IsChecked = true;
                    }
                    else if (checkedCheckBoxes.Count == 0)
                    {
                        SelectAllCheckBox.IsChecked = false;
                    }
                    else
                    {
                        SelectAllCheckBox.IsChecked = null;
                    }
                    PopupNotice popupNotice = new PopupNotice("成功应用更改");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                    popupNotice.ShowAPopup();
                    checkedCheckBoxesCount.Text = "已选择" + checkedCheckBoxes.Count.ToString() + "/" + _lines.Length.ToString();
                }
                else
                {
                    if(!int.TryParse(BeginNumberBox.Text, out beginnum))
                    {
                        BeginNumberBox.Value = 1;
                    }
                    if (!int.TryParse(EndNumberBox.Text, out endnum))
                    {
                        EndNumberBox.Value = lines.Length;
                    }
                    PopupNotice popupNotice = new PopupNotice("请输入范围内整数");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowAPopup();

                }

            }
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxes.Clear();
            if (lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)//此处i指Index
                {
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                    if (checkBox[i].IsEnabled==true)
                    {
                        checkBox[i].IsChecked = true;
                        checkedCheckBoxes.Add(i);
                    }
                }
                //将checkBoxes转换为string存入应用设置
                string checkedCheckBoxesString = string.Join(",", checkedCheckBoxes);
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["checkedCheckBoxesString"] = checkedCheckBoxesString;
                checkedCheckBoxesCount.Text = "已选择" + checkedCheckBoxes.Count.ToString() + "/" + _lines.Length.ToString();
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxes.Clear();
            if (lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)//此处i指Index
                {
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                    if (checkBox[i].IsEnabled == true)
                    {
                        checkBox[i].IsChecked = false;
                    }
                }
                //将checkBoxes转换为string存入应用设置
                string checkedCheckBoxesString = string.Join(",", checkedCheckBoxes);
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["checkedCheckBoxesString"] = checkedCheckBoxesString;
                checkedCheckBoxesCount.Text = "已选择" + checkedCheckBoxes.Count.ToString() + "/" + _lines.Length.ToString();
            }
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(SelectAllCheckBox.IsChecked==null)
            {
                SelectAllCheckBox.IsChecked = false;
            }
        }

    }
}
