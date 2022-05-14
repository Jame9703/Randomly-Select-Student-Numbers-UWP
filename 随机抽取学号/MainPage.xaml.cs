using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        public MainPage()
        {
            this.InitializeComponent();
        }
        System.Random t = new System.Random();//初始化随机数函数


        private void Item1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            contentFrame.SourcePageType = typeof(Page1);
        }

        private void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            contentFrame.SourcePageType = typeof(Page2);
        }

    }
}

