using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
            var ClassCount = StudentManager.ClassList.Count;
            var ClassNames = StudentManager.ClassList.Select(x => x.ClassName).ToList();
            if (!ClassNames.Contains(ClassNameTextBox.Text) && ClassNameTextBox.Text != String.Empty && ClassCount < 99)
            {
                Class _class = new Class
                {
                    ClassName = ClassNameTextBox.Text,
                    ClassEmblemPath = "ms-appx:///Assets/ClassEmblems/ClassEmblem1.png"
                };
                StudentManager.ClassList.Add(_class);
            }
            else if (ClassCount >= 99)
            {
                PopupNotice popupNotice = new PopupNotice("班级数量已达到限制");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
            }
            else if (ClassNameTextBox.Text == String.Empty)
            {
                PopupNotice popupNotice = new PopupNotice("班级名称不能为空");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("班级名称不能重复");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                popupNotice.ShowPopup();
            }
        }

        private void DeleteClassButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = ClassGridView.SelectedItems.ToList();
            foreach (var item in selectedItems)
            {
                StudentManager.ClassList.Remove(item as Class);
            }
        }

        private void RenameClassButton_Click(object sender, RoutedEventArgs e)
        {
            var _class = ClassGridView.SelectedItem as Class;
            if (_class != null)
            {
                var ClassNames = StudentManager.ClassList.Select(x => x.ClassName).ToList();
                if (!ClassNames.Contains(RenameClassNameTextBox.Text))
                {
                    _class.ClassName = RenameClassNameTextBox.Text;
                }
                else
                {
                    PopupNotice popupNotice = new PopupNotice("班级名称不能重复");
                    popupNotice.PopupContent.Severity = InfoBarSeverity.Warning;
                    popupNotice.ShowPopup();
                }
            }
            else
            {
                PopupNotice popupNotice = new PopupNotice("请选择要重命名的班级");
                popupNotice.PopupContent.Severity = InfoBarSeverity.Informational;
                popupNotice.ShowPopup();
            }
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            await StudentManager.SaveClassesAsync(StudentManager.ClassList);
            this.Hide();
        }
    }
}
