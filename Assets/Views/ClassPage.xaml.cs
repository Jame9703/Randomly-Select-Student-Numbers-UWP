using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Classes;

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClassPage : Page
    {
        public static ClassPage Current;
        public List<string> names = new List<string>();
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        public delegate void TextChangedEventHandler(string Text);
        ObservableCollection<Student> studentList = new ObservableCollection<Student>();
        GridView PhotosGridView = new GridView();
        StudentManager studentManager = new StudentManager();

        public ClassPage()
        {
            this.InitializeComponent();
            //dbHelper = new DatabaseHelper();
            //LoadStudents();
            Current = this;
            // 初始化行号
            UpdateLineNumbers();
            if (localSettings.Values["Names"] != null) Editor.Text = (string)localSettings.Values["Names"];
            if (localSettings.Values["ClassName"] != null) ClassNameTextBox.Text = (string)localSettings.Values["ClassName"];
            if (localSettings.Values["PhotosLocation"] != null)
            {
                PhotosLocationTextBlock.Text = (string)localSettings.Values["PhotosLocation"];
            }
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //读取学生信息
            studentList = await studentManager.LoadStudentsAsync();
            PhotosGridView.ItemsSource = studentList;
            PhotosGrid.Children.Add(PhotosGridView);
            PhotosGridView.ItemTemplate = (DataTemplate)Resources["GridViewItemTemplate"];
            PhotosGridView.Style = (Style)Resources["GridViewStyle"];
            PhotosGridView.CanReorderItems = true;
            //PhotosGridView.CanDragItems = true;
            //PhotosGridView.IsItemClickEnabled = true;
            PhotosGridView.AllowDrop = true;
            PhotosGridView.DragOver += PhotosGridView_DragOver;
            PhotosGridView.Drop += PhotosGridView_Drop;
            PhotosGridView.ItemsSource = studentList;
            PhotosLocationTextBlock.Text = localFolder.Path;
            localSettings.Values["PhotosLocation"] = localFolder.Path;

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            GC.Collect();
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

        private void ClassNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["ClassName"] = ClassNameTextBox.Text;
            //MainPage mainPage = new MainPage();
            //mainPage.ClassNameHyperlinkButton.Content = localSettings.Values[ClassNameKey];
            //if (TextChanged != null)
            //{
            //    TextChanged(ClassNameTextBox.Text);
            //}
            //else
            //{
            //    TextChanged("我的班级");
            //}

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
            string text = Editor.Text;
            names = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLineNumbers();
        }

        private async void FolderPickerButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add(".jpg");
            folderPicker.FileTypeFilter.Add(".jpeg");
            folderPicker.FileTypeFilter.Add(".png");
            folderPicker.FileTypeFilter.Add(".bmp");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                //GridView PhotosGridView = new GridView();

                PhotosGrid.Children.Clear();

                studentList.Clear();
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                int studentListCount = studentList.Count;//提前记录循环前studentList的长度
                for (int i = 0; i < files.Count; i++)
                {
                    var localFile = await localFolder.CreateFileAsync(files[i].Name, CreationCollisionOption.ReplaceExisting);
                    await files[i].CopyAndReplaceAsync(localFile);
                    StorageFile photoFile = await localFolder.GetFileAsync(files[i].Name);
                    var item = new Student { Id = studentListCount+i+1, PhotoPath = photoFile.Path, Name = files[i].Name };//Id表示学号，从1开始
                    studentList.Add(item);
                }
                await studentManager.SaveStudentsAsync(studentList);
                //GridView PhotosGridView = new();
                PhotosGrid.Children.Clear();
                PhotosGrid.Children.Add(PhotosGridView);
                PhotosGridView.ItemTemplate = (DataTemplate)Resources["GridViewItemTemplate"];
                PhotosGridView.Style = (Style)Resources["GridViewStyle"];
                //PhotosGridView.CanReorderItems = true;
                //PhotosGridView.CanDragItems = true;
                //PhotosGridView.IsItemClickEnabled = true;
                PhotosGridView.AllowDrop = true;
                PhotosGridView.DragOver += PhotosGridView_DragOver;
                PhotosGridView.Drop += PhotosGridView_Drop;
                PhotosGridView.ItemsSource = studentList;
                PhotosLocationTextBlock.Text = folder.Path;
                localSettings.Values["PhotosLocation"] = folder.Path;
            }
        }

        private async void PhotosGridView_Drop(object sender, DragEventArgs e)
        {
            if(e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Any())
                { 
                    for(int i= 0; i < items.Count; i++)//支持同时拖入多个文件
                    {
                        var storageFile = items[i] as StorageFile;
                        var contentType = storageFile.ContentType;
                        StorageFolder folder = ApplicationData.Current.LocalFolder;
                        BitmapImage bitmapImage = new BitmapImage();
                        using (var stream = await storageFile.OpenAsync(FileAccessMode.Read))
                        {
                            bitmapImage.SetSource(stream);
                        }
                        if (contentType == "image/jpg"|| contentType == "image/jpeg" || contentType == "image/png" || contentType == "image/bmp")
                        {
                            StorageFile newFile = await storageFile.CopyAsync(folder, storageFile.Name, NameCollisionOption.ReplaceExisting);
                            var item = new Student { PhotoPath = items[i].Path, Name = storageFile.Name };
                            studentList.Add(item);
                            PhotosGridView.ItemsSource = studentList;
                        }
                    }
                }
            }
        }

        private void PhotosGridView_DragOver(object sender, DragEventArgs e)
        {
            var PhotosGridView = sender as GridView;
            PhotosGridView.CanDragItems = true;
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "拖放此处即可添加文件 o(^▽^)o";
        }
    }

}

