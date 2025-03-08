using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Classes;
using 随机抽取学号.Controls;

namespace 随机抽取学号.Views
{
    public class CheckBoxItem
    {
        public string Name { get; set; } // 以姓名作为Content
        public bool IsChecked { get; set; } // 勾选状态
    }
    public sealed partial class HomePage : Page
    {

        public DispatcherTimer timer = new DispatcherTimer();
        private bool isRandomizing = false;//是否正在单人模式随机抽取

        List<string> checkBoxItemsName = new List<string>();// 记录每个CheckBox的Name
        List<int> checkedCheckBoxesIndex = new List<int>();// 记录每个被选中CheckBox的Index
        List<string> checkedCheckBoxesName = new List<string>();// 记录每个被选中CheckBox的Name
        ObservableCollection<Student> selectedStudentList = new ObservableCollection<Student>();// 记录多选模式下选中的学生
        private GridView PhotosGridView;
        private int randomIndex;
        public HomePage()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = TimeSpan.FromMilliseconds(20);//默认每秒50次
            timer.Tick += Timer_Tick;
            await  CreateCheckBoxes();
            EndNumberBox.Value = StudentManager.StudentList.Count;
            BeginNumberBox.Maximum = StudentManager.StudentList.Count;
            EndNumberBox.Maximum = StudentManager.StudentList.Count;
            if(StudentManager.checkedCheckBoxes.Count > 0)
            {
                Numbers.Maximum = StudentManager.checkedCheckBoxes.Count;
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
        private async Task CreateCheckBoxes()
        {
            StudentManager.CheckBoxItems.Clear();
            checkedCheckBoxesName = StudentManager.checkedCheckBoxes.Select(x => x.Name).ToList();// 获取所有被选中CheckBox的Name
            for (int i = 0; i < StudentManager.StudentList.Count; i++)
            {
                var item = new CheckBoxItem();
                {
                    item.Name = StudentManager.StudentList[i].Name;
                    if (checkedCheckBoxesName.Contains(item.Name) == true)
                    {
                        item.IsChecked = true;
                    }
                    else
                    {
                        item.IsChecked = false;
                    }
                }
                StudentManager.CheckBoxItems.Add(item);
            }
            CheckBoxListView.ItemsSource = StudentManager.CheckBoxItems;
            if (StudentManager.checkedCheckBoxes.Count==StudentManager.StudentList.Count &&StudentManager.StudentList.Count > 0)
            { 
                SelectAllCheckBox.IsChecked = true;
            }
            else if (StudentManager.checkedCheckBoxes.Count==0)
            {
                SelectAllCheckBox.IsChecked = false;
            }
            else
            {
                SelectAllCheckBox.IsChecked = null;
            }
            checkedCheckBoxesCount.Text = "已选择" + StudentManager.checkedCheckBoxes.Count.ToString() + "/" + StudentManager.StudentList.Count.ToString();
        }
        private async Task SaveCheckedCheckBoxesAsync()//将选中的CheckBox存入localSettings
        {
            StudentManager.checkedCheckBoxes.Clear();
            if (StudentManager.StudentList.Count > 0)
            {
                for (int i = 0; i < StudentManager.StudentList.Count; i++)
                {
                    if (StudentManager.CheckBoxItems[i].IsChecked== true)
                    {
                        CheckedCheckBox checkedCheckBox = new CheckedCheckBox()
                        {
                            Index = i,
                            Name = StudentManager.CheckBoxItems[i].Name
                        };
                        StudentManager.checkedCheckBoxes.Add(checkedCheckBox);
                    }
                }
                //将checkBoxes转换为json文件存入应用文件夹
                try
                {
                    await StudentManager.SaveCheckedStudentsAsync(StudentManager.checkedCheckBoxes);
                }
                catch (Exception)
                {
                    PopupNotice popupNotice = new PopupNotice("自动保存抽取范围失败");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
                checkedCheckBoxesCount.Text = "已选择" + StudentManager.checkedCheckBoxes.Count.ToString() + "/" + StudentManager.StudentList.Count.ToString();
                Numbers.Minimum = 1;
                if (StudentManager.checkedCheckBoxes.Count > 0)//重新设置最大抽取人数
                {
                    Numbers.Maximum = StudentManager.checkedCheckBoxes.Count;
                }
                // 重新判断是否全选
                if(StudentManager.checkedCheckBoxes.Count == StudentManager.StudentList.Count)
                {
                    SelectAllCheckBox.IsChecked = true;
                }
                else if (StudentManager.checkedCheckBoxes.Count == 0)
                {
                    SelectAllCheckBox.IsChecked = false;
                }
                else
                {
                    SelectAllCheckBox.IsChecked = null;
                }
            }
        }
        private async void checkBox_Click(object sender, RoutedEventArgs e)
        {
            await SaveCheckedCheckBoxesAsync();
        }
        private async void StartorStopButton_Click(object sender, RoutedEventArgs e)//仅在单人模式有效
        {
            //PopupMessage.ShowPopupMessage("ss", "This is a popup message from AnotherPage.",InfoBarSeverity.Success);
            //PopupMessage popup = new PopupMessage("ss6789", "This is a popup message from AnotherPage.", InfoBarSeverity.Success);
            PopupMessage popup = new PopupMessage();
            popup.ShowMessage();
            //MainPage.PopupContainerInstance.Children.Add(popup);
            //popup.VerticalAlignment = VerticalAlignment.Bottom;

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
                        StudentManager.CheckBoxItems[randomIndex].IsChecked = false;
                        await SaveCheckedCheckBoxesAsync();
                    }
                }
            }
        }
        private void Timer_Tick(object sender, object e)
        {
            if(StudentManager.checkedCheckBoxes .Count != 0)
            {
                Random random = new Random();
                var checkBoxItemsName = StudentManager.CheckBoxItems.Select(x => x.Name).ToList();// 获取所有CheckBox的Name
                List<int> checkedCheckBoxesIndex = new List<int>();// 用于存储被选中的CheckBox的索引
                foreach (var item in StudentManager.checkedCheckBoxes)
                {
                    var index = checkBoxItemsName.IndexOf(item.Name);
                    checkedCheckBoxesIndex.Add(index);
                }

                randomIndex = checkedCheckBoxesIndex[random.Next(StudentManager.checkedCheckBoxes.Count)];
                StudentPhoto.Source = new BitmapImage(new Uri(StudentManager.StudentList[randomIndex].PhotoPath));
                ResultTextBox.Text = (randomIndex + 1).ToString() + "." + StudentManager.StudentList[randomIndex].Name;
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

        private async void changeCheckBoxes_Click(object sender, RoutedEventArgs e)
        {
            StudentManager.checkedCheckBoxes.Clear();
            if (StudentManager.StudentList.Count > 0)
            {
                if (isOnlyThisRangeCheckBox.IsChecked == true)
                {
                    for (int i = 0; i < StudentManager.StudentList.Count; i++)
                    {
                        StudentManager.CheckBoxItems[i].IsChecked = false;// 若勾选"仅选择此范围"，先取消所有CheckBox的选中状态
                    }
                }
                if (int.TryParse(BeginNumberBox.Text, out int beginnum) == true && int.TryParse(EndNumberBox.Text, out int endnum) == true)
                {
                    // 转换成功
                    for (int i = beginnum; i <= endnum; i++)
                    {
                        StudentManager.CheckBoxItems[i - 1].IsChecked = true;// 将指定范围内的CheckBox设置为选中状态
                    }
                    CheckBoxListView.ItemsSource = null;
                    CheckBoxListView.ItemsSource = StudentManager.CheckBoxItems;
                    await SaveCheckedCheckBoxesAsync();
                    PopupNotice popupNotice = new PopupNotice("成功应用更改");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                    checkedCheckBoxesCount.Text = "已选择" + StudentManager.checkedCheckBoxes.Count.ToString() + "/" + StudentManager.StudentList.Count.ToString();
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


        private async void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SelectAllCheckBox.IsChecked == null)
            {
                SelectAllCheckBox.IsChecked = false;//在未全选状态下点击全选，全部不选
            }
            if (SelectAllCheckBox.IsChecked == true)
            {
                foreach (var item in StudentManager.CheckBoxItems)
                {
                    item.IsChecked = true;
                }
            }
            else if (SelectAllCheckBox.IsChecked == false)
            {
                foreach (var item in StudentManager.CheckBoxItems)
                {
                    item.IsChecked = false;
                }
            }

            CheckBoxListView.ItemsSource = null;
            CheckBoxListView.ItemsSource = StudentManager.CheckBoxItems;
            await SaveCheckedCheckBoxesAsync();
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

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)//仅在多人模式有效
        {
            //停止抽取
            timer.Stop();
            isRandomizing = false;
            StartorStopButtonContent.Text = "开始";
            StartorStopButtonIcon.Symbol = Symbol.Play;
            StartorStopButton.Background = new SolidColorBrush(new Color() { A = 100, R = 108, G = 229, B = 89 });
            if (StudentManager.StudentList.Count > 0)
            {
                if (int.TryParse(Numbers.Text, out int a))
                {
                    // 转换成功
                    if (a <= StudentManager.checkedCheckBoxes.Count)
                    {
                        //将checkedCheckBoxes打乱顺序
                        Random random = new Random();
                        StudentManager.checkedCheckBoxes = StudentManager.checkedCheckBoxes.OrderBy(x => random.Next()).ToList();
                        var checkBoxItemsName = StudentManager.CheckBoxItems.Select(x => x.Name).ToList();// 获取所有CheckBox的Name
                        List<int> checkedCheckBoxesIndex = new List<int>();// 用于存储被选中的CheckBox的索引
                        foreach (var item in StudentManager.checkedCheckBoxes)
                        {
                            var index = checkBoxItemsName.IndexOf(item.Name);
                            checkedCheckBoxesIndex.Add(index);
                        }
                        // 取前几位数字
                        List<int> Result = checkedCheckBoxesIndex.Take(a).ToList();
                        selectedStudentList.Clear();
                        for (int i = 0; i < Result.Count; i++)
                        {
                            int randomIndex = Result[i];
                            var item = new Student { StudentNumber = randomIndex + 1, PhotoPath = StudentManager.StudentList[randomIndex].PhotoPath, Name = StudentManager.StudentList[randomIndex].Name };//Id表示学号，从1开始
                            selectedStudentList.Add(item);
                            if (NoReturnCheckBox.IsChecked == true)//抽完不放回
                            {
                                StudentManager.CheckBoxItems[randomIndex].IsChecked = false;
                            }
                        }
                        LoadPhotosGridView();
                        await SaveCheckedCheckBoxesAsync();
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
    }
}
