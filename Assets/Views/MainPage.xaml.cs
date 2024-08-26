using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        }
        public event EventHandler<double> ChangeOpacityRequested;
        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
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


        //private void NavView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // You can also add items in code.
        //    NavView.MenuItems.Add(new muxc.NavigationViewItemSeparator());
        //    //NavView.MenuItems.Add(new muxc.NavigationViewItem
        //    //{
        //        //Content = "My content",
        //        //Icon = new SymbolIcon((Symbol)0xF1AD),
        //        //Tag = "content"
        //    //});
        //    _pages.Add(("content", typeof(HomePage)));

        //    // Add handler for ContentFrame navigation.
        //    ContentFrame.Navigated += On_Navigated;

        //    // NavView doesn't load any page by default, so load home page.
        //    NavView.SelectedItem = NavView.MenuItems[0];
        //    // If navigation occurs on SelectionChanged, this isn't needed.
        //    // Because we use ItemInvoked to navigate, we need to call Navigate
        //    // here to load the home page.
        //    NavView_Navigate("home", new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());

        //    // Listen to the window directly so the app responds
        //    // to accelerator keys regardless of which element has focus.
        //    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=CoreDispatcher_AcceleratorKeyActivated;

        //    Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

        //    SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
        //    var settings = (muxc.NavigationViewItem)NavView.SettingsItem;
        //    settings.Content = "设置 [ Settings ]";
        //    settings.FontFamily = new FontFamily("{StaticResource HarmonyOSSans}");
        //}

        //private void NavView_ItemInvoked(muxc.NavigationView sender,
        //                                 muxc.NavigationViewItemInvokedEventArgs args)
        //{
        //    if (args.IsSettingsInvoked == true)
        //    {
        //        NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
        //    }
        //    else if (args.InvokedItemContainer != null)
        //    {
        //        var navItemTag = args.InvokedItemContainer.Tag.ToString();
        //        NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
        //    }
        //}

        // NavView_SelectionChanged is not used in this example, but is shown for completeness.
        // You will typically handle either ItemInvoked or SelectionChanged to perform navigation,
        // but not both.
        //private void NavView_SelectionChanged(muxc.NavigationView sender,
        //                                      muxc.NavigationViewSelectionChangedEventArgs args)
        //{
        //    if (args.IsSettingsSelected == true)
        //    {
        //        NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
        //    }
        //    else if (args.SelectedItemContainer != null)
        //    {
        //        var navItemTag = args.SelectedItemContainer.Tag.ToString();
        //        NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
        //    }
        //}
        //private void NavigationView_SelectionChanged(muxc.NavigationView sender,
        //                              muxc.NavigationViewSelectionChangedEventArgs args)
        //{
        //    if (args.IsSettingsSelected == true)
        //    {
        //        NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
        //    }
        //    else if (args.SelectedItemContainer != null)
        //    {
        //        var navItemTag = args.SelectedItemContainer.Tag.ToString();
        //        NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
        //    }
        //}
        //private void NavView_Navigate(
        //    string navItemTag,
        //    Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        //{
        //    Type _page = null;
        //    if (navItemTag == "settings")
        //    {
        //        _page = typeof(SettingsPage);
        //    }
        //    else
        //    {
        //        var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
        //        _page = item.Page;
        //    }
        //    // Get the page type before navigation so you can prevent duplicate
        //    // entries in the backstack.
        //    var preNavPageType = ContentFrame.CurrentSourcePageType;

        //    // Only navigate if the selected page isn't currently loaded.
        //    if (!(_page is null) && !Type.Equals(preNavPageType, _page))
        //    {
        //        ContentFrame.Navigate(_page, null, transitionInfo);
        //    }
        //}

        //private void NavView_BackRequested(muxc.NavigationView sender,
        //                                   muxc.NavigationViewBackRequestedEventArgs args)
        //{
        //    TryGoBack();
        //}

        //private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        //{
        //    // When Alt+Left are pressed navigate back
        //    if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
        //        && e.VirtualKey == VirtualKey.Left
        //        && e.KeyStatus.IsMenuKeyDown == true
        //        && !e.Handled)
        //    {
        //        e.Handled = TryGoBack();
        //    }
        //}

        //private void System_BackRequested(object sender, BackRequestedEventArgs e)
        //{
        //    if (!e.Handled)
        //    {
        //        e.Handled = TryGoBack();
        //    }
        //}

        //private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        //{
        //    // Handle mouse back button.
        //    if (e.CurrentPoint.Properties.IsXButton1Pressed)
        //    {
        //        e.Handled = TryGoBack();
        //    }
        //}

        //private bool TryGoBack()
        //{
        //    if (!ContentFrame.CanGoBack)
        //        return false;

        //    // Don't go back if the nav pane is overlayed.
        //    if (NavView.IsPaneOpen &&
        //        (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
        //         NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
        //        return false;

        //    ContentFrame.GoBack();
        //    return true;
        //}

        //private void On_Navigated(object sender, NavigationEventArgs e)
        //{
        //    NavView.IsBackEnabled = ContentFrame.CanGoBack;

        //    if (ContentFrame.SourcePageType == typeof(SettingsPage))
        //    {
        //        // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
        //        NavView.SelectedItem = (muxc.NavigationViewItem)NavView.SettingsItem;
        //    }
        //    else if (ContentFrame.SourcePageType != null)
        //    {
        //        var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

        //        NavView.SelectedItem = NavView.MenuItems
        //            .OfType<muxc.NavigationViewItem>()
        //            .First(n => n.Tag.Equals(item.Tag));

        //        //NavView.Header =
        //            //((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
        //    }
        //}
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

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}

