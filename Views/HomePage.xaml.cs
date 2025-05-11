using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Classes;
using 随机抽取学号.Controls;

namespace 随机抽取学号.Views
{
    public sealed partial class HomePage : Page
    {

        public DispatcherTimer timer = new DispatcherTimer();
        private bool isRandomizing = false;//是否正在单人模式随机抽取
        ObservableCollection<Student> selectedStudentList = new ObservableCollection<Student>();// 记录多选模式下选中的学生
        private int randomIndex;
        private int currentIndex = 0;
        Random random = new Random();
        public HomePage()
        {
            this.InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(20);//默认每秒50次
            timer.Tick += Timer_Tick;
            segmented.SelectedIndex = 0;//在xaml中设置会导致后面的控件未加载就被调用//System.NullReferenceException:“Object reference not set to an instance of an object.”
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBoxListView.ItemsSource = StudentManager.StudentList;
            if (StudentManager.CheckedStudents.Count <= StudentManager.StudentList.Count)
            {
                CheckBoxListView.SelectionChanged -= CheckBoxListView_SelectionChanged;
                foreach (var range in StudentManager.SelectedRanges)
                {
                    CheckBoxListView.SelectRange(range);
                }
                CheckBoxListView.SelectionChanged += CheckBoxListView_SelectionChanged;
                UpdateCheckedStudentsCount();//更新已选人数

                EndNumberBox.Value = StudentManager.StudentList.Count;
                BeginNumberBox.Maximum = StudentManager.StudentList.Count;
                EndNumberBox.Maximum = StudentManager.StudentList.Count;
                if (StudentManager.CheckedStudents.Count > 0)
                {
                    Numbers.Maximum = StudentManager.CheckedStudents.Count;
                }
                Numbers.Minimum = 1;
                ContentGrid.Visibility = Visibility.Visible;
                LoadProgressRing.Visibility = Visibility.Collapsed;
            }
            else //上一次保存StudentList失败
            {
                StudentManager.CheckedStudents.Clear();
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();
            GC.Collect();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        private void UpdateCheckedStudentsCount()
        {
            UpdateCheckedStudents();
            Numbers.Minimum = 1;
            if (StudentManager.CheckedStudents.Count > 0)//重新设置最大抽取人数
            {
                Numbers.Maximum = StudentManager.CheckedStudents.Count;
            }
            //重新判断是否全选
            if (StudentManager.CheckedStudents.Count == StudentManager.StudentList.Count && StudentManager.StudentList.Count > 0)
            {
                SelectAllCheckBox.IsChecked = true;
            }
            else if (StudentManager.CheckedStudents.Count == 0)
            {
                SelectAllCheckBox.IsChecked = false;
            }
            else
            {
                SelectAllCheckBox.IsChecked = null;
            }
            //更新已选人数
            SelectAllCheckBox.Content = StudentManager.CheckedStudents.Count.ToString() + "/" + StudentManager.StudentList.Count.ToString();
        }
        private async Task SaveCheckedStudentsAsync()
        {
            if (StudentManager.StudentList.Count > 0)
            {
                //将SelectedRanges转换为数据库文件存入应用文件夹
                await StudentManager.SaveCheckedStudentsAsync(StudentManager.SelectedRanges);
            }
        }
        private void StartorStopButton_Click(object sender, RoutedEventArgs e)//仅在单人模式有效
        {
            PopupMessage popup = new PopupMessage();
            popup.ShowMessage();

            if (StudentManager.StudentList.Count == 0)
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
                    StartorStopButtonIcon.Glyph = "\uE62E";
                    StartorStopButtonIcon.Foreground = new SolidColorBrush(Colors.Yellow);
                    StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 236, G = 179, B = 1 });
                    //根据SelectedRanges更新CheckedStudents
                    UpdateCheckedStudents();
                    if (SettingsHelper.Optimize == true)//启用优化
                    {
                        for (int i = 0; i < StudentManager.CheckedStudents.Count; i++)//洗牌算法，全部打乱
                        {
                            int j = random.Next(StudentManager.CheckedStudents.Count);
                            var temp = StudentManager.CheckedStudents[i];
                            StudentManager.CheckedStudents[i] = StudentManager.CheckedStudents[j];
                            StudentManager.CheckedStudents[j] = temp;
                        }
                    }
                }
                else if (isRandomizing)
                {
                    //停止抽取
                    timer.Stop();
                    isRandomizing = false;
                    StartorStopButtonContent.Text = "开始";
                    StartorStopButtonIcon.Glyph = "\uF5B0";
                    StartorStopButtonIcon.Foreground = new SolidColorBrush(Colors.Green);
                    StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
                    if (SettingsHelper.NoReturn == true)//抽完不放回
                    {
                        var randomstudent = StudentManager.StudentList[randomIndex];
                        CheckBoxListView.SelectedItems.Remove(randomstudent);
                    }
                }
            }
        }
        private void UpdateCheckedStudents()
        {
            StudentManager.CheckedStudents.Clear();
            foreach (var range in StudentManager.SelectedRanges)
            {
                for (int i = range.FirstIndex; i <= range.LastIndex; i++)
                {
                    StudentManager.CheckedStudents.Add(i);
                }
            }
        }
        private void Timer_Tick(object sender, object e)
        {
            if (StudentManager.CheckedStudents.Count != 0)
            {
                if (SettingsHelper.Optimize == false)//优化开关关闭
                {
                    var randomIndexinCheckedStudents = random.Next(StudentManager.CheckedStudents.Count);
                    randomIndex = StudentManager.CheckedStudents[randomIndexinCheckedStudents];
                    var randomstudent = StudentManager.StudentList[randomIndex];
                    StudentPhoto.Source = new BitmapImage(new Uri(randomstudent.PhotoPath));
                    ResultTextBox.Text = (randomIndex + 1).ToString() + "." + randomstudent.Name;
                }
                else //优化开关打开
                {
                    if (currentIndex + 1 > StudentManager.CheckedStudents.Count)
                    {
                        currentIndex = 0;
                    }
                    randomIndex = StudentManager.CheckedStudents[currentIndex];
                    StudentPhoto.Source = new BitmapImage(new Uri(StudentManager.StudentList[randomIndex].PhotoPath));
                    ResultTextBox.Text = (randomIndex + 1).ToString() + "." + StudentManager.StudentList[randomIndex].Name;
                    currentIndex++;
                }
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
                StartorStopButtonIcon.Glyph = "\uF5B0";
                StartorStopButtonIcon.Foreground = new SolidColorBrush(Colors.Green);
                StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
            }

        }
        int peopleCount = 1;//peopleCount用于记录在多选模式下的抽取人数
        private void segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (segmented.SelectedIndex == 0)//单人模式
            {
                Numbers.IsEnabled = false;//Numbers:选择抽取人数
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
            if (StudentManager.StudentList.Count > 0)
            {
                if (isOnlyThisRangeCheckBox.IsChecked == true)
                {
                    CheckBoxListView.DeselectAll();
                }
                if (int.TryParse(BeginNumberBox.Text, out int beginnum) == true && int.TryParse(EndNumberBox.Text, out int endnum) == true)
                {
                    // 转换成功,将ListView指定范围内的项设置为选中状态
                    CheckBoxListView.SelectRange(new ItemIndexRange(beginnum - 1, (uint)(endnum - beginnum + 1)));
                    PopupNotice popupNotice = new PopupNotice("成功应用更改");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                }
                else
                {
                    if (!int.TryParse(BeginNumberBox.Text, out beginnum))
                    {
                        BeginNumberBox.Value = 1;
                    }
                    if (!int.TryParse(EndNumberBox.Text, out endnum))
                    {
                        EndNumberBox.Value = StudentManager.StudentList.Count;
                    }
                    PopupNotice popupNotice = new PopupNotice("请输入范围内整数");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
        }


        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SelectAllCheckBox.IsChecked == null)
            {
                SelectAllCheckBox.IsChecked = false;//在未全选状态下点击全选，全部不选
            }
            if (SelectAllCheckBox.IsChecked == true)
            {
                StudentManager.CheckedStudents.Clear();
                CheckBoxListView.SelectAll();
            }
            else if (SelectAllCheckBox.IsChecked == false)
            {
                CheckBoxListView.DeselectAll();
            }
        }
        private void FrequencySelector_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (FrequencySelector.Value == 0)
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
            StartorStopButtonIcon.Glyph = "\uF5B0";
            StartorStopButtonIcon.Foreground = new SolidColorBrush(Colors.Green);
            StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
            if (StudentManager.StudentList.Count > 0)
            {
                if (int.TryParse(Numbers.Text, out int a))
                {
                    // 转换成功
                    if (a <= StudentManager.CheckedStudents.Count)
                    {
                        //将CheckedStudents打乱顺序
                        Random random = new Random();
                        StudentManager.CheckedStudents = StudentManager.CheckedStudents.OrderBy(x => random.Next()).ToList();
                        // 取前几位数字
                        List<int> Result = StudentManager.CheckedStudents.Take(a).ToList();
                        selectedStudentList.Clear();
                        for (int i = 0; i < Result.Count; i++)
                        {
                            var result = Result[i];
                            var student = StudentManager.StudentList[result];
                            selectedStudentList.Add(student);
                            if (SettingsHelper.NoReturn == true)//抽完不放回
                            {
                                CheckBoxListView.SelectedItems.Remove(student);
                            }
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

        private async void CheckBoxListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StudentManager.SelectedRanges = CheckBoxListView.SelectedRanges.ToList();
            UpdateCheckedStudentsCount();
            await SaveCheckedStudentsAsync();
            var selectedranges_string = new List<string>();
            foreach (ItemIndexRange range in StudentManager.SelectedRanges)
            {
                string range_string = "[" + (range.FirstIndex + 1).ToString() + "," + (range.LastIndex + 1).ToString() + "]";
                selectedranges_string.Add(range_string);
            }
            RangeListView.ItemsSource = selectedranges_string;
        }

        private void FoldButton_Click(object sender, RoutedEventArgs e)
        {
            FoldButton.Visibility = Visibility.Collapsed;
            ExpandButton.Visibility = Visibility.Visible;
            CheckBoxSplitViewGridSplitter.Visibility = Visibility.Collapsed;
            CheckBoxGrid.Visibility = Visibility.Collapsed;
            ContentGrid.ColumnDefinitions.Clear();
            Grid.SetColumn(CheckBoxGrid, 0);

            var _compositor = ElementCompositionPreview.GetElementVisual(CheckBoxGrid).Compositor;
            var _splitViewVisual = ElementCompositionPreview.GetElementVisual(CheckBoxGridContent);
            // 展开透明度动画
            ScalarKeyFrameAnimation splitViewOpacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
            splitViewOpacityAnimation.Duration = TimeSpan.FromSeconds(0.5);
            splitViewOpacityAnimation.InsertKeyFrame(0f, 1f);
            splitViewOpacityAnimation.InsertKeyFrame(1f, 0f);
            _splitViewVisual.StartAnimation("Opacity", splitViewOpacityAnimation);

            // 展开位移动画
            Vector3KeyFrameAnimation splitViewOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            splitViewOffsetAnimation.Duration = TimeSpan.FromSeconds(0.5);
            splitViewOffsetAnimation.InsertKeyFrame(0f, new System.Numerics.Vector3((float)CheckBoxGrid.ActualWidth, 0, 0));
            splitViewOffsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3(0, 0, 0));
            _splitViewVisual.StartAnimation("Offset", splitViewOffsetAnimation);
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            var _compositor = ElementCompositionPreview.GetElementVisual(CheckBoxGrid).Compositor;
            var _splitViewVisual = ElementCompositionPreview.GetElementVisual(CheckBoxGridContent);
            // 展开透明度动画
            ScalarKeyFrameAnimation splitViewOpacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
            splitViewOpacityAnimation.Duration = TimeSpan.FromSeconds(0.5);
            splitViewOpacityAnimation.InsertKeyFrame(0f, 0f);
            splitViewOpacityAnimation.InsertKeyFrame(1f, 1f);
            _splitViewVisual.StartAnimation("Opacity", splitViewOpacityAnimation);

            // 展开位移动画
            Vector3KeyFrameAnimation splitViewOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
            splitViewOffsetAnimation.Duration = TimeSpan.FromSeconds(0.5);
            splitViewOffsetAnimation.InsertKeyFrame(0f, new System.Numerics.Vector3((float)CheckBoxGrid.ActualWidth, 0, 0));
            splitViewOffsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3(0, 0, 0));
            _splitViewVisual.StartAnimation("Offset", splitViewOffsetAnimation);
            FoldButton.Visibility = Visibility.Visible;
            ExpandButton.Visibility = Visibility.Collapsed;
            CheckBoxSplitViewGridSplitter.Visibility = Visibility.Visible;
            CheckBoxGrid.Visibility = Visibility.Visible;
            ContentGrid.ColumnDefinitions.Insert(0, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), MinWidth = 300 });
            ContentGrid.ColumnDefinitions.Insert(1, new ColumnDefinition() { Width = new GridLength(12) });
            ContentGrid.ColumnDefinitions.Insert(2, new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto), MinWidth = 150, MaxWidth = 500 });
            Grid.SetColumn(CheckBoxGrid, 2);
        }

        private void SelectFlyout_Opened(object sender, object e)
        {
            var selectedranges_string = new List<string>();
            foreach (ItemIndexRange range in StudentManager.SelectedRanges)
            {
                string range_string = "[" + (range.FirstIndex + 1).ToString() + "," + (range.LastIndex + 1).ToString() + "]";
                selectedranges_string.Add(range_string);
            }
            RangeListView.ItemsSource = selectedranges_string;
        }
    }
}
