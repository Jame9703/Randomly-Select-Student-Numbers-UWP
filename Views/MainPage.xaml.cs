using System;
using System.Linq;
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
            //确保数据库正常加载
            await StudentManager.InitializeDatabase();
            //在MainPage初始化时加载学生信息，确保不重复加载
            StudentManager.StudentList = await StudentManager.LoadStudentsAsync();
            StudentManager.SelectedRanges = await StudentManager.LoadCheckedStudentsAsync();
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
                if (localSettings.Values.ContainsKey("MainPageBackground"))
                {
                    if ((int)localSettings.Values["MainPageBackground"] == 0)
                    {
                        this.Background = new SolidColorBrush(Colors.White);
                    }
                    else if ((int)localSettings.Values["MainPageBackground"] == 1)
                    {
                        var acrylicBrush = new AcrylicBrush
                        {
                            BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
                            Opacity = 1,
                            TintOpacity = 0.6,
                            TintColor = Colors.Transparent
                        };
                        this.Background = acrylicBrush;
                    }
                    else if ((int)localSettings.Values["MainPageBackground"] == 2)
                    {
                        var backdropMicaBrush = new BackdropMicaBrush
                        {
                            BackgroundSource = BackgroundSource.WallpaperBackdrop,
                        };
                        this.Background = backdropMicaBrush;
                    }
                }
                else
                {
                    var backdropMicaBrush = new BackdropMicaBrush
                    {
                        BackgroundSource = BackgroundSource.WallpaperBackdrop,
                    };
                    this.Background = backdropMicaBrush;
                    localSettings.Values["MainPageBackground"] = 2;//设置默认MainPage背景
                }
            }
            else
            {
                //找不到值，第一次启动，显示欢迎界面
                WelcomeContentDialog welcomeDialog = new WelcomeContentDialog();
                await welcomeDialog.ShowAsync();
                localSettings.Values["Theme"] = 2;//确保下次打开不显示欢迎界面
                localSettings.Values["MainPageBackground"] = 2;//设置默认MainPage背景
            }
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
            ContentFrame.Navigate(typeof(ClassPage), null, new DrillInNavigationTransitionInfo());

        }

        private void NumbersPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(NumbersPage), null, new DrillInNavigationTransitionInfo());
        }
        private void CharactersPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(CharactersPage), null, new DrillInNavigationTransitionInfo());



        }
        private void HelpPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(HelpPage), null, new DrillInNavigationTransitionInfo());

        }
        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarToggleButton;
            StartAnimation(button);
            ContentFrame.Navigate(typeof(SettingsPage), null, new DrillInNavigationTransitionInfo());

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

