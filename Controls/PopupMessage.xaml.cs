using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace 随机抽取学号.Controls
{
    public sealed partial class PopupMessage : UserControl
    {
        private Popup _popup = null;
        public  PopupMessage()
        {
            this.InitializeComponent();
            //将当前的长、宽赋值给控件
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
            _popup = new Popup();
            _popup.Child = this;
            this.Loaded += PopupMessageLoaded;
        }
        public void PopupMessageLoaded(object sender, RoutedEventArgs e)
        {
            this.PopupIn.Begin();
            this.PopupIn.Completed += PopupInCompleted;
        }
        public void ShowMessage()
        {
            _popup.IsOpen = true;
        }
        public async void PopupInCompleted(object sender, object e)
        {
            //在原地停留2坤秒
            await Task.Delay(5000);
            //将消失动画打开
            this.PopupOut.Begin();
            //popout动画完成后触发
            this.PopupOut.Completed += PopupOutCompleted;
        }
        public void PopupOutCompleted(object sender, object e)
        {
            _popup.IsOpen = false;
        }
        //public sealed partial class PopupMessage : UserControl
        //{
        //    public static void ShowPopupMessage(string message)
        //    {
        //        PopupMessage popup = new PopupMessage(message);
        //        popup.PopupContainer.Children.Add(popup);
        //    }
        //    public static void ShowPopupMessage(string title ,string message)
        //    {
        //        PopupMessage popup = new PopupMessage(title,message);
        //        MainPage.PopupContainerInstance.Children.Add(popup);
        //    }
        //    public static void ShowPopupMessage(string title, string message, InfoBarSeverity severity)
        //    {
        //        PopupMessage popup = new PopupMessage(title, message, severity);
        //        MainPage.PopupContainerInstance.Children.Add(popup);
        //    }
        //    public PopupMessage()
        //    {
        //        this.InitializeComponent();
        //    }
        //    public PopupMessage(string message)
        //    {
        //        this.InitializeComponent();
        //        MessageInfoBar.Title = "消息";
        //        MessageInfoBar.Content = message;
        //        AnimateIn();
        //        StartTimer();
        //    }
        //    public PopupMessage(string title,string message)
        //    {
        //        this.InitializeComponent();
        //        MessageInfoBar.Title = title;
        //        MessageInfoBar.Content= message;
        //        AnimateIn();
        //        StartTimer();
        //    }
        //    public PopupMessage(string title,string message,InfoBarSeverity severity)
        //    {
        //        this.InitializeComponent();
        //        MessageInfoBar.Title = title;
        //        MessageInfoBar.Content = message;
        //        MessageInfoBar.Severity = severity;
        //        AnimateIn();
        //        StartTimer();
        //    }
        //    private void AnimateIn()
        //    {
        //        DoubleAnimation opacityAnimation = new DoubleAnimation
        //        {
        //            From = 0,
        //            To = 1,
        //            Duration = TimeSpan.FromSeconds(0.3)
        //        };

        //        DoubleAnimation translateAnimation = new DoubleAnimation
        //        {
        //            From = 250,  // 从右侧开始
        //            To = 0,
        //            Duration = TimeSpan.FromSeconds(0.3)
        //        };

        //        Storyboard.SetTarget(opacityAnimation, this);
        //        Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
        //        Storyboard.SetTarget(translateAnimation, this.RenderTransform);
        //        Storyboard.SetTargetProperty(translateAnimation, "(CompositeTransform.TranslateX)");

        //        Storyboard storyboard = new Storyboard();
        //        storyboard.Children.Add(opacityAnimation);
        //        storyboard.Children.Add(translateAnimation);
        //        storyboard.Begin();
        //    }

        //    private void StartTimer()
        //    {
        //        DispatcherTimer timer = new DispatcherTimer();
        //        timer.Interval = TimeSpan.FromSeconds(5);
        //        timer.Tick += (s, e) =>
        //        {
        //            timer.Stop();
        //            AnimateOut();
        //        };
        //        timer.Start();
        //    }

        //    private void AnimateOut()
        //    {
        //        DoubleAnimation opacityAnimation = new DoubleAnimation
        //        {
        //            From = 1,
        //            To = 0,
        //            Duration = TimeSpan.FromSeconds(0.3)
        //        };

        //        DoubleAnimation translateAnimation = new DoubleAnimation
        //        {
        //            From = 0,
        //            To = 250,  // 回到右侧
        //            Duration = TimeSpan.FromSeconds(0.3)
        //        };

        //        Storyboard.SetTarget(opacityAnimation, this);
        //        Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
        //        Storyboard.SetTarget(translateAnimation, this.RenderTransform);
        //        Storyboard.SetTargetProperty(translateAnimation, "(CompositeTransform.TranslateX)");

        //        Storyboard storyboard = new Storyboard();
        //        storyboard.Children.Add(opacityAnimation);
        //        storyboard.Children.Add(translateAnimation);
        //        storyboard.Completed += (s, e) =>
        //        {
        //            if (this.Parent is Panel parentPanel)
        //            {
        //                parentPanel.Children.Remove(this);
        //            }
        //        };
        //        storyboard.Begin();
        //    }

        //    private void MessageInfoBar_Closed(InfoBar sender, InfoBarClosedEventArgs args)
        //    {

        //    }
        //}
    }
}
