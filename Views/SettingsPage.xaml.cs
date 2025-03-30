using System;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Controls;
using 随机抽取学号.Media;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            AppearanceRadioButtons.SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values["Theme"];
            BackgroundRadioButtons.SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values["MainPageBackground"];
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
            var compositor = ElementCompositionPreview.GetElementVisual(ContentGrid).Compositor;
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0f, 1f);
            animation.InsertKeyFrame(1f, 0f);
            animation.Duration = TimeSpan.FromSeconds(1);
            var visual = ElementCompositionPreview.GetElementVisual(ContentGrid);
            visual.StartAnimation("Opacity", animation);
            GC.Collect();
        }
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private void AppearanceRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SelectedIndex Theme

            //         0           Light

            //         1           Dark

            //         2           Default(Use system settings)

            ApplicationData.Current.LocalSettings.Values["Theme"] = AppearanceRadioButtons.SelectedIndex;
            (Window.Current.Content as Frame).RequestedTheme = AppearanceRadioButtons.SelectedIndex switch
            {
                0 => ElementTheme.Light,
                1 => ElementTheme.Dark,
                _ => ElementTheme.Default
            };

            var view = ApplicationView.GetForCurrentView();
            if (AppearanceRadioButtons.SelectedIndex == 0)
            {
                view.TitleBar.ButtonForegroundColor = ((Color)Application.Current.Resources["SystemAccentColor"]);
            }
            else if (AppearanceRadioButtons.SelectedIndex == 1)
            {
                view.TitleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    view.TitleBar.ButtonForegroundColor = Colors.White;
                }
                else if (Application.Current.RequestedTheme == ApplicationTheme.Light)
                {
                    view.TitleBar.ButtonForegroundColor = ((Color)Application.Current.Resources["SystemAccentColor"]);
                }
            }
        }

        private void BackgroundRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mainPage = ((Frame)Window.Current.Content).Content as MainPage;
            if (mainPage != null)
            {
                if (BackgroundRadioButtons.SelectedIndex == 0)// 无背景
                {
                        // 创建一个新的纯色笔刷来设置MainPage的背景
                        var newBrush = new SolidColorBrush(Colors.White);
                        mainPage.MainPageBackground = newBrush;
                    localSettings.Values["MainPageBackground"] = 0;
                }
                else if (BackgroundRadioButtons.SelectedIndex == 1)// 亚克力背景
                {
                    var acrylicBrush = new AcrylicBrush
                    {
                        BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                        Opacity = 1,
                        TintOpacity = 0.6,
                        TintColor = Colors.Transparent
                    };
                    mainPage.MainPageBackground = acrylicBrush;
                    localSettings.Values["MainPageBackground"] = 1;
                }
                else if (BackgroundRadioButtons.SelectedIndex == 2)// 云母背景
                {
                    var backdropMicaBrush = new BackdropMicaBrush
                    {
                        BackgroundSource = BackgroundSource.WallpaperBackdrop,
                    };
                    mainPage.MainPageBackground = backdropMicaBrush;
                    localSettings.Values["MainPageBackground"] = 2;
                }
            }
            else
            {
                //PopupMessage.ShowPopupMessage("更改背景失败");
            }
        }

        private async void GoToGithubSettingsCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri("https://github.com/Jame9703/Randomly-Select-Student-Numbers-UWP"));
            }
            catch (Exception)
            {

            }
        }
        private void ExpanderContent_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // 阻止事件冒泡
            e.Handled = true;
        }
    }
}
