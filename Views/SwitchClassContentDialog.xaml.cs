using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using 随机抽取学号.Classes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace 随机抽取学号.Views
{
    public sealed partial class SwitchClassContentDialog : ContentDialog
    {
        public SwitchClassContentDialog()
        {
            this.InitializeComponent();

        }

        private async void ContentDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }

        private async void CloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await StudentManager.SaveClassesAsync(StudentManager.ClassList);
            this.Hide();
        }
    }
}
