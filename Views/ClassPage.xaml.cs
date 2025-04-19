using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Classes;

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClassPage : Page
    {
        public List<string> names = new List<string>();
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private ListViewItem previousExpandedItem;

        public ClassPage()
        {
            this.InitializeComponent();
            // 初始化行号
            UpdateLineNumbers();
            StudentManager.StudentList.CollectionChanged += StudentList_CollectionChanged;
            FileEditSegmented.SelectedIndex = 0;
            AddModeSegmented.SelectedIndex = 0;
        }

        private async void StudentList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateStudentId();
            await UpdateSaveButton();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            var compositor = ElementCompositionPreview.GetElementVisual(ContentGrid).Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0f, 1f);
            animation.InsertKeyFrame(1f, 0f);
            animation.Duration = TimeSpan.FromSeconds(1);
            var visual = ElementCompositionPreview.GetElementVisual(ContentGrid);
            visual.StartAnimation("Opacity", animation);
            GC.Collect();
        }
        private void UpdateStudentId()//重新更改学号
        {
            for (int i = 0; i < StudentManager.StudentList.Count; i++)
            {
                StudentManager.StudentList[i].StudentNumber = i + 1;
            }
            //StudentListView.ItemsSource = null;
            //StudentListView.ItemsSource = StudentManager.StudentList;
        }
        public void UpdateLineNumbers()
        {
            string text = Editor.Text;
            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            lineNumberTextBlock.Text = string.Empty;
            for (int i = 0; i < lines.Length; i++)
            {
                lineNumberTextBlock.Text += $"{i + 1}\n";
            }
        }


        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".txt");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    // 打开文件并读取内容
                    string text = await FileIO.ReadTextAsync(file);
                    Editor.Text = text;
                    PopupNotice popupNotice = new PopupNotice("成功打开文件: " + file.Name);
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                }
                catch (Exception)
                {
                    PopupNotice popupNotice = new PopupNotice("打开文件失败");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("打开操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
            AddItemButton.Flyout.ShowAt(AddItemButton);
        }
        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建文件选取器
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("文本文档", new List<string>() { ".txt" });
            string filename = (string)localSettings.Values["ClassName"];
            savePicker.SuggestedFileName = filename;

            // 显示文件选取器并等待用户选择文件
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // 将文本写入文件
                string text = Editor.Text;
                await FileIO.WriteTextAsync(file, text);
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    PopupNotice popupNotice = new PopupNotice(file.Name + " 保存成功");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                }
                else
                {
                    PopupNotice popupNotice = new PopupNotice(file.Name + " 保存失败");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("保存操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
        }
        public void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                localSettings.Values["Names"] = Editor.Text;
            }
            catch (Exception)
            {
                PopupNotice popupNotice = new PopupNotice("保存失败，请删除部分名字后重试");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
            UpdateLineNumbers();

        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLineNumbers();
        }

        private async void FolderPickerButton_Click(object sender, RoutedEventArgs e)
        {
            if(StudentListView.SelectedItem != null)
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.FileTypeFilter.Add(".jpg");
                folderPicker.FileTypeFilter.Add(".jpeg");
                folderPicker.FileTypeFilter.Add(".png");
                folderPicker.FileTypeFilter.Add(".bmp");

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                    int i = 0;
                    while (i < files.Count)//支持同时拖入多个文件
                    {
                        var selectedItem = StudentListView.SelectedItem as Student;
                        var file = files[i] as StorageFile;
                        var contentType = file.ContentType;
                        if (contentType == "image/jpg" || contentType == "image/jpeg" || contentType == "image/png" || contentType == "image/bmp")
                        {
                            var localFile = await localFolder.CreateFileAsync(files[i].Name, CreationCollisionOption.ReplaceExisting);
                            await file.CopyAndReplaceAsync(localFile);
                            StorageFile photoFile = await localFolder.GetFileAsync(file.Name);
                            if (StudentListView.Items.Count > StudentListView.SelectedIndex + 1)//超出部分不填充
                            {
                                selectedItem.PhotoPath = photoFile.Path;
                                StudentListView.SelectedItem = StudentListView.Items[StudentListView.SelectedIndex + 1];
                            }
                            else if (StudentListView.Items.Count == StudentListView.SelectedIndex + 1)//当前选择的是最后一项
                            {
                                selectedItem.PhotoPath = photoFile.Path;
                                break;//避免被后面的图片覆盖
                            }
                        }
                        i++;
                    }
                    await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
                    //StudentListView.ItemsSource = null;
                    //StudentListView.ItemsSource = StudentManager.StudentList;
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请先选择开始填充的项");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
            }
        }

        private void StudentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (StudentListView.SelectedItem != null)
            //{
            //    CurrentSelectionTextBlock.Text = "当前选择项:"+(StudentListView.SelectedIndex+1).ToString();
            //}
            //else
            //{
            //    CurrentSelectionTextBlock.Text = "当前选择项:无";
            //}
            //// 还原前一项
            ////if (previousExpandedItem != null)
            ////{


            ////}
            //CollapseStudentListViewItemStorybord.Stop();
            //ExpandStudentListViewItemStorybord.Stop();
            //// 获取当前选中项
            //var selectedItem = StudentListView.SelectedItem;
            //if (selectedItem != null)
            //{
            //    // 获取当前选中项的 ListViewItem
            //    var selectedListViewItem = (ListViewItem)StudentListView.ContainerFromItem(selectedItem);
            //    if (selectedListViewItem != null)
            //    {
            //        if(previousExpandedItem != null)
            //        {
            //            //将前一项折叠，将当前项展开
            //            //Storyboard.SetTarget(CollapseStudentListViewItemStorybord, previousExpandedItem);
            //            //Storyboard.SetTarget(ExpandStudentListViewItemStorybord, selectedListViewItem);
            //            //CollapseStudentListViewItemStorybord.Begin();
            //            //ExpandStudentListViewItemStorybord.Begin();
            //            previousExpandedItem.ContentTemplate = (DataTemplate)Resources["CollapsedTemplate"];
            //            selectedListViewItem.ContentTemplate = (DataTemplate)Resources["ExpandedTemplate"];
            //        }
            //        else
            //        {
            //            //Storyboard.SetTarget(ExpandStudentListViewItemStorybord, selectedListViewItem);
            //            //ExpandStudentListViewItemStorybord.Begin();
            //            selectedListViewItem.ContentTemplate = (DataTemplate)Resources["ExpandedTemplate"];
            //        }
            //        previousExpandedItem = selectedListViewItem;
            //    }
            //}
        }

        private async void StudentListView_Drop(object sender, DragEventArgs e)
        {
            if (StudentListView.SelectedItem != null)
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (e.DataView.Contains(StandardDataFormats.StorageItems) && items.Any())
                {
                    int i = 0;
                    while (i < items.Count)//支持同时拖入多个文件
                    {
                        var file = items[i] as StorageFile;
                        var selectedItem = StudentListView.SelectedItem as Student;
                        try
                        {
                            var contentType = file.ContentType;
                            if (contentType == "image/jpg" || contentType == "image/jpeg" || contentType == "image/png" || contentType == "image/bmp")
                            {
                                var localFile = await localFolder.CreateFileAsync(items[i].Name, CreationCollisionOption.ReplaceExisting);
                                await file.CopyAndReplaceAsync(localFile);
                                StorageFile photoFile = await localFolder.GetFileAsync(file.Name);
                                if (StudentListView.Items.Count > StudentListView.SelectedIndex + 1)//超出部分不填充
                                {
                                    selectedItem.PhotoPath = photoFile.Path;
                                    StudentListView.SelectedItem = StudentListView.Items[StudentListView.SelectedIndex + 1];
                                }
                                else if (StudentListView.Items.Count == StudentListView.SelectedIndex + 1)//当前选择的是最后一项
                                {
                                    selectedItem.PhotoPath = photoFile.Path;
                                    break;//避免被后面的图片覆盖
                                }
                            }
                            i++;
                        }
                        catch (Exception)
                        {
                            PopupNotice popupNotice = new PopupNotice("填充失败");
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                    }
                    await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
                    //StudentListView.ItemsSource = null;
                    //StudentListView.ItemsSource = StudentManager.StudentList;
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请先选择要开始填充的项");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
            }
            //var listView = sender as ListView;
            //var dropPosition = e.GetPosition(listView);
            //var hitTest = VisualTreeHelper.FindElementsInHostCoordinates(dropPosition, listView);

            //foreach (var element in hitTest)
            //{
            //    if (element is ListViewItem item)
            //    {
            //        item.Background = new SolidColorBrush(Colors.Red);
            //        var targetIndex = listView.IndexFromContainer(item);
            //        PopupNotice popupNotice = new PopupNotice(targetIndex.ToString());
            //        popupNotice.ShowPopup();
            //        // 通过 targetIndex 定位具体项
            //        break;

            //    }
            //}

        }

        private void StudentListView_DragOver(object sender, DragEventArgs e)
        {
            var StudentListView = sender as ListView;
            StudentListView.CanDrag = true;
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "替换被选择项的照片";
        }

        private async void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentListView.SelectedItem != null)
            {
                var selectedIndex = StudentListView.SelectedIndex;
                var selectedItem = StudentListView.SelectedItem as Student;
                StudentManager.StudentList.Remove(selectedItem);
                //if (StudentManager.ch.Contains(selectedItem.Name))
                //{
                //    StudentManager.checkedCheckBoxes.Remove(selectedItem.Name);
                //    await StudentManager.SaveCheckedStudentsAsync(StudentManager.checkedCheckBoxes);
                //}
                try
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(selectedItem.PhotoPath);//一并删除存入LocalFolder的照片
                    await file.DeleteAsync(StorageDeleteOption.Default);
                }
                catch (Exception)
                {
                    //使用的是默认照片
                }

                UpdateStudentId();
                if (selectedIndex < StudentListView.Items.Count)
                {
                    StudentListView.SelectedItem = StudentListView.Items[selectedIndex];//默认选择被删除项的下一项
                }

            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请先选择要删除的项");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //UpdateStudentId();
            await UpdateSaveButton();
        }
        private async Task UpdateSaveButton()
        {
            //var focusedElement = FocusManager.GetFocusedElement() as Control;
            //focusedElement.Focus(FocusState.Programmatic);
            //StudentListView.SelectedItem = null;
            if (SaveProcessBar.Value == 100)
            {
                SaveProcessBar.Value = 0;
            }
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += Timer_Tick;
            timer.Start();
            await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
        }
        private async void Timer_Tick(object sender, object e)
        {
            if (StudentManager.SaveStudentsProcess >= 0 && StudentManager.SaveStudentsProcess < 100)
            {
                SaveProcessBar.Value = StudentManager.SaveStudentsProcess;
                SavingStackPanel.Visibility = Visibility.Visible;
                SaveStackPanel.Visibility = Visibility.Collapsed;
                SavedStackPanel.Visibility = Visibility.Collapsed;
                SaveButtonGrid.Background = new SolidColorBrush
                { 
                    Color = Colors.LightBlue,
                    Opacity = 0.5
                };
            }
            else
            {
                SaveProcessBar.Value = 100;
                ((DispatcherTimer)sender).Stop();
                SavedStackPanel.Visibility = Visibility.Visible;
                SavingStackPanel.Visibility = Visibility.Collapsed;
                SaveStackPanel.Visibility = Visibility.Collapsed;
                SaveButtonGrid.Background = new SolidColorBrush
                {
                    Color = Colors.LightGreen,
                    Opacity = 0.5
                };
                await Task.Delay(5000);
                SaveStackPanel.Visibility = Visibility.Visible;
                SavedStackPanel.Visibility = Visibility.Collapsed;
                SavingStackPanel.Visibility = Visibility.Collapsed;
                SaveButtonGrid.Background = new SolidColorBrush
                {
                    Color = Colors.LightYellow,
                    Opacity = 0.5
                };
                SaveProcessBar.Value = 0;
            }
        }

        private void StudentListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            //double scrollOffset = GetScrollViewerVerticalOffset(StudentListView);
            UpdateStudentId();
            //SetScrollViewerVerticalOffset(StudentListView, scrollOffset);
        }
        private double GetScrollViewerVerticalOffset(ListView listView)
        {
            ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(listView);
            return scrollViewer?.VerticalOffset ?? 0;
        }

        private void SetScrollViewerVerticalOffset(ListView listView, double offset)
        {
            ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(listView);
            if (scrollViewer != null)
            {
                scrollViewer.ChangeView(null, offset, null);
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
        private async  void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if(AddModeSegmented.SelectedIndex == 0)//逐个添加
            {
                var student = new Student
                {
                    StudentNumber = StudentManager.StudentList.Count + 1,
                    Name = NameTextBox.Text,
                    PhotoPath = "ms-appx:///Assets/RSSN_Logos/StoreLogo.scale-400.png"
                };
                StudentManager.StudentList.Add(student);
            }
            else//批量添加
            {
                StudentManager.StudentList.CollectionChanged -= StudentList_CollectionChanged;
                string[] lines = Editor.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var student = new Student
                    {
                        StudentNumber = StudentManager.StudentList.Count + 1,
                        Name = line,
                        PhotoPath = "ms-appx:///Assets/RSSN_Logos/StoreLogo.scale-400.png"
                    };
                    StudentManager.StudentList.Add(student);
                }
                StudentManager.StudentList.CollectionChanged += StudentList_CollectionChanged;
            }
            UpdateStudentId();
        }

        private void FileEditSegmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileEditSegmented.SelectedIndex == 0)//文件
            {
                EditStackPanel.Visibility = Visibility.Collapsed;
                FileStackPanel.Visibility = Visibility.Visible;
            }
            else//编辑
            {
                FileStackPanel.Visibility = Visibility.Collapsed;
                EditStackPanel.Visibility = Visibility.Visible;
            }
        }

        private void AddModeSegmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AddModeSegmented.SelectedIndex == 0)//逐个添加
            {
                SingleAddGrid.Visibility = Visibility.Visible;
                BatchAddGrid.Visibility = Visibility.Collapsed;
            }
            else//批量添加
            {
                SingleAddGrid.Visibility = Visibility.Collapsed;
                BatchAddGrid.Visibility = Visibility.Visible;
            }
        }

        private async void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            StudentManager.StudentList.Clear();
            StudentManager.CheckedStudents.Clear();
            await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
            await StudentManager.SaveCheckedStudentsAsync(StudentManager.SelectedRanges);
        }

        private async void OpenLocalFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取应用的本地文件夹
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            // 使用文件管理器打开本地文件夹
            await LaunchFolderInFileManager(localFolder);
        }
        private async Task LaunchFolderInFileManager(StorageFolder folder)
        {
            try
            {
                
                bool success = await Launcher.LaunchFolderAsync(folder);// 尝试使用文件管理器打开文件夹
                if (!success)
                {
                    // 打开失败
                    PopupNotice popupNotice = new PopupNotice("打开失败，请重试");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                PopupNotice popupNotice = new PopupNotice("打开失败，请重试");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
        }

        private async void OpenDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            // 设置选取器的起始位置
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // 添加允许的文件类型
            openPicker.FileTypeFilter.Add(".db");

            // 显示选取器并等待用户选择文件
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    SqliteConnection.ClearAllPools();
                    //打开所选文件的流以进行读取
                    using (Stream sourceStream = await file.OpenStreamForReadAsync())
                    {
                        //打开目标文件的流以进行写入
                        StorageFile destinationFile = await localFolder.CreateFileAsync("students.db", CreationCollisionOption.ReplaceExisting);
                        using (Stream destinationStream = await destinationFile.OpenStreamForWriteAsync())
                        {
                            //复制流内容
                           await sourceStream.CopyToAsync(destinationStream);

                        }
                    }
                    StudentManager.StudentList = await StudentManager.LoadStudentsAsync();
                    //StudentListView.ItemsSource = StudentManager.StudentList;
                    PopupNotice popupNotice = new PopupNotice("打开数据库成功");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                }
                catch (Exception)
                {
                    PopupNotice popupNotice = new PopupNotice("打开数据库失败，请重试");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("打开数据库操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
        }

        private async void SaveDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            // 设置选取器的起始位置
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // 添加允许的文件类型
            savePicker.FileTypeChoices.Add("数据库文件", new[] { ".db" });
            // 设置建议的文件名
            savePicker.SuggestedFileName = "students.db";
            // 显示选取器并等待用户选择保存位置和文件名
            StorageFile saveFile = await savePicker.PickSaveFileAsync();
            if(saveFile != null)
            {
                try
                {
                    StorageFile sourceFile = await localFolder.TryGetItemAsync("students.db") as StorageFile;
                    await UpdateSaveButton();
                    await StudentManager.SaveStudentsAsync(StudentManager.StudentList);

                    // 打开源文件进行读取
                    using (Stream sourceStream = await sourceFile.OpenStreamForReadAsync())
                    {
                        // 打开目标文件进行写入
                        using (Stream targetStream = await saveFile.OpenStreamForWriteAsync())
                        {
                            // 将源文件的内容复制到目标文件
                            await sourceStream.CopyToAsync(targetStream);
                        }
                    }

                    PopupNotice popupNotice = new PopupNotice("另存数据库成功");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
                    popupNotice.ShowPopup();
                }
                catch (Exception)
                {
                    PopupNotice popupNotice = new PopupNotice("另存数据库失败，请重试");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("另存数据库操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
        }

        private void ExpandStudentListViewItemStorybord_Completed(object sender, object e)
        {
            var selectedListViewItem = (ListViewItem)StudentListView.ContainerFromItem(StudentListView.SelectedItem);
            //previousExpandedItem.ContentTemplate = (DataTemplate)Resources["CollapsedTemplate"];
            selectedListViewItem.ContentTemplate = (DataTemplate)Resources["ExpandedTemplate"];
        }

        private void CollapseStudentListViewItemStorybord_Completed(object sender, object e)
        {
            previousExpandedItem.ContentTemplate = (DataTemplate)Resources["CollapsedTemplate"];
            //selectedListViewItem.ContentTemplate = (DataTemplate)Resources["ExpandedTemplate"];
        }
    }

}

