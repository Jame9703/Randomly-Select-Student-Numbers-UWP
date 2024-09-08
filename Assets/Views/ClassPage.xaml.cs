using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    public class ImageTextItem
    {
        public BitmapImage Photos { get; set; }
        public string Names { get; set; }
    }
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClassPage : Page
    {
        public static ClassPage Current;
        public List<string> names = new List<string>();
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public delegate void TextChangedEventHandler(string Text);
        List<ImageTextItem> imageTextItems = new List<ImageTextItem>();

        public ClassPage()
        {
            this.InitializeComponent();
            Current = this;
            // 初始化行号
            UpdateLineNumbers();
            LoadData();
            //lineNumberBorder.Background = Editor.Background;
            if (localSettings.Values["Names"] != null) Editor.Text = (string)localSettings.Values["Names"];
            if (localSettings.Values["ClassName"] != null) ClassNameTextBox.Text = (string)localSettings.Values["ClassName"];
            if (localSettings.Values["PhotosLocation"] != null)
            {
                PhotosLocationTextBlock.Text = (string)localSettings.Values["PhotosLocation"];
            }
        }
        private void LoadData()
        {

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
                    popupNotice.ShowAPopup();
                }
                catch (Exception)
                {
                    PopupNotice popupNotice = new PopupNotice("打开文件失败");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowAPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("打开操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowAPopup();
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
                    popupNotice.ShowAPopup();
                }
                else
                {
                    PopupNotice popupNotice = new PopupNotice(file.Name + " 保存失败");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                    popupNotice.ShowAPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("保存操作已取消");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowAPopup();
            }
        }
        public void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["Names"] = Editor.Text;
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
                GridView PhotosGridView = new GridView();
                PhotosGridView.ItemTemplate = (DataTemplate)Resources["GridViewItemTemplate"];
                PhotosGridView.Style = (Style)Resources["GridViewStyle"];
                PhotosGridView.CanReorderItems = true;
                PhotosGridView.CanDragItems = true;
                PhotosGridView.IsItemClickEnabled = true;
                PhotosGridView.AllowDrop = true;
                PhotosGrid.Children.Clear();
                PhotosGrid.Children.Add(PhotosGridView);
                imageTextItems.Clear();
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                foreach (StorageFile file in files)
                {
                    BitmapImage bitmapImage = new BitmapImage();


                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
                        //bitmapImage.DecodePixelWidth = (int)properties.Width;
                        //bitmapImage.DecodePixelHeight = (int)properties.Height;
                        await bitmapImage.SetSourceAsync(fileStream);
                    }

                    var item = new ImageTextItem { Photos = bitmapImage, Names = file.Name };
                    imageTextItems.Add(item);
                }

                PhotosGridView.ItemsSource = imageTextItems;
                PhotosLocationTextBlock.Text = folder.Path;
                localSettings.Values["PhotosLocation"] = folder.Path;
            }
        }
    }

}

