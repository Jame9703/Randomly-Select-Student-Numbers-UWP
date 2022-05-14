using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace 随机抽取学号.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page2 : Page
    {
        public Page2()
        {
            this.InitializeComponent();
            ComboBox1.Items.Add("-- --");
            ComboBox1.Items.Add("“ ”");
            ComboBox1.Items.Add("‘ ’");
            ComboBox1.SelectedIndex = 0;
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
               int x = int.Parse(txtFrom.Text);
               int y = int.Parse(txtTo.Text);
               int n = int.Parse(txtQuan.Text);
                Guid temp = Guid.NewGuid();
                int guidseed = BitConverter.ToInt32(temp.ToByteArray(), 1);
                Random r = new Random(guidseed);

                HashSet<int> Set = new HashSet<int>();
               aa: for (int i = 1; i <= n; i++)
               {
                    int random = r.Next(x, y + 1);
                    Set.Add(random);

                }
               if (n != Set.Count)
               {
                Set.Clear();
                goto aa;
               }

               foreach (int a in Set)
               {

                  if (ComboBox1.SelectedItem.ToString() == "-- --")
                  {
                    txtRe.Text += "--" + a.ToString() + "--";
                  }
                  else if (ComboBox1.SelectedItem.ToString() == "“ ”")
                  {
                    txtRe.Text += "“" + a.ToString() + "”";
                  }
                  else if (ComboBox1.SelectedItem.ToString() == "‘ ’")
                  {
                    txtRe.Text += "‘" + a.ToString() + "’";
                  }
               }
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
    }

}
