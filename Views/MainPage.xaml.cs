using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Classes;
using 随机抽取学号.Media;
using 随机抽取学号.Views;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace 随机抽取学号
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        private ToggleButton _lastSelectedButton;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static StackPanel PopupContainerInstance;
        private Compositor _compositor;
        private Visual _oldPageVisual;
        private Visual _newPageVisual;
        private bool isFirstNavigate = true;
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 隐藏系统标题栏并设置新的标题栏
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(AppTitleBar);
            //将标题栏右上角的3个按钮改为透明
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            ContentFrame.Navigate(typeof(HomePage));
            HomePageButton.IsChecked = true;
            this.SizeChanged += MainPage_SizeChanged;
            _lastSelectedButton = HomePageButton;//确保开始时_lastSelectedButton不为null
            //加载应用设置
            LoadSettings();
            if (SettingsHelper.IsFirstRun == true)//判断是否是第一次运行
            {
                //第一次运行，显示欢迎界面
                WelcomeContentDialog welcomeDialog = new WelcomeContentDialog();
                await welcomeDialog.ShowAsync();
                SettingsHelper.IsFirstRun = false;
            }
            //确保数据库正常加载
            await StudentManager.InitializeDatabase();
            //在MainPage初始化时加载学生信息，确保不重复加载
            StudentManager.StudentList = await StudentManager.LoadStudentsAsync();
            StudentManager.SelectedRanges = await StudentManager.LoadCheckedStudentsAsync();
            //加载所有班级信息
            StudentManager.ClassList = await StudentManager.LoadClassesAsync();
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //StorageFolder imageFolder = await localFolder.CreateFolderAsync("ClassPictures", CreationCollisionOption.OpenIfExists);
            //IStorageItem item = await imageFolder.TryGetItemAsync("ClassEmblem.png");
            ContentGrid.Visibility = Visibility.Visible;
            LoadProgressRing.Visibility = Visibility.Collapsed;
        }
        private void LoadSettings()
        {
            //设置应用主题
            (Window.Current.Content as Frame).RequestedTheme = SettingsHelper.Theme switch
            {
                0 => ElementTheme.Light,
                1 => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
            //获取当前视图并设置标题栏为透明
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            //设置系统标题栏按钮颜色(最小化、最大化、关闭)
            if (SettingsHelper.Theme == 0)
            {
                view.TitleBar.ButtonForegroundColor = ((Color)Application.Current.Resources["SystemAccentColor"]);
            }
            else if (SettingsHelper.Theme == 1)
            {
                view.TitleBar.ButtonForegroundColor = Colors.White;
            }
            //设置MainPage背景
            if (SettingsHelper.MainPageBackground == 0)
            {
                this.Background = new SolidColorBrush
                {
                    Color = Colors.White,
                    Opacity = SettingsHelper.MainPageBackgroundOpacity
                };
            }
            else if (SettingsHelper.MainPageBackground == 1)
            {
                this.Background = new AcrylicBrush
                {
                    BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                    TintOpacity = 0.6,
                    TintColor = Colors.Transparent,
                    Opacity = SettingsHelper.MainPageBackgroundOpacity
                };
            }
            else if (SettingsHelper.MainPageBackground == 2)
            {
                this.Background = new BackdropMicaBrush
                {
                    BackgroundSource = BackgroundSource.WallpaperBackdrop,
                    Opacity = SettingsHelper.MainPageBackgroundOpacity
                };
            }
            //设置ContentFrame背景
            if (SettingsHelper.ContentFrameBackground == 0)
            {
                ContentFrame.Background = new SolidColorBrush
                {
                    Color = Colors.White,
                    Opacity = SettingsHelper.ContentFrameBackgroundOpacity
                };
            }
            else if (SettingsHelper.ContentFrameBackground == 1)
            {
                ContentFrame.Background = new AcrylicBrush
                {
                    BackgroundSource = AcrylicBackgroundSource.Backdrop,
                    TintOpacity = 0.6,
                    TintColor = Colors.Transparent,
                    Opacity = SettingsHelper.ContentFrameBackgroundOpacity
                };
            }
            else if (SettingsHelper.ContentFrameBackground == 2)
            {
                ContentFrame.Background = new BackdropMicaBrush
                {
                    BackgroundSource = BackgroundSource.Backdrop,
                    Opacity = SettingsHelper.ContentFrameBackgroundOpacity
                };
            }
        }
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("页面加载失败:" + e.SourcePageType.FullName);
        }
        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (_lastSelectedButton != button)
            {
                StartAnimation(button);
                ContentFrame.Navigate(typeof(HomePage));
            }
            else
            {
                button.IsChecked = true;//上次点击的按钮和本次一样，保持选中状态
            }
            HomeIcon.Glyph = "\uEA8A";
        }
        private void ClassPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (_lastSelectedButton != button)
            {
                StartAnimation(button);
                ContentFrame.Navigate(typeof(ClassPage));
            }
            else
            {
                button.IsChecked = true;//上次点击的按钮和本次一样，保持选中状态
            }
            ClassIcon.Glyph = "\uEA8C";
        }
        private void NumbersPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (_lastSelectedButton != button)
            {
                StartAnimation(button);
                ContentFrame.Navigate(typeof(NumbersPage), null);
            }
            else
            {
                button.IsChecked = true;//上次点击的按钮和本次一样，保持选中状态
            }
            NumbersIcon.Visibility = Visibility.Collapsed;
            NumbersIconChecked.Visibility = Visibility.Visible;
        }
        private void CharactersPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (_lastSelectedButton != button)
            {
                StartAnimation(button);
                ContentFrame.Navigate(typeof(CharactersPage));
            }
            else
            {
                button.IsChecked = true;//上次点击的按钮和本次一样，保持选中状态
            }
        }
        private void HelpPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (_lastSelectedButton != button)
            {
                StartAnimation(button);
                ContentFrame.Navigate(typeof(HelpPage));
            }
            else
            {
                button.IsChecked = true;//上次点击的按钮和本次一样，保持选中状态
            }
        }
        private void MorePage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (_lastSelectedButton != button)
            {
                StartAnimation(button);
                ContentFrame.Navigate(typeof(MorePage));
            }
            else
            {
                button.IsChecked = true;//上次点击的按钮和本次一样，保持选中状态
            }
        }
        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (_lastSelectedButton != button)
            {
                StartAnimation(button);
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                button.IsChecked = true;//上次点击的按钮和本次一样，保持选中状态
            }
            SettingsIcon.Glyph = "\uF8B0";
        }

        private void StartAnimation(ToggleButton button)
        {
            if (button != null)
            {
                if (_lastSelectedButton != null)
                {
                    // 如果之前有选中的按钮，取消其选中状态
                    _lastSelectedButton.IsChecked = false;
                    ((StackPanel)_lastSelectedButton.Content).Children.OfType<TextBlock>().First().Visibility = Visibility.Visible;
                    ((StackPanel)button.Content).Children.OfType<TextBlock>().First().Visibility = Visibility.Collapsed;
                }

                button.IsChecked = true; // 设置当前按钮为选中状态
                var targetPosition = button.TransformToVisual(Canvas).TransformPoint(new Point(button.ActualWidth / 2, (button.ActualHeight) / 2));
                // 设置Storyboard的动画值
                if (doubleAnimation != null)
                {
                    doubleAnimation.To = targetPosition.Y - MovingRectangle.Height;
                }
                MovingRectangle.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Collapsed;
                // 开始Storyboard动画
                MoveRectangleStoryboard.Begin();

                _lastSelectedButton = button; // 更新最后选中的按钮
                HomeIcon.Glyph = "\uE80F";
                ClassIcon.Glyph = "\uE77B";
                NumbersIcon.Visibility = Visibility.Visible;
                NumbersIconChecked.Visibility = Visibility.Collapsed;
                SettingsIcon.Glyph = "\uE713";
            }
        }
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SettingsPageButton.IsChecked == true)
            {
                MovingRectangle.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Visible;
            }
        }

        private void ManageStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lastSelectedButton != ClassPageButton)
            {
                StartAnimation(ClassPageButton);
                ContentFrame.Navigate(typeof(ClassPage));
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("当前已在班级页面");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
            ClassIcon.Glyph = "\uEA8C";
        }

        private async void EditPictureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 创建文件选择器
                FileOpenPicker openPicker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail, // 缩略图视图
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary // 起始位置为图片库
                };

                // 添加支持的图片格式
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".bmp");
                openPicker.FileTypeFilter.Add(".gif");

                // 打开选择器并获取文件
                StorageFile file = await openPicker.PickSingleFileAsync();

                if (file != null)
                {
                    // 将文件转换为可显示的图像
                    using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(fileStream);
                        SmallClassPicture.ProfilePicture = bitmapImage;
                        BigClassPicture.ProfilePicture = bitmapImage;
                        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                        StorageFolder imagesFolder = await localFolder.GetFolderAsync("ClassPictures");
                        if (imagesFolder != null)
                        {
                            StorageFile copyFile = await file.CopyAsync(imagesFolder, "ClassEmblem.png", NameCollisionOption.ReplaceExisting);
                        }
                    }
                }
                else
                {
                    // 用户取消选择
                }
            }
            catch (Exception ex)
            {
                // 处理异常（如权限问题）
                PopupNotice popupNotice = new PopupNotice("打开失败" + ex.Message);
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
        }

        private void CurrentClassNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            localSettings.Values["CurrentClassName"] = CurrentClassNameTextBox.Text;
        }

        private async void SwitchClassButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchClassContentDialog switchClassContentDialog = new SwitchClassContentDialog();
            await switchClassContentDialog.ShowAsync();
        }

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (ContentFrame.Content != null)
            {
                _oldPageVisual = ElementCompositionPreview.GetElementVisual(ContentFrame.Content as Page);

                // 旧页面的透明度动画
                ScalarKeyFrameAnimation oldPageOpacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
                oldPageOpacityAnimation.Duration = TimeSpan.FromSeconds(0.2);
                oldPageOpacityAnimation.InsertKeyFrame(1f, 0f);
                _oldPageVisual.StartAnimation("Opacity", oldPageOpacityAnimation);

                // 旧页面的位移动画
                Vector3KeyFrameAnimation oldPageOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
                oldPageOffsetAnimation.Duration = TimeSpan.FromSeconds(0.5);
                oldPageOffsetAnimation.InsertKeyFrame(0f, new System.Numerics.Vector3(0, 0, 0));
                oldPageOffsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3(0, -100, 0));
                _oldPageVisual.StartAnimation("Offset", oldPageOffsetAnimation);
                isFirstNavigate = false;
            }
            else
            {
                isFirstNavigate = true;
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content != null && isFirstNavigate == false)
            {
                _newPageVisual = ElementCompositionPreview.GetElementVisual((UIElement)e.Content);
                _newPageVisual.Opacity = 0f;
                _newPageVisual.Offset = new System.Numerics.Vector3(0, 100, 0);

                // 新页面的透明度动画
                ScalarKeyFrameAnimation newPageOpacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
                newPageOpacityAnimation.Duration = TimeSpan.FromSeconds(1.5);
                newPageOpacityAnimation.InsertKeyFrame(1f, 1f);
                _newPageVisual.StartAnimation("Opacity", newPageOpacityAnimation);

                // 新页面的位移动画
                Vector3KeyFrameAnimation newPageOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
                newPageOffsetAnimation.Duration = TimeSpan.FromSeconds(0.8);
                newPageOffsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3(0, 0, 0));
                _newPageVisual.StartAnimation("Offset", newPageOffsetAnimation);
            }
            else
            {
                _compositor = ElementCompositionPreview.GetElementVisual(ContentFrame).Compositor;
                _newPageVisual = ElementCompositionPreview.GetElementVisual((UIElement)e.Content);
                _newPageVisual.Opacity = 0f;
                _newPageVisual.Offset = new System.Numerics.Vector3(0, 500, 0);

                // 新页面的透明度动画
                ScalarKeyFrameAnimation newPageOpacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
                newPageOpacityAnimation.Duration = TimeSpan.FromSeconds(2);
                newPageOpacityAnimation.InsertKeyFrame(1f, 1f);
                _newPageVisual.StartAnimation("Opacity", newPageOpacityAnimation);

                // 新页面的位移动画
                Vector3KeyFrameAnimation newPageOffsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
                newPageOffsetAnimation.Duration = TimeSpan.FromSeconds(1.2);
                newPageOffsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3(0, 0, 0));
                _newPageVisual.StartAnimation("Offset", newPageOffsetAnimation);
            }
        }
    }
}

