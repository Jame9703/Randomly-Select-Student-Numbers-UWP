using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace 随机抽取学号.Views
{
    public sealed partial class WelcomeContentDialog : ContentDialog
    {
        public WelcomeContentDialog()
        {
            this.InitializeComponent();
            segmented.SelectedIndex = 0;
        }

        private void Segmented_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (segmented.SelectedIndex == 0)
            {
                IntroductionGrid.Visibility = Visibility.Visible;
                LicenseGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                IntroductionGrid.Visibility = Visibility.Collapsed;
                LicenseGrid.Visibility = Visibility.Visible;
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
