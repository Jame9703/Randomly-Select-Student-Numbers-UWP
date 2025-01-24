using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        public ClassPage()
        {
            this.InitializeComponent();
            // 初始化行号
            UpdateLineNumbers();
            //if (localSettings.Values["Names"] != null) Editor.Text = (string)localSettings.Values["Names"];
            if (localSettings.Values["ClassName"] != null) ClassNameTextBox.Text = (string)localSettings.Values["ClassName"];
            StudentManager.StudentList.CollectionChanged += StudentList_CollectionChanged;
            FileEditSegmented.SelectedIndex = 0;
            AddModeSegmented.SelectedIndex = 0;
        }

        private async void StudentList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
            //try
            //{
            //    UpdateStudentId();
            //    await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
            //    StudentListView.ItemsSource = null;
            //    StudentListView.ItemsSource = StudentManager.StudentList;

            //}
            //catch (Exception ex)
            //{

            //}
            //finally
            //{
            //    PopupNotice popupNotice = new PopupNotice("保存学生列表成功");
            //    popupNotice.PopupContent.Severity = InfoBarSeverity.Success;
            //    popupNotice.ShowPopup();
            //}
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //StudentListView.ItemsSource = StudentManager.StudentList;
        }

        private void UpdateStudentId()//重新更改学号
        {
            for (int i = 0; i < StudentManager.StudentList.Count; i++)
            {
                StudentManager.StudentList[i].StudentNumber = i + 1;
            }
            StudentListView.ItemsSource = null;
            StudentListView.ItemsSource = StudentManager.StudentList;
        }
        //protected async override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    /*简介
        //     * 步骤1：将在Editor中做的更改用于刷新StudentListView(比如Editor有3行，但StudentListView只有2项)
        //     * 步骤2：保存对StudentListView的更改(通过保存studentList)
        //     */
        //    string[] lines = Editor.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        //    ObservableCollection<Student> newStudentList = new ObservableCollection<Student>();
        //    for (int i = 0; i < lines.Length; i++)//将在Editor中做的更改用于刷新StudentListView
        //    {
        //        if (lines[i].Length > 0)
        //        {
        //            if (i < StudentManager.StudentList.Count)
        //            {
        //                var item = StudentListView.Items[i] as Student;
        //                var student = new Student { Id = i + 1, Name = lines[i], PhotoPath = item.PhotoPath };//Id表示学号，从1开始
        //                newStudentList.Add(student);
        //            }
        //            else
        //            {
        //                var student = new Student { Id = i + 1, Name = lines[i], PhotoPath = "ms-appx:///Assets/RSSN_Logos/StoreLogo.scale-400.png" };//Id表示学号，从1开始
        //                newStudentList.Add(student);
        //            }
        //        }
        //        else//空行
        //        {
        //            var student = new Student { Id = i + 1, Name = "未知", PhotoPath = "ms-appx:///Assets/RSSN_Logos/StoreLogo.scale-400.png" };//Id表示学号，从1开始
        //            newStudentList.Add(student);
        //        }
        //    }//到此时，newStudentList已经包含了所有学生
        //    StudentListView.ItemsSource = newStudentList;//手动刷新StudentListView
        //    StudentManager.StudentList.Clear();//清空studentList
        //    for (int i = 0; i < newStudentList.Count; i++)//以下为保存StudentListView
        //    {
        //        var _item = StudentListView.Items[i] as Student;
        //        if (_item.PhotoPath != null)
        //        {
        //            var student = new Student { Id = i + 1, Name = lines[i], PhotoPath = _item.PhotoPath };//Id表示学号，从1开始
        //            StudentManager.StudentList.Add(student);
        //        }
        //        else//未更改照片，使用的是默认照片
        //        {
        //            var student = new Student { Id = i + 1, Name = lines[i], PhotoPath = "ms-appx:///Assets/RSSN_Logos/StoreLogo.scale-400.png" };//Id表示学号，从1开始
        //            StudentManager.StudentList.Add(student);
        //        }
        //    }
        //    await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
        //    GC.Collect();
        //}
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

        private void ClassNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["ClassName"] = ClassNameTextBox.Text;
            MainPage mainPage = (Window.Current.Content as Frame).Content as MainPage;
            if (mainPage != null)
            {
                // 传递班级名称
                mainPage.TriggerUpdateTextEvent(ClassNameTextBox.Text);
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
                    ClassNameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(file.Name);
                    localSettings.Values["Names"] = Editor.Text;

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
        }
        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            // 创建文件选取器
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("文本文档", new List<string>() { ".txt" });
            string filename = ClassNameTextBox.Text;
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
                    StudentListView.ItemsSource = null;
                    StudentListView.ItemsSource = StudentManager.StudentList;
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
            if (StudentListView.SelectedItem != null)
            {
                CurrentSelectionTextBlock.Text = "当前选择项:"+(StudentListView.SelectedIndex+1).ToString();
            }
            else
            {
                CurrentSelectionTextBlock.Text = "当前选择项:无";
            }
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
                        catch(Exception)
                        {
                            PopupNotice popupNotice = new PopupNotice("填充失败");
                            popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                            popupNotice.ShowPopup();
                        }
                    }
                    await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
                    StudentListView.ItemsSource = null;
                    StudentListView.ItemsSource = StudentManager.StudentList;
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请先选择要开始填充的项");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
            }

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
                await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
                StudentListView.ItemsSource = null;
                StudentListView.ItemsSource = StudentManager.StudentList;
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
            var focusedElement = FocusManager.GetFocusedElement() as Control;
            focusedElement.Focus(FocusState.Programmatic);
            //focusedElement.SetValue(FocusStateProperty, FocusState.Unfocused);
            StudentListView.SelectedItem = null;
            await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
            //ObservableCollection<Student> newStudentList = new ObservableCollection<Student>();
            //StudentListView.ItemsSource = newStudentList;//手动刷新StudentListView
            //StudentManager.StudentList.Clear();//清空studentList
            //for (int i = 0; i < newStudentList.Count; i++)//以下为保存StudentListView
            //{
            //    var _item = StudentListView.Items[i] as Student;
            //    if (_item.PhotoPath != null)
            //    {
            //        var student = new Student { Id = i + 1, Name = lines[i], PhotoPath = _item.PhotoPath };//Id表示学号，从1开始
            //        StudentManager.StudentList.Add(student);
            //    }
            //    else//未更改照片，使用的是默认照片
            //    {
            //        var student = new Student { Id = i + 1, Name = lines[i], PhotoPath = "ms-appx:///Assets/RSSN_Logos/StoreLogo.scale-400.png" };//Id表示学号，从1开始
            //        StudentManager.StudentList.Add(student);
            //    }
            //}
            //await StudentManager.SaveStudentsAsync(StudentManager.StudentList);
            //StudentListView.ItemsSource = null;
            //StudentListView.ItemsSource = StudentManager.StudentList;
        }

        private void StudentListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            UpdateStudentId();
            //var item = StudentListView.ContainerFromIndex(1) as ListViewItem;
            //var idTextBlock = item.FindName("IdTextBlock") as TextBlock;//找不到
            //var child = VisualTreeHelper.GetChild(item, 0) as ContentPresenter;
            //var child1 = child;
            //var child = VisualTreeHelper.GetChild(item, 0) /*as Grid*/;
            //var child1 = VisualTreeHelper.GetChild(child, 0) as Border;
            //var child2 = child1.Child as Grid;
            //var child2 = VisualTreeHelper.GetChild(child1, 0) as Grid/* as Grid*/;
            //var child3 = VisualTreeHelper.GetChild(child2, 0) as StackPanel;
            //var idTextBlock = child3.FindName("IdTextBlock") as TextBlock;
            //idTextBlock.Text = "1000";

        }
        private void SaveStudents()
        {
            StudentManager.StudentList.Clear();
            foreach (var item in StudentListView.Items)
            {

            }
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
            }
            UpdateStudentId();
        }

        private void FileEditSegmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(FileEditSegmented.SelectedIndex == 0)//文件
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
    }

}

