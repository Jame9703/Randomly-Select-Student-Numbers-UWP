using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using 随机抽取学号.Classes;
using 随机抽取学号.Media;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainPage mainPage = ((Frame)Window.Current.Content).Content as MainPage;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public SettingsPage()
        {
            this.InitializeComponent();
            //将设置项加载到控件
            AppearanceRadioButtons.SelectedIndex = SettingsHelper.Theme;
            BackgroundRadioButtons.SelectedIndex = SettingsHelper.MainPageBackground;
            NoBackgroundOpacitySlider.Value = SettingsHelper.MainPageNoBackgroundOpacity;
            PageBackgroundRadioButtons.SelectedIndex = SettingsHelper.ContentFrameBackground;
            PageBackgroundOpacitySlider.Value = SettingsHelper.ContentFrameBackgroundOpacity;
            NoReturnToggleSwitch.IsOn = SettingsHelper.NoReturn;
            AutoStopToggleSwitch.IsOn = SettingsHelper.AutoStop;
            OptimizeToggleSwitch.IsOn = SettingsHelper.Optimize;
            SaveRangeToggleSwitch.IsOn = SettingsHelper.SaveRange;
            SaveHistoryToggleSwitch.IsOn = SettingsHelper.SaveHistory;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            GC.Collect();
        }

        private void AppearanceRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //SelectedIndex   Theme
            //         0                Light
            //         1                Dark
            //         2                Default(Use system settings)
            SettingsHelper.Theme = AppearanceRadioButtons.SelectedIndex;
            (Window.Current.Content as Frame).RequestedTheme = SettingsHelper.Theme switch
            {
                0 => ElementTheme.Light,
                1 => ElementTheme.Dark,
                _ => ElementTheme.Default
            };

            var view = ApplicationView.GetForCurrentView();
            if (SettingsHelper.Theme == 0)
            {
                view.TitleBar.ButtonForegroundColor = ((Color)Application.Current.Resources["SystemAccentColor"]);
            }
            else if (SettingsHelper.Theme == 1)
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
            if (mainPage != null)
            {
                SettingsHelper.MainPageBackground = BackgroundRadioButtons.SelectedIndex;
                switch (SettingsHelper.MainPageBackground)
                {
                    case 0://无背景
                        mainPage.Background = new SolidColorBrush()
                        {
                            Color = (Color)Application.Current.Resources["SystemAltMediumColor"],
                            Opacity = SettingsHelper.MainPageNoBackgroundOpacity
                        };
                        break;
                    case 1://亚克力背景
                        mainPage.Background = new AcrylicBrush
                        {
                            BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                            Opacity = SettingsHelper.MainPageAcrylicBackgroundOpacity,
                            TintOpacity = 0.6,
                            TintColor = Colors.Transparent
                        };
                        break;
                    case 2://云母背景
                        mainPage.Background = new BackdropMicaBrush
                        {
                            BackgroundSource = BackgroundSource.WallpaperBackdrop,
                            Opacity = SettingsHelper.MainPageMicaBackgroundOpacity
                        };
                        break;
                    case 3://图片背景
                        mainPage.Background = new ImageBrush
                        {
                            Opacity = SettingsHelper.MainPageImageBackgroundOpacity
                        };
                        break;
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("更改应用背景失败");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
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
                PopupNotice popupNotice = new PopupNotice("打开失败");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
        }

        private void NoBackgroundOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SettingsHelper.MainPageNoBackgroundOpacity = NoBackgroundOpacitySlider.Value;
            if (mainPage != null)
            {
                mainPage.Background.Opacity = SettingsHelper.MainPageNoBackgroundOpacity;
            }
        }
        #region ToggledEvents
        private void NoReturnToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["NoReturn"] = NoReturnToggleSwitch.IsOn;
        }

        private void AutoStopToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["AutoStop"] = AutoStopToggleSwitch.IsOn;
        }

        private void OptimizeToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["Optimize"] = OptimizeToggleSwitch.IsOn;
        }

        private void SaveRangeToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SaveRange"] = SaveRangeToggleSwitch.IsOn;
        }

        private void SaveHistoryToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            localSettings.Values["SaveHistory"] = SaveHistoryToggleSwitch.IsOn;
        }
        #endregion

        private void PageBackgroundOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SettingsHelper.ContentFrameBackgroundOpacity = PageBackgroundOpacitySlider.Value;
            if (mainPage != null)
            {
                mainPage.ContentFrame.Background.Opacity = SettingsHelper.ContentFrameBackgroundOpacity;
            }
        }

        private void PageBackgroundRadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainPage != null)
            {
                SettingsHelper.ContentFrameBackground = PageBackgroundRadioButtons.SelectedIndex;
                switch (SettingsHelper.ContentFrameBackground)
                {
                    case 0:// 无背景
                        mainPage.ContentFrame.Background = new SolidColorBrush()
                        {
                            Color = Colors.White,
                            Opacity = SettingsHelper.ContentFrameBackgroundOpacity
                        };
                        break;
                    case 1:// 亚克力背景
                        mainPage.ContentFrame.Background = new AcrylicBrush
                        {
                            BackgroundSource = AcrylicBackgroundSource.Backdrop,
                            Opacity = SettingsHelper.ContentFrameBackgroundOpacity,
                            TintOpacity = 0.6,
                            TintColor = Colors.Transparent
                        };
                        break;
                    case 2:// 云母背景
                        mainPage.ContentFrame.Background = new BackdropMicaBrush
                        {
                            BackgroundSource = BackgroundSource.Backdrop,
                            Opacity = SettingsHelper.ContentFrameBackgroundOpacity,
                        };
                        break;
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("更改页面背景失败");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Error;
                popupNotice.ShowPopup();
            }
        }

        private void AcrylicBackgroundOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SettingsHelper.MainPageAcrylicBackgroundOpacity = AcrylicBackgroundOpacitySlider.Value;
            if (mainPage != null)
            {
                mainPage.Background.Opacity = SettingsHelper.MainPageAcrylicBackgroundOpacity;
            }
        }

        private void MicaBackgroundOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SettingsHelper.MainPageMicaBackgroundOpacity = MicaBackgroundOpacitySlider.Value;
            if (mainPage != null)
            {
                mainPage.Background.Opacity = SettingsHelper.MainPageMicaBackgroundOpacity;
            }
        }

        private void ImageBackgroundOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SettingsHelper.MainPageImageBackgroundOpacity = ImageBackgroundOpacitySlider.Value;
            if (mainPage != null)
            {
                mainPage.Background.Opacity = SettingsHelper.MainPageImageBackgroundOpacity;
            }
        }
    }
}
