using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Classes;

namespace 随机抽取学号.Views
{
    public sealed partial class HomePage : Page
    {

        public DispatcherTimer timer = new DispatcherTimer();
        private bool isRandomizing = false;
        List<int> checkedCheckBoxes = new List<int>();
        ClassPage ClassPage = new ClassPage();
        ObservableCollection<Student> selectedStudentList = new ObservableCollection<Student>();//记录多选模式下选中的学生
        private GridView PhotosGridView;
        private int randomIndex;
        //string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
        //string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        public HomePage()
        {
            this.InitializeComponent();
        }
        private void Timer1_Tick(object sender, object e)
        {
           CreateCheckBoxes();
            // 停止计时器，避免重复加载
            ((DispatcherTimer)sender).Stop();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var timer1 = new DispatcherTimer();//使用定时器在Page加载后加载CheckBoxes以提高性能
            timer1.Tick += Timer1_Tick;
            timer1.Start();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            EndNumberBox.Value = lines.Length;
            BeginNumberBox.Maximum = lines.Length;
            EndNumberBox.Maximum = lines.Length;
            if(checkedCheckBoxes.Count > 0)
            {
                Numbers.Maximum = checkedCheckBoxes.Count;
            }
            Numbers.Minimum = 1;
            segmented.SelectedIndex = 0;//在xaml中设置会导致后面的控件未加载就被调用//System.NullReferenceException:“Object reference not set to an instance of an object.”
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();
            GC.Collect();
        }
        private void LoadPhotosGridView()//多人模式下加载PhotosGridView
        {
            PhotosGrid.Children.Clear();
            PhotosGridView = new();
            PhotosGrid.Children.Add(PhotosGridView);
            PhotosGridView.ItemTemplate = (DataTemplate)Resources["GridViewItemTemplate"];
            PhotosGridView.Style = (Style)Resources["GridViewStyle"];
            PhotosGridView.ItemsSource = selectedStudentList;
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
                    checkBox.Content += "未知";
                    checkBox.IsChecked = false;
                    checkBox.IsEnabled = false;
                }

            }
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
                localSettings.Values["checkedCheckBoxesString"] = checkedCheckBoxesString;
                checkedCheckBoxesCount.Text = "已选择" + checkedCheckBoxes.Count.ToString() + "/" + _lines.Length.ToString();
                Numbers.Maximum = checkedCheckBoxes.Count;
            }
            else
            {
                checkedCheckBoxesCount.Text = "未选择";
            }
            if (checkedCheckBoxes.Count.ToString()==_lines.Length.ToString())
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

        private void UpdateCheckedCheckBoxes()
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxes.Clear();
            if (lines.Length > 0)
            {
                for (int i = 0; i < lines.Length; i++)//此处i指Index
                {
                    var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
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
                Numbers.Minimum = 1;
                if (checkedCheckBoxes.Count > 0)//重新设置最大抽取人数
                {
                    Numbers.Maximum = checkedCheckBoxes.Count;
                }
            }
        }
        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            UpdateCheckedCheckBoxes();
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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
        private async void StartorStopButton_Click(object sender, RoutedEventArgs e)//仅在单人模式有效
        {
            //Storyboard1.Begin();
            //Storyboard2.Begin();
            //Storyboard3.Begin();
            //Storyboard4.Begin();

            if (ClassPage.Current.Editor.Text == string.Empty)
            {
                PopupNotice popupNotice = new PopupNotice("请先填写班级信息");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
            else
            {
                ResultTextBox.Visibility = Visibility.Visible;
                if (!isRandomizing)
                {
                    //开始抽取
                    isRandomizing = true;
                    timer.Start();
                    StartorStopButtonContent.Text = "停止";
                    StartorStopButtonIcon.Symbol = Symbol.Pause;
                    StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 236, G = 179, B = 1 });
                }
                else if (isRandomizing)
                {
                    //停止抽取
                    timer.Stop();
                    isRandomizing = false;
                    StartorStopButtonContent.Text = "开始";
                    StartorStopButtonIcon.Symbol = Symbol.Play;
                    StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
                    if (NoReturnCheckBox.IsChecked == true)//抽完不放回
                    {
                        var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                        checkBox[randomIndex].IsChecked = false;
                        UpdateCheckedCheckBoxes();
                        if (checkedCheckBoxes.Count == 0)
                        {
                            SelectAllCheckBox.IsChecked = false;
                        }
                        else
                        {
                            SelectAllCheckBox.IsChecked = null;
                        }
                    }
                }
            }
            StartorStopButton.RenderTransform = new CompositeTransform { ScaleX = 0.9f, ScaleY = 0.9f };
            await Task.Delay(50);
            StartorStopButton.RenderTransform = new CompositeTransform { ScaleX = 1.0f, ScaleY = 1.0f };
        }
        private void Timer_Tick(object sender, object e)
        {
            if(checkedCheckBoxes .Count != 0)
            {
                string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
                Random random = new Random();
                randomIndex = checkedCheckBoxes[random.Next(checkedCheckBoxes.Count)];
                StudentPhoto.Source = new BitmapImage(new Uri(StudentManager.StudentList[randomIndex].PhotoPath));
                ResultTextBox.Text = (randomIndex + 1).ToString() + "." + lines[randomIndex];
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请至少选择一个学生样本");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
                //停止本次抽取
                ((DispatcherTimer)sender).Stop();
                isRandomizing = false;
                StartorStopButtonContent.Text = "开始";
                StartorStopButtonIcon.Symbol = Symbol.Play;
                StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
            }

        }
        int peopleCount = 1;//peopleCount用于记录在多选模式下的抽取人数
        private void segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (segmented.SelectedIndex == 0)//单人模式
            {
                Numbers.IsEnabled = false;//Nnmbers:选择抽取人数
                peopleCount = (int)Numbers.Value;
                Numbers.Text = "1";
                FrequencySelectorButton.IsEnabled = true;
                SingleModeGrid.Visibility = Visibility.Visible;
                MultipleModeGrid.Visibility = Visibility.Collapsed;
            }
            else//多人模式
            {
                Numbers.IsEnabled = true;
                Numbers.Text = peopleCount.ToString();
                FrequencySelectorButton.IsEnabled = false;
                SingleModeGrid.Visibility = Visibility.Collapsed;
                MultipleModeGrid.Visibility = Visibility.Visible;
            }
        }

        private void changeCheckBoxes_Click(object sender, RoutedEventArgs e)
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            checkedCheckBoxes.Clear();
            var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
            if (lines.Length > 0)
            {
                if (isOnlyThisRangeCheckBox.IsChecked == true)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        checkBox[i].IsChecked = false;
                    }
                }
                int beginnum;int endnum;
                if (int.TryParse(BeginNumberBox.Text,out beginnum)==true&& int.TryParse(EndNumberBox.Text, out endnum)==true)
                {
                    // 转换成功
                    for (int i = beginnum; i <= endnum; i++)//此处i指Index
                    {

                        if (checkBox[i - 1].IsEnabled == true)
                        {
                            checkBox[i - 1].IsChecked = true;
                        }
                    }
                    UpdateCheckedCheckBoxes();
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
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
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
                    popupNotice.ShowPopup();

                }

            }
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)//此处i指Index
            {
                var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                if (checkBox[i].IsEnabled == true)
                {
                    checkBox[i].IsChecked = true;
                }
            }
            UpdateCheckedCheckBoxes();
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)//此处i指Index
            {
                var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                if (checkBox[i].IsEnabled == true)
                {
                    checkBox[i].IsChecked = false;
                }
            }
            UpdateCheckedCheckBoxes();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(SelectAllCheckBox.IsChecked==null)
            {
                SelectAllCheckBox.IsChecked = false;
            }
        }
        private void FrequencySelector_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if(FrequencySelector.Value==0)
            {
                FrequencySelector.Value = 1;
            }
            else
            {
                double frequency = 1.0 / FrequencySelector.Value;
                timer.Interval = TimeSpan.FromSeconds(frequency);
                FrequencySelectorButton.Content = "滚动频率:" + FrequencySelector.Value.ToString() + "Hz";
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)//仅在多人模式有效
        {
            //停止抽取
            timer.Stop();
            isRandomizing = false;
            StartorStopButtonContent.Text = "开始";
            StartorStopButtonIcon.Symbol = Symbol.Play;
            StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
            string[] lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.None);
            string[] _lines = ClassPage.Current.Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                int a; bool success = int.TryParse(Numbers.Text, out a);
                if (success)
                {
                    // 转换成功
                    if (a <= checkedCheckBoxes.Count)
                    {
                        //将checkedCheckBoxes打乱顺序
                        Random random = new Random();
                        checkedCheckBoxes = checkedCheckBoxes.OrderBy(x => random.Next()).ToList();
                        // 取前几位数字
                        List<int> Result = checkedCheckBoxes.Take(a).ToList();
                        selectedStudentList.Clear();
                        var checkBox = StackPanelCheckBoxes.Children.OfType<CheckBox>().ToList();// 遍历所有CheckBox
                        for (int i = 0; i < Result.Count; i++)
                        {
                            int randomIndex = Result[i];
                            var item = new Student { Id = randomIndex + 1, PhotoPath = StudentManager.StudentList[randomIndex].PhotoPath, Name = StudentManager.StudentList[randomIndex].Name };//Id表示学号，从1开始
                            selectedStudentList.Add(item);
                            if (NoReturnCheckBox.IsChecked == true)//抽完不放回
                            {
                                checkBox[randomIndex].IsChecked = false;
                            }
                        }
                        LoadPhotosGridView();
                        UpdateCheckedCheckBoxes();
                        if (checkedCheckBoxes.Count == 0)
                        {
                            SelectAllCheckBox.IsChecked = false;
                        }
                        else
                        {
                            SelectAllCheckBox.IsChecked = null;
                        }
                    }
                    else
                    {
                        PopupNotice popupNotice = new PopupNotice("抽取人数不能大于能抽到的人数，请更改左侧勾选框后重试");
                        popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                        popupNotice.ShowPopup();
                    }

                }
                else
                {
                    // 转换失败，NumberBox中的值不是有效的整数
                    Numbers.Text = "1";
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请先填写班级信息");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
        }

        private void StartorStopButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
    //        if (
    //e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    //        {
    //            // 这里处理鼠标左键按下的逻辑
    //            Debug.WriteLine("Mouse left button pressed.");
    //            StartorStopButton.RenderTransform = new CompositeTransform { ScaleX = 0.8f, ScaleY = 0.8f };
    //        }

            var temp = e.GetCurrentPoint(sender as AppBarButton);
            if (temp.Properties.IsLeftButtonPressed)
            {
                PopupNotice popupNotice = new PopupNotice("请先填写班级信息");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
                //MyEllipse.Center = temp.Position;
                var animation = new DoubleAnimation()
                {
                    From = 0,
                    To = 150,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };
                animation.BeginTime = TimeSpan.FromSeconds(0);
                //MyEllipse.BeginAnimation(EllipseGeometry.RadiusXProperty, animation);
                var animation2 = new DoubleAnimation()
                {
                    From = 0.3,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };
                animation2.BeginTime = TimeSpan.FromSeconds(0.5);
                //MyPath.BeginAnimation(Path.OpacityProperty, animation2);
            }

        }

        private void StartorStopButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            StartorStopButton.RenderTransform = new CompositeTransform { ScaleX = 1.0f, ScaleY = 1.0f };
        }
    }
}
