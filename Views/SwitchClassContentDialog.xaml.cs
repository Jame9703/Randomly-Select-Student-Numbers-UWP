using CommunityToolkit.WinUI;
using System.Linq;
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

        private void MultiSelectToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (MultiSelectToggleSwitch.IsOn == false)
            {
                if (ClassGridView.SelectedItems.Count > 1)
                {
                    var lastselecteditem = ClassGridView.SelectedItems.Last();
                    ClassGridView.DeselectAll();
                    ClassGridView.SelectedItems.Add(lastselecteditem);
                }
            }
        }

        private void ClassGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassGridView.SelectedItems.Count > 1 && MultiSelectToggleSwitch.IsOn == false)
            {
                var lastselecteditem = ClassGridView.SelectedItems.Last();
                ClassGridView.SelectionChanged -= ClassGridView_SelectionChanged;
                ClassGridView.DeselectAll();
                ClassGridView.SelectionChanged += ClassGridView_SelectionChanged;
                ClassGridView.SelectedItems.Add(lastselecteditem);
            }
        }
    }
}
