using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NumbersPage : Page
    {

        public NumbersPage()
        {
            this.InitializeComponent();
        }
        public void c()
        {
            txtFrom.Text = "";
            txtQuan.Text = "";
            txtRe.Text = "";
            txtTo.Text = "";
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (txtFrom.Text == "" | txtTo.Text == "" | txtQuan.Text == "")
            {
                PopupNotice popupNotice = new PopupNotice("请输入完整信息");
                popupNotice.ShowAPopup();
            }
            else
            {
               int Start = int.Parse(txtFrom.Text);
               int End = int.Parse(txtTo.Text);
                int a; bool success = int.TryParse(txtQuan.Text, out a);
                if (success)
                {
                    // 转换成功
                    int TakeCount = int.Parse(txtQuan.Text);
                    // 生成指定范围内的数字序列
                    List<int> numbers = Enumerable.Range(Start, End - Start + 1).ToList();

                    // 使用随机数生成器对数字序列进行打乱
                    Random random = new Random();
                    numbers = numbers.OrderBy(x => random.Next()).ToList();

                    // 取前几位数字
                    List<int> Result = numbers.Take(TakeCount).ToList();
                    foreach (int result in Result)
                    {

                        if (ComboBox1.SelectedItem.ToString() == "-- --")
                        {
                            txtRe.Text += "--" + result.ToString() + "--";
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "“ ”")
                        {
                            txtRe.Text += "“" + result.ToString() + "”";
                        }
                        else if (ComboBox1.SelectedItem.ToString() == "‘ ’")
                        {
                            txtRe.Text += "‘" + result.ToString() + "’";
                        }
                    }
                }
                else
                {
                    // 转换失败，NumberBox中的值不是有效的整数
                    txtQuan.Text = "10";
                }

                /* Guid temp = Guid.NewGuid();
                int guidseed = BitConverter.ToInt32(temp.ToByteArray(), 1);
                Random r = new Random(guidseed);
                HashSet<int> Set = new HashSet<int>();
                re: for (int i = 1; i <= n; i++)
                {int random = r.Next(x, y + 1);Set.Add(random);}
                if (n != Set.Count)
                {Set.Clear();goto re;}    */
                


            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            c();
        }



        private void 清空结果_Click(object sender, RoutedEventArgs e)
        {
            txtRe.Text = "";
        }

        private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                txtRe.FontSize = slider.Value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Generate a list of numbers from 1 to 45
            List<int> numbers = new List<int>();
            for (int i = 1; i <= 45; i++)
            {
                numbers.Add(i);
            }

            // Shuffle the list
            Random random = new Random();
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int n = random.Next(i + 1);
                int temp = numbers[i];
                numbers[i] = numbers[n];
                numbers[n] = temp;
            }

            // Take the first 5 numbers in order
            List<int> selectedNumbers = numbers.Take(5).ToList();

            // Clear the GridView of any previous items
            RandomNumbersGridView.ItemsSource = null;

            // Add the selected numbers to the GridView
            RandomNumbersGridView.ItemsSource = selectedNumbers;

            // Define the DataTemplate for the GridView items
            //RandomNumbersGridView.ItemTemplate = (DataTemplate)this.Resources["NumberBoxDataTemplate"];
        }
    }

}
