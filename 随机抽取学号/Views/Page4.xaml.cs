using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using 随机抽取学号.Views;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page4 : Page
    {
        public Page4()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Page1 page = new Page1();
            int x = int.Parse(Text1.Text);
            page.timer1.Interval = TimeSpan.FromMilliseconds(x);
        }

        private void Text1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Page1 page = new Page1();
            int x = int.Parse(Text1.Text);
            page.timer1.Interval = TimeSpan.FromMilliseconds(x);
        }
    }
}
