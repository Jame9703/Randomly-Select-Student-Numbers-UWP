using CommunityToolkit.WinUI;
using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
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
        private AppBarToggleButton _lastSelectedButton;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static readonly string ClassNameKey = "ClassName";
        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            // 隐藏系统标题栏并设置新的标题栏
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            //设置标题栏边距
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateAppTitle(s);
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(AppTitleBar);
            //将标题栏右上角的3个按钮改为透明（显示Mica）
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;

            ContentFrame.Navigate(typeof(HomePage), null, new DrillInNavigationTransitionInfo());
            HomePageButton.IsChecked = true;
            this.SizeChanged += MainPage_SizeChanged;
            _lastSelectedButton = HomePageButton;//确保开始时_lastSelectedButton不为null
            string ClassName = localSettings.Values[ClassNameKey] as string;
            if (ClassName != null) ClassNameHyperlinkButton.Content = ClassName;
            ChangeOpacityRequested += MainPage_ChangeOpacityRequested;
            if (localSettings.Values["Theme"] !=null)
            {
                if ((int)localSettings.Values["Theme"] == 0)
                {
                    view.TitleBar.ButtonForegroundColor = ((Color)Application.Current.Resources["SystemAccentColor"]);
                }
                else if ((int)localSettings.Values["Theme"] == 1)
                {
                    view.TitleBar.ButtonForegroundColor = Colors.White;
                }
            }
            else
            {
                //找不到值，第一次启动，显示欢迎界面
                ContentDialog dialog = new ContentDialog();
                dialog.Title = AddText(24,"欢迎使用随机抽取学号");
                dialog.Content = AddText(12, "随机抽取学号是一款使用C#编写，基于Random()伪随机数生成器，免费￥、开源的通用Windows平台应用程序(UWP)。\r\n \r\n随机抽取学号不会将您的隐私数据发送到服务器(姓名，性别，照片等)，同时应用数据会在卸载时自动删除，以下为随机抽取学号的开源许可:\r\n\r\nMIT License\r\n\r\nCopyright (c) 2022-2024 Randomly Select Student Numbers\r\n\r\nPermission is hereby granted, free of charge, to any person obtaining a copy\r\nof this software and associated documentation files (the \"Software\"), to deal\r\nin the Software without restriction, including without limitation the rights\r\nto use, copy, modify, merge, publish, distribute, sublicense, and/or sell\r\ncopies of the Software, and to permit persons to whom the Software is\r\nfurnished to do so, subject to the following conditions:\r\n\r\nThe above copyright notice and this permission notice shall be included in all\r\ncopies or substantial portions of the Software.\r\n\r\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");
                dialog.PrimaryButtonText = "同意并继续";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.PrimaryButtonClick += (sender, args) => dialog.Hide();
                TextBlock AddText(int newFontSize,string newText)
                {
                    TextBlock textBlock = new TextBlock()
                    {
                        TextWrapping= TextWrapping.Wrap,
                        Text = newText,
                        FontFamily = (FontFamily)Application.Current.Resources["HarmonyOSSans"],
                        FontSize = newFontSize,
                    };

                    return textBlock;
                }
                _ = dialog.ShowAsync();
                localSettings.Values["Theme"] = 2;//确保下次打开不显示欢迎界面
            }

        }
        public event EventHandler<double> ChangeOpacityRequested;
        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            AppTitleBar.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);
        }
        public  void OnChangeOpacityRequested(double opacity)
        {
            ChangeOpacityRequested?.Invoke(this, opacity);
        }
        private void MainPage_ChangeOpacityRequested(object sender, double opacity)
        {
            // 设置 MainPage 的透明度
            MainGrid.Opacity = opacity;
        }
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("页面加载失败:" + e.SourcePageType.FullName);
        }
        private void HomePage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
                ContentFrame.Navigate(typeof(HomePage), null, new DrillInNavigationTransitionInfo());

        }
        private void ClassPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);

                ContentFrame.Navigate(typeof(ClassPage));

        }

        private void NumbersPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(NumbersPage));
        }
        private void CharactersPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(CharactersPage));



        }
        private void HelpPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(HelpPage));



        }
        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(SettingsPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });



        }

        private void StartAnimation(AppBarToggleButton button)
        {
            if (button != null)
            {
                if (_lastSelectedButton != null)
                {
                    // 如果之前有选中的按钮，取消其选中状态（如果需要）
                    _lastSelectedButton.IsChecked = false;
                }

                button.IsChecked = true; // 设置当前按钮为选中状态

                // 获取目标位置
                //GeneralTransform transform1 = _lastSelectedButton.TransformToVisual(null); // 转换为相对于根视觉树的坐标
                //Point point1 = transform1.TransformPoint(new Point(_lastSelectedButton.Margin.Left, _lastSelectedButton.Margin.Top));

                //GeneralTransform transform2 = button.TransformToVisual(null);
                //Point point2 = transform2.TransformPoint(new Point(button.Margin.Left, button.Margin.Top));
                var targetPosition = button.TransformToVisual(Canvas).TransformPoint(new Point(button.ActualWidth / 2, (button.ActualHeight + 40) / 2));

                //double a = point2.Y - point1.Y;
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
            }
        }
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (SettingsPageButton.IsChecked == true)
            {
                //StartAnimation(SettingsPageButton);//为解决当窗口大小变化，SettingsPageButton位置改变，但MovingRectangle位置未改变的问题

                //MovingRectangle.SetValue(Grid.RowProperty, 0);
                //double Height = SettingsPageButton.Margin.Top;
                //MovingRectangle.Margin = new Thickness(0, 5, 0, 0);
                MovingRectangle.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Visible;
            }
        }
        private void ClassNameHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(ClassPage));
            StartAnimation(ClassPageButton);

        }

    }
}

