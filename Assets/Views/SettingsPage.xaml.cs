using System;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

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
            if(localSettings.Values["Theme"]!=null)
            {
                AppearanceRadioButtons.SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values["Theme"];
            }
            else
            {
                AppearanceRadioButtons.SelectedIndex = 2;
            }
            // 开始进入动画
            var enterPageStoryboard = this.Resources["EnterPageStoryboard"] as Storyboard;
            enterPageStoryboard.Begin();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
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

    }
}
