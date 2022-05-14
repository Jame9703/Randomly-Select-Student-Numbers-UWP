using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page1 : Page
    {
        public Page1()
        {
            this.InitializeComponent();
            comboBox1.Items.Add("仅抽取学号");
            comboBox1.Items.Add("仅抽取姓名");
            comboBox1.Items.Add("抽取学号和姓名");
            comboBox1.SelectedIndex = 0;
            timer1 = new DispatcherTimer();
            timer1.Tick += timer_Tick;
        }
        public DispatcherTimer timer1;
        Random t = new Random();//初始化随机数函数
        string num1;//定义一个字符串变量，用于显示
        private void 滚动_Click(object sender, RoutedEventArgs e)
        {
            if (check.IsChecked == true)
            {
                timer1.Start();
                Text.Visibility = Visibility.Collapsed;
                PopupNotice popupNotice = new PopupNotice("正在滚动");
                popupNotice.ShowAPopup();
            }
            else
            {
                timer1.Start();
                Text.Visibility = Visibility.Visible;
            }
        }

        private void 停止_Click(object sender, RoutedEventArgs e)
        {
            timer1.Stop();
            Text.Visibility = Visibility.Visible;
        }


        // callback runs on UI thread
        void timer_Tick(object sender, object e)
        {
            if (comboBox1.SelectedItem.ToString() == "仅抽取学号")
            {
                this.num1 = Convert.ToString(t.Next(1, 49));//产生1到48之间的一个随机数
                Text.Text = num1;
            }
            else if (comboBox1.SelectedItem.ToString() == "仅抽取姓名")
            {
                string[] names = { "名字1", "名字2", "名字3", "名字4", "名字5", "名字6", "名字7", "名字8", "名字9", "名字10", "名字11", "名字12", "名字13", "名字14", "名字15", "名字16", "名字17", "名字18", "名字19", "名字20", "名字21", "名字22", "名字23", "名字24", "名字25", "名字26", "名字27", "名字28", "名字29", "名字30", "名字31", "名字32", "名字33", "名字34", "名字35", "名字36", "名字37", "名字38", "名字39", "名字40", "名字41", "名字42", "名字43", "名字44", "名字45", "名字46", "名字47", "名字48" };
                Text.Text = names[new Random().Next(0, names.Length)];
            }
            else if (comboBox1.SelectedItem.ToString() == "抽取学号和姓名")
            {
                string[] numname = { "1.名字1", "2.名字2", "3.名字3", "4.名字4", "5.名字5", "6.名字6", "7.名字7", "8.名字8", "9.名字9", "10.名字10", "11.名字11", "12.名字12", "13.名字13", "14.名字14", "15.名字15", "16.名字16", "17.名字17", "18.名字18", "19.名字19", "20.名字20", "21.名字21", "22.名字22", "23.名字23", "24.名字24", "25.名字25", "26.名字26", "27.名字27", "28.名字28", "29.名字29", "30.名字30", "31.名字31", "32.名字32", "33.名字33", "34.名字34", "35.名字35", "36.名字36", "37.名字37", "38.名字38", "39.名字39", "40.名字40", "41.名字41", "42.名字42", "43.名字43", "44.名字44", "45.名字45", "46.名字46", "47.名字47", "48.名字48" };
                Text.Text = numname[new Random().Next(0, numname.Length)];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Text.Visibility = Visibility.Collapsed;
        }

        private void AppBarToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Text.Visibility = Visibility.Visible;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            int a = int.Parse(Text1.Text);
            timer1.Interval = TimeSpan.FromMilliseconds(a);
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            Text1.Undo();
            int a = int.Parse(Text1.Text);
            timer1.Interval = TimeSpan.FromMilliseconds(a);

        }
    }
}
