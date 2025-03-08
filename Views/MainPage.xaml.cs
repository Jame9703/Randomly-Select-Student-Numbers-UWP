using System;
using System.Threading.Tasks;
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
using 随机抽取学号.Classes;
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
        public delegate void UpdateTextEventHandler(string text);// 定义委托
        public event UpdateTextEventHandler UpdateTextBoxEvent;// 定义事件
        public static StackPanel PopupContainerInstance;
        public void TriggerUpdateTextEvent(string text)
        {
            UpdateTextBoxEvent?.Invoke(text);
        }

        public MainPage()
        {
            this.InitializeComponent();

        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //在MainPage初始化时加载学生信息，确保不重复加载
            StudentManager.StudentList = await StudentManager.LoadStudentsAsync();
            StudentManager.checkedCheckBoxes = await StudentManager.LoadCheckedStudentsAsync();
            //LoadBackground();
            //PopupContainerInstance = PopupContainer;
            UpdateTextBoxEvent += OnUpdateTextBox; // 订阅事件
            // 隐藏系统标题栏并设置新的标题栏
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            ////设置标题栏边距
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
            if (localSettings.Values["Theme"] != null)
            {
                if ((int)localSettings.Values["Theme"] == 0)
                {
                    view.TitleBar.ButtonForegroundColor = ((Color)Application.Current.Resources["SystemAccentColor"]);
                }
                else if ((int)localSettings.Values["Theme"] == 1)
                {
                    view.TitleBar.ButtonForegroundColor = Colors.White;
                }
               (Window.Current.Content as Frame).RequestedTheme = (int)localSettings.Values["Theme"] switch
               {
                        0 => ElementTheme.Light,
                        1 => ElementTheme.Dark,
                        _ => ElementTheme.Default
               };
            }
            else
            {
                //找不到值，第一次启动，显示欢迎界面
                ContentDialog dialog = new ContentDialog();
                dialog.Title = AddText(24, "欢迎使用随机抽取学号");
                dialog.Content = AddText(12, "随机抽取学号是一款使用C#编写，基于Random()伪随机数生成器，免费￥、开源的通用Windows平台应用程序(UWP)。\r\n \r\n随机抽取学号不会将您的隐私数据发送到服务器(姓名，性别，照片等)，同时应用数据会在卸载时自动删除，以下为随机抽取学号的开源许可:\r\n\r\nMIT License\r\n\r\nCopyright (c) 2022-2024 Randomly Select Student Numbers\r\n\r\nPermission is hereby granted, free of charge, to any person obtaining a copy\r\nof this software and associated documentation files (the \"Software\"), to deal\r\nin the Software without restriction, including without limitation the rights\r\nto use, copy, modify, merge, publish, distribute, sublicense, and/or sell\r\ncopies of the Software, and to permit persons to whom the Software is\r\nfurnished to do so, subject to the following conditions:\r\n\r\nThe above copyright notice and this permission notice shall be included in all\r\ncopies or substantial portions of the Software.\r\n\r\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");
                dialog.PrimaryButtonText = "同意并继续";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.PrimaryButtonClick += (sender, args) => dialog.Hide();
                TextBlock AddText(int newFontSize, string newText)
                {
                    TextBlock textBlock = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                        Text = newText,
                        FontFamily = (FontFamily)Application.Current.Resources["HarmonyOSSans"],
                        FontSize = newFontSize,
                    };

                    return textBlock;
                }
                _ = dialog.ShowAsync();
                localSettings.Values["Theme"] = 2;//确保下次打开不显示欢迎界面
            }
            await Task.Delay(250);
            ContentGrid.Visibility = Visibility.Visible;
            LoadProgressRing.Visibility = Visibility.Collapsed;
        }
        // 用于存储背景笔刷的公共属性
        public Brush MainPageBackground
        {
            get { return (Brush)GetValue(MainPageBackgroundProperty); }
            set { SetValue(MainPageBackgroundProperty, value); }
        }

        // 注册依赖属性
        public static readonly DependencyProperty MainPageBackgroundProperty =
            DependencyProperty.Register("MainPageBackground", typeof(Brush), typeof(MainPage), new PropertyMetadata(null, OnMainPageBackgroundChanged));

        // 依赖属性更改时的回调方法
        private static void OnMainPageBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainPage = d as MainPage;
            if (mainPage != null)
            {
                // 当属性值更改时，更新MainPage的背景
                mainPage.Background = e.NewValue as Brush;
            }
        }

        private void LoadBackground()
        {
            // 创建线性渐变画刷
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Windows.Foundation.Point(0.2, 0);
            gradientBrush.EndPoint = new Windows.Foundation.Point(0.5, 1);
            // 创建透明度动画
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 0;
            opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(2.5));
            opacityAnimation.EnableDependentAnimation = true;
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame.ActualTheme == ElementTheme.Dark)
            {
                // 当前是深色主题
                opacityAnimation.To = 0.5;
                GradientStop stop1 = new GradientStop();
                stop1.Color = Color.FromArgb(255, 0, 0, 139); // 深蓝色
                stop1.Offset = 1;
                gradientBrush.GradientStops.Add(stop1);

                GradientStop stop2 = new GradientStop();
                stop2.Color = Color.FromArgb(255, 139, 0, 139); // 深紫色
                stop2.Offset = 0;
                gradientBrush.GradientStops.Add(stop2);
            }
            else if (rootFrame.ActualTheme == ElementTheme.Light)
            {
                // 当前是浅色主题
                opacityAnimation.To = 0.6;
                // 添加渐变停止点：浅绿色
                GradientStop lightGreenStop = new GradientStop();
                lightGreenStop.Color = Color.FromArgb(255, 144, 238, 144);
                lightGreenStop.Offset = 0;
                gradientBrush.GradientStops.Add(lightGreenStop);

                // 添加渐变停止点：浅蓝绿色
                GradientStop lightBlueGreenStop = new GradientStop();
                lightBlueGreenStop.Color = Color.FromArgb(255, 173, 216, 230);
                lightBlueGreenStop.Offset = 0.5;
                gradientBrush.GradientStops.Add(lightBlueGreenStop);

                // 添加渐变停止点：浅蓝色
                GradientStop lightBlueStop = new GradientStop();
                lightBlueStop.Color = Color.FromArgb(255, 135, 206, 250);
                lightBlueStop.Offset = 1;
                gradientBrush.GradientStops.Add(lightBlueStop);
            }
            // 初始设置背景透明度为 0
            gradientBrush.Opacity = 0;
            this.Background = gradientBrush;



            // 创建故事板
            Storyboard storyboard = new Storyboard();
            Storyboard.SetTarget(opacityAnimation, gradientBrush);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            storyboard.Children.Add(opacityAnimation);

            // 开始动画
            storyboard.Begin();
        }

        private void OnUpdateTextBox(string text)
        {
            ClassNameHyperlinkButton.Content = text;
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

